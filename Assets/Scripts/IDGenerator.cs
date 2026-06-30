using System;
using System.Security.Cryptography;

namespace Habillage
{
    public class IDGenerator
    {
        private static readonly RNGCryptoServiceProvider Random = new();
    
        public static string GenerateUniqueID(int length = 5)
        {
            // We chose an encoding that fits 6 bits into every character,
            // so we can fit length*6 bits in total.
            // Each byte is 8 bits, so...
            int sufficientBufferSizeInBytes = (length * 6 + 7) / 8;

            var buffer = new byte[sufficientBufferSizeInBytes];
            Random.GetBytes(buffer);
            return Convert.ToBase64String(buffer).Substring(0, length);
        }
    }
}