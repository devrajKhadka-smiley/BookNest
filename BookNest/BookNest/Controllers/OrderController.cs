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

        // Discount constants
        private const decimal FiveOrderDiscountRate = 0.05m; // 5% discount for 5+ orders
        private const decimal TenOrderExtraDiscountRate = 0.10m; // extra 10% for 10+ orders
        private const int FiveOrderThreshold = 5;
        private const int TenOrderThreshold = 10;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

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

            // ✅ Apply discount based on order count (not book count)
            decimal discountRate = 0m;

            // Check if user qualifies for order-based discounts
            if (user.SuccessfulOrderCount >= FiveOrderThreshold)
            {
                discountRate += FiveOrderDiscountRate; // Apply 5% discount for 5+ orders

                if (user.SuccessfulOrderCount >= TenOrderThreshold)
                {
                    discountRate += TenOrderExtraDiscountRate; // Stack extra 10% for 10+ orders
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
    }
}
