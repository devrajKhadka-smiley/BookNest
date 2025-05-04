using BookNest.Data.Entities;
using BookNest.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookNest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister(RegisterUserDto registeruserDto)
        {

            var existingUserByUsername = await userManager.FindByNameAsync(registeruserDto.UserName!);
            if (existingUserByUsername != null)
            {
                return BadRequest(new { message = "Username already exists.😏" });
            }

            var existingUserByEmail = await userManager.FindByEmailAsync(registeruserDto.Email!);
            if (existingUserByEmail != null)
            {
                return BadRequest(new { message = "This email is already used in another account." });
            }


            User user = new User
            {
                UserName = registeruserDto.UserName,
                Firstname = registeruserDto.Firstname,
                Lastname = registeruserDto.Lastname,
                Email = registeruserDto.Email,
                Address = registeruserDto.Address,
                MemberShipId = Guid.NewGuid().ToString("N")
            };


            var result = await userManager.CreateAsync(user, registeruserDto.Password!);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Member");
                return Ok("Register Successful");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("staffregister")]
        [Authorize(Roles = "Admin")]
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

            var result = await userManager.CreateAsync(user, registeruserDto.Password!);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Staff");
                return Ok("Staff Registration Successful");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email!);
            if (user == null)
            {
                return Unauthorized(new { message = "No such email address found." });
            }

            if (!await userManager.IsInRoleAsync(user, "Member"))
            {
                return Unauthorized(new { message = "Such user doesn't exist" });
            }

            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password!, false, false);

            if (result.Succeeded)
            {
                return Ok(new { message = "Login Successful" });
            }

            return Unauthorized(new { message = "Some credentials didn't match 😒." });
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email!);
            if (user == null)
            {
                return Unauthorized(new { message = "No such email address found." });
            }

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            var isStaff = await userManager.IsInRoleAsync(user, "Staff");

            if (!isAdmin && !isStaff)
            {
                return Unauthorized(new { message = "Such user doesn't exist" });
            }

            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password!, false, false);

            if (result.Succeeded)
            {
                return Ok(new { message = "Login Successful" });
            }

            return Unauthorized(new { message = "Some credentials didn't match 😒." });
        }

    }
}
