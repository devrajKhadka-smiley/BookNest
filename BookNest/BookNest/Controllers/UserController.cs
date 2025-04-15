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

        //[HttpPost]
        //public IActionResult AddUser(User user)
        //{
        //    dbContext.Users.Add(user);
        //    dbContext.SaveChanges();
        //    return Ok(user);
        //}


        //[HttpGet]
        //public IActionResult GetAllUser()
        //{
        //    List<User> usersList = dbContext.Users.ToList();
        //    return Ok(usersList);
        //}
    }
}
