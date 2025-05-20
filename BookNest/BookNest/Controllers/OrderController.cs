using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Hubs;
using BookNest.Models.Dto;
using BookNest.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;

namespace BookNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHubContext<OrderHub> _hubContext;

        private const decimal FiveOrderDiscountRate = 0.05m;
        private const decimal TenOrderExtraDiscountRate = 0.10m;
        private const int FiveOrderThreshold = 5;
        private const int TenOrderThreshold = 10;

        public OrderController(AppDbContext context, IConfiguration config, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _config = config;
            _hubContext = hubContext;
        }


        [HttpPost("place-order/{userId}")]
        [Authorize(Roles = "Member")]
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

            // var otp = GenerateOtpService.GenerateOtp(4);
            // Step 1: Generate OTP
            var otp = GenerateOtpService.GenerateOtp(6);

            // Fetch books in bulk
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
                OrderItems = new List<OrderItem>(),
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
                var price = book.BookDiscountedPrice * cartItem.Quantity;
                totalPrice += price;

                var orderItem = new OrderItem
                {
                    BookId = book.BookId,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = book.BookDiscountedPrice,
                    Order = order
                };

                order.OrderItems.Add(orderItem);
            }

            // --- Discount Logic ---

            decimal discountRate = 0m;
            decimal finalAmount = totalPrice;

            if (totalBookCount >= 5)
            {
                discountRate = 0.05m;
                finalAmount = finalAmount * (1 - discountRate);
            }

            if (user.SuccessfulOrderCount > 0 && user.SuccessfulOrderCount % 10 == 0)
            {
                discountRate = 0.10m;
                finalAmount = finalAmount * (1 - discountRate);
            }

            //decimal finalAmount = totalPrice * (1 - discountRate);
            order.TotalAmount = finalAmount;


            _context.Orders.Add(order);
            _context.OrderItems.AddRange(order.OrderItems);

            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            user.SuccessfulOrderCount += 1;
            await _context.SaveChangesAsync();


            bool isFiveBooksDiscount = totalBookCount >= 5;
            bool isTenOrdersDiscount = user.SuccessfulOrderCount > 0 && user.SuccessfulOrderCount % 10 == 1;

            string discountMessage = "No Discount Applied";

            if (isFiveBooksDiscount && isTenOrdersDiscount)
                discountMessage = "5% Discount (5+ Books in Order) + 10% Extra Discount (Reached 10/20/30+ Successful Orders) ";
            else if (isFiveBooksDiscount)
                discountMessage = "5% Discount (5+ Books in Order) Applied";
            else if (isTenOrdersDiscount)
                discountMessage = "10% Extra Discount (Reached 10/20/30+ Successful Orders) Applied";

            try
            {
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
                throw;
            }
        }

        [HttpPost("ViewOrderByStaff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> ViewOrderByStaff([FromBody] ClaimCodeDto input)
        {
            var claimCode = input.ClaimCode;

            if (string.IsNullOrWhiteSpace(claimCode))
                return BadRequest("Claim code is required.");

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(o => o.ClaimCode == claimCode);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("No active order found");

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
                Memberid = order.MembershipId,
                OrderDetails = orderDetails,
                TotalAmount = order.TotalAmount
            });
        }

        [HttpPost("UpdateOrderStaff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateOrderStaff([FromBody] string claimCode)
        {
            if (string.IsNullOrWhiteSpace(claimCode))
                return BadRequest("Claim code is required.");

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.ClaimCode == claimCode);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("No active order found");

            // Step 1: Validate stock for all books BEFORE confirming order
            foreach (var item in order.OrderItems)
            {
                var book = item.Book;
                if (book == null)
                    return BadRequest($"Book with ID {item.BookId} not found.");

                if (book.BookStock < item.Quantity)
                    return BadRequest($"Insufficient stock for the book: {book.BookTitle}");
            }

            // Step 2: Deduct stock now that validation passed
            foreach (var item in order.OrderItems)
            {
                item.Book.BookStock -= item.Quantity;
            }

            // Step 3: Mark order as collected & received
            order.Status = "Collected";
            order.OrderReceived = true;
            order.ClaimCode = null;

            // Step 4: Broadcast each book to connected clients
            foreach (var item in order.OrderItems)
            {
                var book = item.Book;
                if (book == null) continue;

                var message = $"{order.User.UserName} Just Collected: \"{book.BookTitle}\"!";
                await _hubContext.Clients.All.SendAsync("ReceiveOrderBroadcast", message);
            }

            // Step 5: Save all changes
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order verified and marked as Delivered",
                OrderId = order.Id,
                User = order.User.UserName,
                Status = order.Status,
                UserSuccessfulOrderCount = order.User?.SuccessfulOrderCount,
                OrderStatus = order.Status
            });
        }


        [HttpPost("CancelOrderStaff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CancelOrderStaff([FromBody] string claimCode)
        {
            if (string.IsNullOrWhiteSpace(claimCode))
                return BadRequest("Claim code is required.");

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.ClaimCode == claimCode);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != "In Process")
                return BadRequest("No active order found");

            order.Status = "Cancelled";
            order.OrderReceived = false;
            order.ClaimCode = null;

            // Decrease successful order count only if it's > 0
            if (order.User.SuccessfulOrderCount > 0)
            {
                order.User.SuccessfulOrderCount -= 1;
                Console.WriteLine("Decreased SuccessfulOrderCount");
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order verified and marked as Cancelled",
                OrderId = order.Id,
                User = order.User.UserName,
                Status = order.Status
            });
        }



        [HttpPost("CancelOrderUser")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CancelOrderUser([FromBody] StaffOrderDto request)
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
            order.ClaimCode = null;

            //order.User.SuccessfulOrderCount -= 1;
            order.User.SuccessfulOrderCount -= 1;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order verified and marked as Cancelled",
                OrderId = order.Id,
                User = order.User.UserName,
                Status = order.Status
            });
        }

        [HttpGet("OrderListStaff")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> OrderListStaff(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Orders
                    .Where(o => o.Status == "In Process");

                var totalOrders = await query.CountAsync();

                var orders = await query
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
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }


        [HttpGet("send")]
        public async Task<IActionResult> SendTestMessage()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveOrderBroadcast", "This is a test broadcast!");
            return Ok("Test message sent.");
        }
    }
}
