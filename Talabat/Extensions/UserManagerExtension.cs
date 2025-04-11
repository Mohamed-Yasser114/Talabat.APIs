using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<AppUser> FindUserEmailWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);  
            var User = userManager.Users.Include(U => U.Address).SingleOrDefault(U => U.Email == email);
            return User;
        }
    }
}
