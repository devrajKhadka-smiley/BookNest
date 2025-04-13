using BookNest.Data;
using BookNest.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly AppDbContext dbContext;

        public UserController (AppDbContext dbContext)
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
    }
}
