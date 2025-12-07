using System.Security.Cryptography;
using System.Text;

namespace EduVision.Helpers
{
    public class PasswordHelper
    {
        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA256();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA256(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (computed.Length != storedHash.Length) return false;

            for (int i = 0; i < computed.Length; i++)
                if (computed[i] != storedHash[i]) return false;

            return true;
        }
    }
}
