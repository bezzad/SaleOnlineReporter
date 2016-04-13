using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Owin
{
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            long imei;
            var user = long.TryParse(userName, out imei) 
                ? UserManager.Users.FirstOrDefault(u => u.IMEI == imei.ToString()) 
                : null;

            if (null != user)
            {
                userName = user.UserName;
                if (user.LockoutEnabled)
                {
                    return (SignInStatus.LockedOut);
                }
            }

            return await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }
    }
}
