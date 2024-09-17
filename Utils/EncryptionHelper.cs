using System.Security.Cryptography;
using System.Text;


namespace StarterKit.Utils
{
    public static class EncryptionHelper
    {
        public static string EncryptPassword(string password)
        {
            SHA256 mySha565 = SHA256.Create();
            return Encoding.Default.GetString(mySha565.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to bytes using UTF8 encoding
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash to a Base64 string
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}