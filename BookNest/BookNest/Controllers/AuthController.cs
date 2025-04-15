using BookNest.Data.Entities;
using BookNest.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase 
    {
        private readonly UserManager<User> userManager;

        public AuthController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister(RegisterUserDto registeruserDto)
        {
            User user = new User
            {
                UserName = registeruserDto.UserName,
                Firstname = registeruserDto.Firstname,
                Lastname = registeruserDto.Lastname,
                Email = registeruserDto.Email,
                Address = registeruserDto.Address,
                MemberShipId = Guid.NewGuid().ToString("N") 
            };

            // Create the user with the provided password
            var result = await userManager.CreateAsync(user, registeruserDto.Password);

            if (result.Succeeded)
            {
                // Add user to the "Member" role
                await userManager.AddToRoleAsync(user, "Member");
                return Ok("Register Successful");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("staff-register")]
        public async Task<IActionResult> StaffRegister(RegisterUserDto registeruserDto)
        {
            User user = new User
            {
                UserName = registeruserDto.UserName,
                Firstname = registeruserDto.Firstname,
                Lastname = registeruserDto.Lastname,
                Email = registeruserDto.Email,
                Address = registeruserDto.Address,
                MemberShipId = Guid.NewGuid().ToString("N")
            };

            var result = await userManager.CreateAsync(user, registeruserDto.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Staff");
                return Ok("Staff Registration Successful");
            }

            return BadRequest(result.Errors);
        }
    }
}
