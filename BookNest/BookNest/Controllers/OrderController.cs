using BookNest.Data;
using BookNest.Data.Entities;
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

        public OrderController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("place-order/{userId}")]
        public async Task<IActionResult> Placeholder(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty or does not exist");

            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

            foreach (var cartItem in cart.Items)
            {
                if (cartItem.Quantity <= 0)
                    return BadRequest($"Cart item with ID {cartItem.Id} has invalid quantity.");

                var book = await _context.Books.FindAsync(cartItem.BookId);
                if (book == null)
                    return NotFound($"Book with ID {cartItem.BookId} not found");

                if (book.BookStock < cartItem.Quantity)
                    return BadRequest($"Insufficient stock for book: {book.BookTitle}");

                book.BookStock -= cartItem.Quantity;

                var orderItem = new OrderItem
                {
                    BookId = book.BookId,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = book.BookFinalPrice,
                    OrderId = order.Id,
                };

                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            _context.OrderItems.AddRange(order.Items);

            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            //---- send the mail to user
            try
            {
                var emailsettings = _config.GetSection("EmailConfig");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine($"SMTP Server: {emailsettings["SmtpServer"]}, SenderEmail: {emailsettings["SenderEmail"]}");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");


                var message = new MimeKit.MimeMessage();
                message.From.Add(new MimeKit.MailboxAddress(emailsettings["SenderName"], emailsettings["SenderEmail"]));
                message.To.Add(MimeKit.MailboxAddress.Parse(user.Email));
                message.Subject = "Order Confirmation - BookNest";
                message.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"<h1>Order Confirmation</h1><p>Dear {user.UserName},</p><p>Your order has been successfully placed. Thank you for shopping with us!</p><p>Best regards,<br>BookNest Team</p>"
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

            return Ok("Order Placed");
        }
    }
}
