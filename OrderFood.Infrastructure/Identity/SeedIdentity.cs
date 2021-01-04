using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderFood.Domain.Identity;

namespace OrderFood.Infrastructure.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed(IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var usersCount = await userManager.Users.CountAsync();
            var rolesCount = await roleManager.Roles.CountAsync();

            if (usersCount == 0 && rolesCount == 0)
            {
                var adminUser = new User
                {
                    FirstName = configuration["Data:AdminUser:FirstName"],
                    LastName = configuration["Data:AdminUser:LastName"],
                    UserName = configuration["Data:AdminUser:UserName"],
                    Email = configuration["Data:AdminUser:Email"],
                    EmailConfirmed = true
                };

                var password = configuration["Data:AdminUser:Password"];
                var adminRole = configuration["Data:AdminUser:Role"];

                await roleManager.CreateAsync(new IdentityRole(adminRole));

                await userManager.CreateAsync(adminUser, password);

                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
