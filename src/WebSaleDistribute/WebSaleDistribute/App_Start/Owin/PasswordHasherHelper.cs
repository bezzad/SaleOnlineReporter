using System;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using WebSaleDistribute.Core;

namespace WebSaleDistribute.Owin
{

    public class PasswordHasherHelper : PasswordHasher
    {

        public FormsAuthPasswordFormat FormsAuthPasswordFormat { get; set; }

        public PasswordHasherHelper(FormsAuthPasswordFormat format)
        {
            FormsAuthPasswordFormat = format;
        }

        public override string HashPassword(string password)
        {
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(password, FormsAuthPasswordFormat.ToString());
            return password.GetMd5();
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            //var providedPasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(providedPassword, FormsAuthPasswordFormat.ToString());

            var providedPasswordHash = providedPassword.GetMd5();

            return hashedPassword.Equals(providedPasswordHash, StringComparison.CurrentCultureIgnoreCase) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
