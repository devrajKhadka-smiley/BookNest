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

            var order = new Order
            {
                Id = Guid.NewGuid(), // Ensure a new unique GUID for each order
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = "In Process", // Set the initial status as "In Process"
                Items = new List<OrderItem>()
            };

            int totalBookCount = 0;
            decimal totalPrice = 0;

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

            // Apply 10% discount if user has 10+ successful orders
            decimal discount = user.SuccessfulOrderCount >= 10 ? 0.10m : 0m;
            decimal finalAmount = totalPrice * (1 - discount);
            order.TotalAmount = finalAmount;

            // Save order and update user
            _context.Orders.Add(order);
            _context.OrderItems.AddRange(order.Items);

            _context.CartItems.RemoveRange(cart.Items);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order Placed",
                OrderId = order.Id, // Return the newly generated orderId here
                BooksOrdered = totalBookCount,
                DiscountApplied = user.SuccessfulOrderCount >= 10 ? "10%" : "No Discount",
                FinalAmount = finalAmount,
                OrderStatus = order.Status // Display the current status of the order
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

            // Set the status to completed
            order.Status = "Completed";

            // Only increment the successful order count after the order is completed
            if (order.User != null)
            {
                order.User.SuccessfulOrderCount++; // Increment successful order count upon completion
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Order completed and user record updated.",
                OrderId = order.Id,
                UserSuccessfulOrderCount = order.User.SuccessfulOrderCount,
                OrderStatus = order.Status // Display the updated status
            });
        }
    }
}
