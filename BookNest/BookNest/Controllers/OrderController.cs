using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto;
using BookNest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using MailKit.Security;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        private const decimal FiveOrderDiscountRate = 0.05m;
        private const decimal TenOrderExtraDiscountRate = 0.10m;
        private const int FiveOrderThreshold = 5;
        private const int TenOrderThreshold = 10;

        public OrderController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //[HttpPost("place-order/{userId}")]
        //public async Task<IActionResult> PlaceOrder(long userId)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return NotFound("User not found");

        //    var membershipId = user.MemberShipId;

        //    var cart = await _context.Carts
        //        .Include(c => c.Items)
        //        .FirstOrDefaultAsync(c => c.UserId == userId);

        //    if (cart == null || !cart.Items.Any())
        //        return BadRequest("Cart is empty or does not exist");

        //    var otp = GenerateOtpService.GenerateOtp(4);
        //    var bookIds = cart.Items.Select(i => i.BookId).ToList();
        //    var books = await _context.Books
        //        .Where(b => bookIds.Contains(b.BookId))
        //        .ToDictionaryAsync(b => b.BookId);

        //    var order = new Order
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = userId,
        //        MembershipId = membershipId,
        //        CreatedAt = DateTime.UtcNow,
        //        ClaimCode = otp,
        //        Status = "In Process",
        //        OrderItems = new List<OrderItem>()
        //    };

        //    int totalBookCount = 0;
        //    decimal totalPrice = 0;

        //    foreach (var cartItem in cart.Items)
        //    {
        //        if (cartItem.Quantity <= 0)
        //            return BadRequest($"Cart item with ID {cartItem.Id} has invalid quantity.");

        //        if (!books.TryGetValue(cartItem.BookId, out var book))
        //            return NotFound($"Book with ID {cartItem.BookId} not found");

        //        if (book.BookStock < cartItem.Quantity)
        //            return BadRequest($"Insufficient stock for book: {book.BookTitle}");

        //        totalBookCount += cartItem.Quantity;

        //        var price = book.BookFinalPrice * cartItem.Quantity;
        //        totalPrice += price;

        //        var orderItem = new OrderItem
        //        {
        //            BookId = book.BookId,
        //            Quantity = cartItem.Quantity,
        //            PriceAtPurchase = book.BookFinalPrice,
        //            Order = order
        //        };

        //        order.OrderItems.Add(orderItem);
        //    }


        //    decimal discountRate = 0m;

        //    if (user.SuccessfulOrderCount >= FiveOrderThreshold)
        //    {
        //        discountRate += FiveOrderDiscountRate;

        //        if (user.SuccessfulOrderCount >= TenOrderThreshold)
        //        {
        //            discountRate += TenOrderExtraDiscountRate;
        //        }
        //    }

        //    decimal finalAmount = totalPrice * (1 - discountRate);
        //    order.TotalAmount = finalAmount;

        //    _context.Orders.Add(order);
        //    _context.OrderItems.AddRange(order.OrderItems);

        //    _context.CartItems.RemoveRange(cart.Items);
        //    _context.Carts.Remove(cart);

        //    await _context.SaveChangesAsync();

        //    string discountMessage = "No Discount";

        //    if (user.SuccessfulOrderCount >= TenOrderThreshold)
        //        discountMessage = "5% Discount (5+ Orders) + 10% Extra Discount (10+ Orders) Applied (Total 15%)";
        //    else if (user.SuccessfulOrderCount >= FiveOrderThreshold)
        //        discountMessage = "5% Discount (5+ Orders) Applied";
        //    else
        //        discountMessage = "No Discount (Less than 5 Orders)";


        //    try
        //    {
        //        var emailsettings = _config.GetSection("EmailConfig");

        //        var message = new MimeKit.MimeMessage();
        //        message.From.Add(new MimeKit.MailboxAddress(emailsettings["SenderName"], emailsettings["SenderEmail"]));
        //        message.To.Add(MimeKit.MailboxAddress.Parse(user.Email));
        //        message.Subject = "Order Confirmation - BookNest";
        //        message.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html)
        //        {
        //            Text = $"<h1>Order Confirmation</h1><p>Dear {user.UserName},</p><p>Your order has been successfully placed. Thank you for shopping with us!</p><p>Your OTP for confirming your order is: <strong>{otp}</strong></p><p>Best regards,<br>BookNest Team</p>"
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

        //    // return Ok("Order Placed");

        //    return Ok(new
        //    {
        //        Message = "Order Placed",
        //        OrderId = order.Id,
        //        BooksOrdered = totalBookCount,
        //        DiscountApplied = discountMessage,
        //        FinalAmount = finalAmount,
        //        OrderStatus = order.Status
        //    });
        //}

        [HttpPost("place-order/{userId}")]
        public async Task<IActionResult> PlaceOrder(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var membershipId = user.MemberShipId;
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty or does not exist");

            var otp = GenerateOtpService.GenerateOtp(4);
            var bookIds = cart.Items.Select(i => i.BookId).ToList();
            var books = await _context.Books
                .Where(b => bookIds.Contains(b.BookId))
                .ToDictionaryAsync(b => b.BookId);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MembershipId = membershipId,
                CreatedAt = DateTime.UtcNow,
                ClaimCode = otp,
                Status = "In Process",
                OrderItems = new List<OrderItem>()
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

                order.OrderItems.Add(orderItem);
            }

            decimal discountRate = 0m;

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

            _context.Orders.Add(order);
            _context.OrderItems.AddRange(order.OrderItems);

            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            string discountMessage = "No Discount";

            if (user.SuccessfulOrderCount >= TenOrderThreshold)
                discountMessage = "5% Discount (5+ Orders) + 10% Extra Discount (10+ Orders) Applied (Total 15%)";
            else if (user.SuccessfulOrderCount >= FiveOrderThreshold)
                discountMessage = "5% Discount (5+ Orders) Applied";

           
            try
            {
                // Wrap the user and order in an OrderEmailRequest
                var request = new OrderEmailRequest
                {
                    Order = order,
                    User = user
                };

                await SendOrderConfirmationEmail(request, otp); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

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

        [HttpPost]
        public async Task SendOrderConfirmationEmail(OrderEmailRequest request, string otp)
        {
            var order = request.Order;
            var user = request.User;

            byte[] invoicePdf = InvoiceService.GenerateInvoice(order);

            // Load config from appsettings
            var emailConfig = _config.GetSection("EmailConfig");
            var smtpServer = emailConfig["SmtpServer"];
            var port = int.Parse(emailConfig["Port"]);
            var senderName = emailConfig["Sender Name"];
            var senderEmail = emailConfig["SenderEmail"];
            var userName = emailConfig["UserName"];
            var password = emailConfig["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = "Order Confirmation";

            var body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                <h1>Order Confirmation</h1>
                <p>Dear {user.UserName},</p>
                <p>Your order has been successfully placed. Thank you for shopping with us!</p>
                <p>Your OTP for confirming your order is: <strong>{otp}</strong></p>
                <p>Best regards,<br>BookNest Team</p>"
            };

            var invoiceAttachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(invoicePdf)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = $"Invoice_{order.Id}.pdf"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(invoiceAttachment);
            message.Body = multipart;

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(userName, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
                throw; // Optional: rethrow to handle upstream
            }
        }


        [HttpPost("ViewOrderByStaff")]
        public async Task<IActionResult> ViewOrderByStaff([FromBody] StaffOrderDto request)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                //.Include(o => o.Items)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("No active order found");

            if (order.User.MemberShipId != request.MembershipId)
                return BadRequest("Membership ID does not match the order's user.");

            var orderDetails = order.OrderItems.Select(i => new
            {
                BookTitle = i.Book?.BookTitle ?? "Unknown",
                Quantity = i.Quantity,
                UnitPrice = i.PriceAtPurchase,
                SubTotal = i.Quantity * i.PriceAtPurchase
            });

            return Ok(new
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                Status = order.Status,
                UserName = order.User.UserName,
                Email = order.User.Email,
                OrderDetails = orderDetails,
                TotalAmount = order.TotalAmount
            });
        }

        [HttpPost("UpdateOrderStaff")]
        public async Task<IActionResult> UpdateOrderStaff([FromBody] StaffOrderDto request)
        {
            var order = await _context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderItems)
        .ThenInclude(od => od.Book)
        .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("No active order found");

            if (order.User.MemberShipId != request.MembershipId)
                return BadRequest("Invalid OTP / Claim Code.");

            order.Status = "Collected";
            order.OrderReceived = true;
            order.ClaimCode = null;


            foreach (var orderDetail in order.OrderItems)
            {
                var book = orderDetail.Book;

                book.BookStock -= orderDetail.Quantity;

                if (book.BookStock < 0)
                {
                    return BadRequest($"Insufficient stock for the book: {book.BookTitle}");
                }

                order.User.SuccessfulOrderCount += 1;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order verified and marked as Delivered",
                OrderId = order.Id,
                User = order.User.UserName,
                Status = order.Status
            });

        }

        [HttpPost("CancelOrderStaff")]
        public async Task<IActionResult> CancelOrderStaff([FromBody] StaffOrderDto request)
        {
            var order = await _context.Orders
        .Include(o => o.User)
        .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("No active order found");

            if (order.User.MemberShipId != request.MembershipId)
                return BadRequest("Invalid OTP / Claim Code.");

            order.Status = "Cancelled";
            order.OrderReceived = false;

            order.User.SuccessfulOrderCount += 1;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order verified and marked as Delivered",
                OrderId = order.Id,
                User = order.User.UserName,
                Status = order.Status
            });

        }

        [HttpGet("OrderListStaff")]
        public async Task<IActionResult> OrderListStaff(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var orders = await _context.Orders
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new
                    {
                        o.Id,
                        o.Status,
                        o.MembershipId,
                        o.CreatedAt,
                        BookCount = o.OrderItems.Count(),
                        OrderItems = o.OrderItems.Select(oi => new
                        {
                            oi.BookId,
                            oi.Quantity,
                            oi.PriceAtPurchase
                        }).ToList(),
                        UserEmail = o.User.Email,
                        UserName = o.User.UserName
                    })
                    .ToListAsync();

                if (orders == null || orders.Count == 0)
                {
                    return NotFound("No orders found.");
                }

                var totalOrders = await _context.Orders.CountAsync();

                var result = new
                {
                    Orders = orders,
                    TotalOrders = totalOrders,

                    TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize),
                    CurrentPage = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}