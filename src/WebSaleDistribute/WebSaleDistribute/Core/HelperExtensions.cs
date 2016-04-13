using System;
using System.Security.Cryptography;
using System.Text;
using AdoManager;

namespace WebSaleDistribute.Core
{
    public static class HelperExtensions
    {
        public static ConnectionManager UsersManagementsDb => AdoManager.ConnectionManager.Find("UsersManagements");

        public static string GetMd5Bytes(this string input)
        {
            byte[] result;
            var buffer = Encoding.UTF8.GetBytes(input);

            using (var md5 = new MD5CryptoServiceProvider())
            {
                result = md5.ComputeHash(buffer);
            }

            return result.ToHex(false);
        }

        public static string ToHex(this byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

    }
}
