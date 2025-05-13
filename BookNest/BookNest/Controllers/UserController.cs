using BookNest.Data;
using BookNest.Data.Entities;
using BookNest.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public UserController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetAllUser([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var query = from user in dbContext.Users
                        join userRole in dbContext.UserRoles on user.Id equals userRole.UserId
                        join role in dbContext.Roles on userRole.RoleId equals role.Id
                        where role.Name == "Member"
                        select user;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchTerm.ToLower()) ||
                    u.UserName.ToLower().Contains(searchTerm.ToLower()));

                var users = query.ToList();

                return Ok(new
                {
                    totalMembers = users.Count,
                    totalPages = 1,
                    pageNumber = 1,
                    pageSize = users.Count,
                    users
                });
            }

            var totalMembers = query.Count();

            var totalPages = (int)Math.Ceiling(totalMembers / (double)pageSize);

            if (pageNumber > totalPages)
            {
                return Ok(new
                {
                    message = "Page number exceeds available data.",
                    totalMembers,
                    totalPages,
                    pageNumber,
                    pageSize,
                    users = new List<object>()
                });
            }


            var usersWithPagination = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                totalMembers,
                totalPages,
                pageNumber,
                pageSize,
                users = usersWithPagination
            });
        }


        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Address = user.Address,
                MemberShipId = user.MemberShipId,
                SuccessfulOrderCount = user.SuccessfulOrderCount,
            };

            return Ok(userDto);
            //return Ok(user);
        }

        [HttpGet("staff")]
        public IActionResult GetAllStaff([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var staffQuery = from user in dbContext.Users
                             join userRole in dbContext.UserRoles on user.Id equals userRole.UserId
                             join role in dbContext.Roles on userRole.RoleId equals role.Id
                             where role.Name == "Staff"
                             orderby user.UserName
                             select new
                             {
                                 user.UserName,
                                 user.Firstname,
                                 user.Lastname,
                                 user.PhoneNumber,
                                 user.Email
                             };

            var totalRecords = staffQuery.Count();
            var paginatedStaff = staffQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                data = paginatedStaff,
                totalRecords
            });
        }



    }
}
