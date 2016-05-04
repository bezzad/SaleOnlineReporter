using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using WebSaleDistribute.Models;
using WebSaleDistribute.Owin;
using System.Threading.Tasks;
using System.Web;
using WebSaleDistribute.Core;

namespace WebSaleDistribute
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // for see properties details: https://msdn.microsoft.com/en-us/library/microsoft.owin.security.cookies.cookieauthenticationoptions(v=vs.113).aspx
                //
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  

                    //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                    //    validateInterval: TimeSpan.FromMinutes(15),
                    //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))

                    OnValidateIdentity = async (context) => { await CustomValidateIdentity(context); } // refer to the implementation below
                },
                // ref: http://www.jamessturtevant.com/posts/ASPNET-Identity-Cookie-Authentication-Timeouts/
                // CookieAuthenticationOptions.ExpireTimespan is the option that allows you to set how long the issued cookie is 
                // valid for. In the example above, the cookie is valid for 30 minutes from the time of creation. Once those 30 minutes are 
                // up the user will have to sign back in becuase the SlidingExpiration is set to false.
                //
                // If SlidingExpiration is set to true then the cookie would be re - issued on any request half way through the 
                // ExpireTimeSpan.For example, if the user logged in and then made a second request 16 minutes later the cookie 
                // would be re - issued for another 30 minutes.If the user logged in and then made a second request 31 minutes later then 
                // the user would be prompted to log in.
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromSeconds(Properties.Settings.Default.UserAuthenticateExpireTimeSec),
                CookieName = Properties.Settings.Default.UserAuthenticateCookieName, // The default value is ".AspNet.Cookies"
                //
                // Determines if the browser should allow the cookie to be accessed by client-side javascript. 
                // The default is true, which means the cookie will only be passed to http requests and is not made available to script on the page.
                CookieHttpOnly = false,
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }


        /// <summary>
        /// this method will be called on every request
        /// it is also one of the few places where you can access unencrypted cookie content as CookieValidateIdentityContext
        /// once you get cookie information you need, keep it as one of the Claims
        /// please ignore the MyUserManager and MyUser classes, they are only for sample, you should have yours
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task CustomValidateIdentity(CookieValidateIdentityContext context)
        {
            // validate security stamp for 'sign out everywhere'
            // here I want to verify the security stamp in every 120 seconds.
            // but I choose not to regenerate the identity cookie, so I passed in NULL 
            var stampValidator = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                validateInterval: TimeSpan.FromSeconds(Properties.Settings.Default.UserAuthenticateValidateIntervalSec), // refresh expire time every UserAuthenticateValidateInterval sec
                regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager));


            // here we get the cookie expiry time
            var expireUtc = context.Properties.ExpiresUtc;

            // add the expiry time back to cookie as one of the claims, called 'UserLoginExpireUtc'
            // to ensure that the claim has latest value, we must keep only one claim
            // otherwise we will be having multiple claims with same type but different values
            var claimType = Properties.Settings.Default.LoginExpireClaimType;
            var identity = context.Identity;
            if (identity.HasClaim(c => c.Type == claimType))
            {
                var existingClaim = identity.FindFirst(claimType);
                identity.RemoveClaim(existingClaim);
            }
            var newClaim = new System.Security.Claims.Claim(claimType, expireUtc.Value.LocalDateTime.Ticks.ToString());
            context.Identity.AddClaim(newClaim);

            var reloadTimeout = context.Identity.ExpireRemainingTime();
            HttpContext.Current.Response.SetCookie(Properties.Settings.Default.ReloadPageTimeoutIntervalCookieName, reloadTimeout, expireUtc.Value.DateTime.AddMinutes(1));

            await Task.Run(() => stampValidator.Invoke(context));
        }

    }
}