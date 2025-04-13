using BookNest.Data;
using BookNest.Data.Entities;
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

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return Ok(user);
        }


        [HttpGet]
        public IActionResult GetAllUser()
        {
            List<User> usersList = dbContext.Users.ToList();
            return Ok(usersList);
        }


        [HttpGet("/{id}")]
        public IActionResult GetUserById(Guid id)
        {
            User? user = dbContext.Users.FirstOrDefault(st => st.Id == id);
            return user != null ? Ok(user) : NotFound("User Not Found");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(User std, Guid id)
        {
            var user = dbContext.Users.FirstOrDefault(st => st.Id == id);
            if (user is not null)
            {
                user.Email = std.Email;
                user.PhoneNumber = std.PhoneNumber;
                user.Password = std.Password;
                user.FullName = std.FullName;
                user.UserName = std.UserName;
                user.Role = std.Role;

                dbContext.SaveChanges();
                return Ok(user);
            }
            return NotFound("Khai id nai veteyena");
        }

    }
}
