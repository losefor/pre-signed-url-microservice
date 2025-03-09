using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class UrlSigner
    {
        private static readonly string SecretKey = "my-secret-key"; // Store securely!

        public static string GenerateSignature(string url, string expiry)
        {
            var dataToSign = $"{url}|{expiry}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            return Convert.ToBase64String(hash);
        }

        public static bool ValidateSignature(string url, string expiry, string providedSignature)
        {
            var expectedSignature = GenerateSignature(url, expiry);

            Console.WriteLine(providedSignature);
            Console.WriteLine(expectedSignature);

            return expectedSignature == providedSignature;
        }
    }
}
