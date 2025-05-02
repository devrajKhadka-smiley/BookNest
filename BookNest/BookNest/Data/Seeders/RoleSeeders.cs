using BookNest.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookNest.Data.Seeders
{
    public static class RoleSeeders
    {
        public static async Task SeedRolesAsync(RoleManager<Role> roleManager)
        {
            string[] roleNames = { "Admin", "Member", "Staff" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }
    }
}
