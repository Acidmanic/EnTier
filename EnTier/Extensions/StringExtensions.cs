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

            var eses = new List<string> { "s", "c", "x" };

            if (eses.Contains(lastChar))
            {
                return name + "es";
            }

            if (lastChar == "y")
            {
                return name.Substring(0, name.Length - 1) + "ies";
            }

            return name + "s";
        }
    }
}