using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Book;
using BookNest.Models.Dto.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add-to-cart/{userId}")]
        public async Task<IActionResult> AddToCart(long userId, [FromBody] CreateCartDto addToCartDTO)
        {
            if (addToCartDTO == null || addToCartDTO.Quantity <= 0)
            {
                return BadRequest("Invalid product or quantity.");
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == addToCartDTO.BookId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += addToCartDTO.Quantity;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    BookId = addToCartDTO.BookId,
                    Quantity = addToCartDTO.Quantity
                };

                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();

            return Ok("Item successfully added to the cart.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(long userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Author)
                .Include(ci => ci.Items)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Publication)
                .Include(ci => ci.Items)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Genres)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found for this user.");
            }

            var cartDto = new
            {
                cart.Id,
                cart.CreatedAt,
                Items = cart.Items.Select(ci => new
                {
                    ci.BookId,
                    ci.Quantity,
                    BookTitle = ci.Book?.BookTitle,
                    BookDescription = ci.Book?.BookDescription,
                    BookPrice = ci.Book?.BookDiscountedPrice,
                    BookStock = ci.Book?.BookStock,
                    BookPublisher = ci.Book?.Publication?.PublicationName ?? "Unknown",
                    AuthorName = ci.Book?.Author?.FirstOrDefault()?.AuthorName ?? "Unknown",
                }).ToList()
            };

            return Ok(cartDto);
        }

        [HttpDelete("remove-from-cart/{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFromCart(long userId, Guid bookId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found for this user.");
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == bookId);

            if (cartItem == null)
            {
                return NotFound("Item not found in the cart.");
            }

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return Ok("Item successfully removed from the cart.");
        }

        [HttpPut("update-quantity/{userId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(long userId, [FromBody] CartQuantityUpdateDto dto)
        {
            if (dto == null || dto.BookId == Guid.Empty || dto.Quantity <= 0)
                return BadRequest("Invalid request data");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.BookId == dto.BookId);
            if (item == null)
                return NotFound("Cart item not found");

            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quantity updated successfully" });
        }

    }
}
