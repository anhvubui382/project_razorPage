using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace WebRazor.Helpers
{
    public class Password_encryption
    {
        // Method to hash a password using MD5
        public static string HashPassWord(string passRaw)
        {
            // Create an instance of the MD5 cryptographic hash provider
            MD5 md5 = new MD5CryptoServiceProvider();

            // Compute the hash from the bytes of the input text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(passRaw));

            // Get the hash result after computation
            byte[] result = md5.Hash;

            // Create a StringBuilder to store the hexadecimal representation of the hash
            StringBuilder strBuilder = new StringBuilder(); 

            // Convert each byte of the hash into 2 hexadecimal digits and append to the StringBuilder
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            // Return the hexadecimal representation of the hash
            return strBuilder.ToString();
        }

        // Method to generate a random password of a specified length
        public static string GeneratePassword(int length)
        {
            // Create a Random instance for generating random characters
            Random random = new Random();

            // Define the characters that can be used in the password
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Generate a password of the specified length by randomly selecting characters from the defined set
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
