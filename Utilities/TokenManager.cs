using System.Security.Cryptography;
using System.Text;

namespace SpaceShooter.Utilities
{
    internal class TokenManager
    {
        // Method to generate SHA256 token from input string
        public static string GenerateToken(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using (SHA256? sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                StringBuilder? stringBuilder = new();
                for (int i = 0; i < hashBytes.Length; i++)
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                return stringBuilder.ToString();
            }
        }

        // Save token to file
        public static void SaveTokenToFile(string token)
        {
            string filePath = GetTokenFilePath();
            File.WriteAllText(filePath, token);
        }


        // Helper method to get the path of the token file
        public static string GetTokenFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "SpaceShooter");
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
            return Path.Combine(appFolder, "token.txt");
        }

        
        public static string ReadTokenFromFile()
        {
            string tokenFilePath = GetTokenFilePath();
            if (File.Exists(tokenFilePath)) return File.ReadAllText(tokenFilePath);
            return string.Empty;
        }

        //delete token file
        public static void DeleteTokenFile()
        {
            string tokenFilePath = GetTokenFilePath();
            if (File.Exists(tokenFilePath)) File.Delete(tokenFilePath);
        }
    }
}
