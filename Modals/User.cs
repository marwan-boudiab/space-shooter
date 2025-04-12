using Newtonsoft.Json;
using SpaceShooter.Utilities;

namespace SpaceShooter.Modals
{
    // User class representing a player in the game
    public class User
    {
        // Properties representing user attributes
        public string ID { get; set; }               // Unique identifier for the user
        public string UserName { get; set; }         // Username of the user
        public string Password { get; set; }         // Password of the user (for demo purposes; should not store plain text passwords in real scenarios)
        public string Token { get; set; }            // Token for authentication purposes
        public int Coins { get; set; }               // Number of coins owned by the user
        public int HighestScore { get; set; }        // Highest score achieved by the user
        public DateTime DOB { get; set; }            // Date of birth of the user
        public bool Admin { get; set; }              // Indicates if the user is an administrator
        public List<string> Inventory { get; set; }  // List of items in the user's inventory
        public string BackgroundImage { get; set; }  // Background image chosen by the user
        public Image ProfilePicture { get; set; }    // Profile picture of the user

        public User() { }   

        // Constructor to initialize user properties
        public User(string id, string name, string password, string token, int coins, int highestScore, DateTime dob, bool admin, string inventory, string backgroundImage, string profilePicture)
        {
            // Initialize properties with provided values
            ID = id;
            UserName = name;
            Password = password; // Note: Password should not be stored in plain text in a real application
            Token = token;
            Coins = coins;
            HighestScore = highestScore;
            DOB = dob;
            Admin = admin;

            // Deserialize inventory JSON string into List<string> or initialize empty list if null
            Inventory = JsonConvert.DeserializeObject<List<string>>(inventory) ?? new List<string>();

            // Set background image, defaulting to "BlueSpace" if backgroundImage is null or empty
            BackgroundImage = string.IsNullOrEmpty(backgroundImage) ? "BlueSpace" : backgroundImage;

            // Set profile picture using the provided file path, or fallback to a default avatar image
            ProfilePicture = GetProfilePicture(profilePicture); ;
        }

        // Method to get profile picture based on file path
        private static Image GetProfilePicture(string profilePicture)
        {
            if (string.IsNullOrEmpty(profilePicture))
            {
                // Return default avatar image if profilePicture is null or empty
                return AssetManager.AvatarImage;
            }

            try
            {
                // Attempt to load profile picture from file path
                return Image.FromFile(profilePicture);
            }
            catch (Exception)
            {
                // Return default avatar image if loading fails
                return AssetManager.AvatarImage;
            }
        }
    }
}
