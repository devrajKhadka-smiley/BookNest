using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        // public OrderController(AppDbContext context, IConfiguration config)
        // Discount constants
        private const decimal FiveOrderDiscountRate = 0.05m; // 5% discount for 5+ orders
        private const decimal TenOrderExtraDiscountRate = 0.10m; // extra 10% for 10+ orders
        private const int FiveOrderThreshold = 5;
        private const int TenOrderThreshold = 10;

        public OrderController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //[HttpPost("place-order/{userId}")]
        //public async Task<IActionResult> Placeholder(long userId)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return NotFound("User not found");

        //    var cart = await _context.Carts
        //        .Include(c => c.Items)
        //        .FirstOrDefaultAsync(c => c.UserId == userId);

        //    if (cart == null || !cart.Items.Any())
        //        return BadRequest("Cart is empty or does not exist");

        //    var order = new Order
        //    {
        //        UserId = userId,
        //        CreatedAt = DateTime.UtcNow,
        //    };

        //    foreach (var cartItem in cart.Items)
        //    {
        //        if (cartItem.Quantity <= 0)
        //            return BadRequest($"Cart item with ID {cartItem.Id} has invalid quantity.");

        //        var book = await _context.Books.FindAsync(cartItem.BookId);
        //        if (book == null)
        //            return NotFound($"Book with ID {cartItem.BookId} not found");

        //        if (book.BookStock < cartItem.Quantity)
        //            return BadRequest($"Insufficient stock for book: {book.BookTitle}");

        //        book.BookStock -= cartItem.Quantity;

        //        var orderItem = new OrderItem
        //        {
        //            BookId = book.BookId,
        //            Quantity = cartItem.Quantity,
        //            PriceAtPurchase = book.BookFinalPrice,
        //            OrderId = order.Id,
        //        };

        //        order.Items.Add(orderItem);
        //    }

        //    _context.Orders.Add(order);
        //    _context.OrderItems.AddRange(order.Items);

        //    _context.CartItems.RemoveRange(cart.Items);
        //    _context.Carts.Remove(cart);

        //    await _context.SaveChangesAsync();

        //    //---- send the mail to user
        //    try
        //    {
        //        var emailsettings = _config.GetSection("EmailConfig");

        //        var otp = GenerateOtpService.GenerateOtp(4);
        //        var message = new MimeKit.MimeMessage();
        //        message.From.Add(new MimeKit.MailboxAddress(emailsettings["SenderName"], emailsettings["SenderEmail"]));
        //        message.To.Add(MimeKit.MailboxAddress.Parse(user.Email));
        //        message.Subject = "Order Confirmation - BookNest";
        //        message.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html)
        //        {
        //            Text = $"<h1>Order Confirmation</h1><p>Dear {user.UserName},</p><p>Your order has been successfully placed. Thank you for shopping with us!</p><p>Best regards,<br>BookNest Team</p>"
        //        };

        //        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        //        await smtp.ConnectAsync(emailsettings["SmtpServer"], int.Parse(emailsettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
        //        await smtp.AuthenticateAsync(emailsettings["Username"], emailsettings["Password"]);
        //        await smtp.SendAsync(message);
        //        await smtp.DisconnectAsync(true);
        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine($"Email failed: {ex.Message}");
        //    }

        //    return Ok("Order Placed");
        //}


        [HttpPost("place-order/{userId}")]
        public async Task<IActionResult> PlaceOrder(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty or does not exist");

            // Step 1: Generate OTP and store it in the order
            var otp = GenerateOtpService.GenerateOtp(4);
            // Fetch books in bulk
            var bookIds = cart.Items.Select(i => i.BookId).ToList();
            var books = await _context.Books
                .Where(b => bookIds.Contains(b.BookId))
                .ToDictionaryAsync(b => b.BookId);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ClaimCode = otp,
                Status = "In Process",
                Items = new List<OrderItem>()
            };

            int totalBookCount = 0;
            decimal totalPrice = 0;

            foreach (var cartItem in cart.Items)
            {
                if (cartItem.Quantity <= 0)
                    return BadRequest($"Cart item with ID {cartItem.Id} has invalid quantity.");

                if (!books.TryGetValue(cartItem.BookId, out var book))
                    return NotFound($"Book with ID {cartItem.BookId} not found");

                if (book.BookStock < cartItem.Quantity)
                    return BadRequest($"Insufficient stock for book: {book.BookTitle}");

                book.BookStock -= cartItem.Quantity;
                totalBookCount += cartItem.Quantity;

                var price = book.BookFinalPrice * cartItem.Quantity;
                totalPrice += price;

                var orderItem = new OrderItem
                {
                    BookId = book.BookId,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = book.BookFinalPrice,
                    Order = order
                };

                order.Items.Add(orderItem);
            }


            decimal discountRate = 0m;

            // Check if user qualifies for order-based discounts
            if (user.SuccessfulOrderCount >= FiveOrderThreshold)
            {
                discountRate += FiveOrderDiscountRate; 

                if (user.SuccessfulOrderCount >= TenOrderThreshold)
                {
                    discountRate += TenOrderExtraDiscountRate; 
                }
            }

            decimal finalAmount = totalPrice * (1 - discountRate);
            order.TotalAmount = finalAmount;

            // Save order and clear cart
            _context.Orders.Add(order);
            _context.OrderItems.AddRange(order.Items);

            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            // Discount message
            string discountMessage = "No Discount";

            if (user.SuccessfulOrderCount >= TenOrderThreshold)
                discountMessage = "5% Discount (5+ Orders) + 10% Extra Discount (10+ Orders) Applied (Total 15%)";
            else if (user.SuccessfulOrderCount >= FiveOrderThreshold)
                discountMessage = "5% Discount (5+ Orders) Applied";
            else
                discountMessage = "No Discount (Less than 5 Orders)";


            try
            {
                var emailsettings = _config.GetSection("EmailConfig");

                var message = new MimeKit.MimeMessage();
                message.From.Add(new MimeKit.MailboxAddress(emailsettings["SenderName"], emailsettings["SenderEmail"]));
                message.To.Add(MimeKit.MailboxAddress.Parse(user.Email));
                message.Subject = "Order Confirmation - BookNest";
                message.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"<h1>Order Confirmation</h1><p>Dear {user.UserName},</p><p>Your order has been successfully placed. Thank you for shopping with us!</p><p>Your OTP for confirming your order is: <strong>{otp}</strong></p><p>Best regards,<br>BookNest Team</p>"
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(emailsettings["SmtpServer"], int.Parse(emailsettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(emailsettings["Username"], emailsettings["Password"]);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            // return Ok("Order Placed");

            return Ok(new
            {
                Message = "Order Placed",
                OrderId = order.Id,
                BooksOrdered = totalBookCount,
                DiscountApplied = discountMessage,
                FinalAmount = finalAmount,
                OrderStatus = order.Status
            });
        }

        [HttpPost("complete-order/{orderId}")]
        public async Task<IActionResult> CompleteOrder(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("Order is already completed or invalid.");

            order.Status = "Completed";

            if (order.User != null)
            {
                order.User.SuccessfulOrderCount++; // Increment order count when completed
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order completed and user record updated.",
                OrderId = order.Id,
                UserSuccessfulOrderCount = order.User.SuccessfulOrderCount,
                OrderStatus = order.Status
            });
        }
    };
}
