using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class IdentityContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {
            var user = new AppUser()
            {
                DisplayName = "Mohamed Yasser",
                Email = "my5511501@gmail.com",
                PhoneNumber = "01142647285",
                UserName = "MohamedYasser115"

            };
            if(_userManager.Users.Count() == 0)
                await _userManager.CreateAsync(user, "P@ssw0rd!");
            
        }
    }
}
