using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace EnTier.Extensions
{
    internal static class StringExtensions
    {
        
        public static string Plural(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Nulls";
            }
            var lastChar = name.ToLower().Substring(name.Length - 1, 1);

            var eses = new List<string>{ "s","c","x"};
            
            if(eses.Contains(lastChar))
            {
                return name + "es";
            }

            if (lastChar == "y")
            {
                return name.Substring(0, name.Length - 1)+"ies";
            }

            return name + "s";
        }
        
        public static string ComputeMd5(this string s)
        {
            StringBuilder sb = new StringBuilder();
 
            // Initialize a MD5 hash object
            using (MD5 md5 = MD5.Create())
            {
                // Compute the hash of the given string
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
 
                // Convert the byte array to string format
                foreach (byte b in hashValue) {
                    sb.Append($"{b:X2}");
                }
            }
 
            return sb.ToString();
        }
    }
}