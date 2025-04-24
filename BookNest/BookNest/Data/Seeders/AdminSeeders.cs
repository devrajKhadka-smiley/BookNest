using BookNest.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookNest.Data.Seeders
{
    public static class AdminSeeders
    {
        public static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new Role { Name = "Admin" });
            }

            var adminUser = await userManager.FindByNameAsync("admin");

            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@booknest.com"
                };

                string defaultPassword = "Admin@123";

                var result = await userManager.CreateAsync(user, defaultPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
