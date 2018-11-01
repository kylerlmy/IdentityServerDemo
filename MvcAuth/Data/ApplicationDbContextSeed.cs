using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MvcAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAuth.Data
{
    public class ApplicationDbContextSeed
    {
        private UserManager<ApplicationUser> _userManager;

        public async Task SeedAsync(ApplicationDbContext context,IServiceProvider service)
        {
            if (!context.Users.Any())
            {
                _userManager = service.GetRequiredService<UserManager<ApplicationUser>>();

                var defaultUser = new ApplicationUser
                {
                    Email = "kylerlmy@163.com",
                    UserName = "Adminitrator",
                    NormalizedUserName = "admin"
                };

                var user = await _userManager.CreateAsync(defaultUser, "Password$123");

                if (!user.Succeeded)
                    throw new Exception("创建默认用户失败");
            }
        }
    }
}
