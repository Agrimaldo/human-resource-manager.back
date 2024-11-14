
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HumanResourceManager.Domain.Util
{
    public class Validator
    {
        private static readonly Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) { return false; }
            return emailRegex.IsMatch(email);
        }
    }

    public class Encrypter
    {
        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input); 
                byte[] hashBytes = md5.ComputeHash(inputBytes); // Convert the byte array to a hexadecimal string
                StringBuilder sb = new StringBuilder(); 
                for (int i = 0; i < hashBytes.Length; i++) 
                { sb.Append(hashBytes[i].ToString("X2")); } 

                return sb.ToString(); 
            }
        }
    }
}
