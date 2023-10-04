using System;
using System.Security.Cryptography;
using System.Text;

namespace CarBookingTest.Utils
{
    public class HandlePassword
    {
        public string HashPassword(string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = Convert.ToBase64String(hashBytes);

                return hashedPassword;
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedInputPassword = Convert.ToBase64String(hashBytes);

                return string.Equals(hashedInputPassword, hashedPassword);
            }
        }
    }
}