using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AdoManager;
using System.Data;
using System.Linq;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Core
{
    public static class HelperExtensions
    {
        public static string GetMd5(this string input)
        {
            byte[] result;
            var buffer = Encoding.UTF8.GetBytes(input);

            using (var md5 = new MD5CryptoServiceProvider())
            {
                result = md5.ComputeHash(buffer);
            }

            return result.ToHex(false);
        }

        public static byte[] GetMd5Bytes(this string input)
        {
            byte[] result;
            var buffer = Encoding.UTF8.GetBytes(input);

            using (var md5 = new MD5CryptoServiceProvider())
            {
                result = md5.ComputeHash(buffer);
            }

            return result;
        }

        public static string ToHex(this byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }


        public static dynamic DapperRowToExpando(this object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return (ExpandoObject)expando;
        }

        public static string ReadResourceFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.{path}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        public static List<ExpandoObject> GetSchemaAndData(this IDataReader reader, out List<string> schema)
        {
            schema = reader.GetSchemaTable()?.Rows
                                     .Cast<DataRow>()
                                     .Select(r => (string)r["ColumnName"])
                                     .ToList();

            var results = new List<ExpandoObject>();


            while (reader.Read())
            {
                var obj = new ExpandoObject();
                if (schema != null)
                    foreach (var col in schema)
                    {
                        var row = ((IDictionary<string, object>)obj);

                        if (string.IsNullOrEmpty(col) || row.ContainsKey(col)) continue;

                        row.Add(col, reader[col]);
                    }
                results.Add(obj);
            }

            return results;
        }

        public static List<ExpandoObject> GetSchemaAndData(this IDataReader reader)
        {
            List<string> schema;
            return reader.GetSchemaAndData(out schema);
        }

        public static DataTable ToDataTable(this IDataReader reader)
        {
            var dt = new DataTable();
            dt.Load(reader);

            return dt;
        }

        public static string GetPersianDate(this DateTime date)
        {
            var jc = new PersianCalendar();
            return
                $"{jc.GetYear(date):0000}/{jc.GetMonth(date):00}/{jc.GetDayOfMonth(date):00} {jc.GetHour(date):00}:{jc.GetMinute(date):00}:{jc.GetSecond(date):00}.{jc.GetMilliseconds(date)}";
        }


        /// <summary>
        /// since expiry time has now become part of your claims, you now can get it back easily
        /// this example just returns the remaining time in total seconds, as a string value
        /// assuming this method is part of your controller methods        
        /// </summary>
        /// <returns>Get User Identity Expire DateTime</returns>
        public static string ExpireRemainingTime(this System.Security.Claims.ClaimsIdentity identity)
        {
            //var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var claimType = Properties.Settings.Default.LoginExpireClaimType;

            if (identity != null && identity.HasClaim(c => c.Type == claimType))
            {
                var expireOn = identity.FindFirstValue(claimType);

                long current = DateTime.Now.Ticks;
                long? expire = long.Parse(expireOn);

                long elapsedTicks = (expire ?? 0) - current;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

                var remaining = (int)elapsedSpan.TotalMilliseconds;

                return remaining.ToString();
            }
            return string.Empty;
        }

        public static void SetCookie(this HttpResponse response, string name, string value, DateTime expire)
        {
            // add expire time duration to milisecond in cookie
            HttpCookie cookie = new HttpCookie(name, value);
            //cookie.Expires = expire;
            response.SetCookie(cookie); //SetCookie is used for update the cookies.
        }

        public static string AppSetting(string name)
        {
            return Properties.Settings.Default.Properties[name]?.DefaultValue?.ToString();
        }

        public static DataTable ToDataTable(this object[] data)
        {
            var dt = new DataTable();

            if (!data.Any()) return dt;

            // Get header
            var dapperRowProperties = (data[0] as IDictionary<string, object>).Keys;
            foreach (string col in dapperRowProperties)
            {
                dt.Columns.Add(col);
            }
            //
            // Get details
            foreach (IDictionary<string, object> item in data)
            {
                var row = dt.Rows.Add();
                foreach (string prop in dapperRowProperties)
                {
                    row[prop] = item[prop].ToString();
                }
            }

            return dt;
        }

        public static DataTable ToDataTable(this IEnumerable<dynamic> data)
        {
            return data.ToArray().ToDataTable();
        }

        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public static string RepairCipher(this string invalidString)
        {
            // some times get url convert '+' char to white space ' ' so rollback converting:
            var result = invalidString?.Replace(" ", "+");

            int mod4 = result.Length % 4;
            if (mod4 > 0)
            {
                result += new string('=', 4 - mod4);
            }

            return result;
        }


        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

    }
}
