using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("overview")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> GetSalesOverview()
        {
            var data = await _context.Orders
                .Where(o => o.OrderReceived)
                .ToListAsync();

            var grouped = data
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesOverviewDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Sales = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToList();

            foreach (var order in data)
            {
                Console.WriteLine(order.CreatedAt);
            }

            return Ok(grouped);
        }

        [HttpGet("statstabs")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> GetStatisticsOverview()
        {
            // Pending Orders
            var pendingOrderCount = await _context.Orders
                .Where(o => o.Status == "In Process")
                .CountAsync();

            // Successful Orders
            var successfulOrderCount = await _context.Orders
                .Where(o => o.Status == "Collected")
                .CountAsync();

            // Cancelled Orders
            var cancelledOrderCount = await _context.Orders
                .Where(o => o.Status == "Cancelled")
                .CountAsync();

            // Members with more than 10 successful orders
            var memberCount = await _context.Users
                .Where(u => u.SuccessfulOrderCount >= 10)
                .CountAsync();

            // Return all stats in a single response
            return Ok(new
            {
                PendingOrders = pendingOrderCount,
                SuccessfulOrders = successfulOrderCount,
                CancelledOrders = cancelledOrderCount,
                MembersWithFewerThan10SuccessfulOrders = memberCount
            });
        }

        [HttpGet("recent")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> GetRecentOrders()
        {
            var recentOrders = await _context.Orders
         .Where(o => o.Status == "In Process")
         .OrderByDescending(o => o.CreatedAt)
         .Take(4)
         .Select(o => new
         {
             o.Id,
             o.MembershipId,
             o.TotalAmount,
             o.Status,
             o.OrderReceived,
             CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
         })
         .ToListAsync();

            return Ok(recentOrders);
        }


    }
}
