using SpaceShooter.Modals;
using SpaceShooter.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace SpaceShooter.Globals
{
    internal class UserManager
    {
        // Check the last logged-in user by reading from a token file
        public static void CheckLastLoggedInUser()
        {
            try
            {
                string token = TokenManager.ReadTokenFromFile();
                GetUserByToken(token);
            }
            catch { }
        }

        // Retrieve user information from the database using a token
        public static void GetUserByToken(string userToken)
        {
            try
            {
                DatabaseConnection.Open();
                string query = "SELECT * FROM [User] WHERE [Token] = @UserToken";
                using (SqlCommand command = DatabaseConnection.CreateCommand(query))
                {
                    command.Parameters.AddWithValue("@UserToken", userToken);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            // Create a User object from database fields
                            AppGlobals.CurrentUser = new User(
                                reader["ID"].ToString()!,
                                reader["Name"].ToString()!,
                                reader["Password"].ToString()!,
                                reader["Token"].ToString()!,
                                Convert.ToInt32(reader["Coins"]),
                                Convert.ToInt32(reader["HighestScore"]),
                                DateTime.Parse(reader["Dob"].ToString()!),
                                Convert.ToBoolean(reader["Admin"]),
                                reader["Inventory"].ToString()!,
                                reader["Background"].ToString()!,
                                reader["ProfilePicture"].ToString()!
                            );
                            // Update user interface with retrieved user information
                            UpdateUserInfo();
                        }
                    }
                }
            }
            catch (Exception ex) { AppGlobals.ErrorMessageBox($"Error in GetUserByToken: {ex.Message}\n{ex.StackTrace}"); }
            finally { DatabaseConnection.Close(); }
        }

        // Set token and save it to file
        public static void SetToken(string text)
        {
            string token = TokenManager.GenerateToken(text);
            TokenManager.SaveTokenToFile(token);
            GetUserByToken(token);
        }

        // Update user information displayed in leaderboards and other UI elements
        public static void UpdateUserInfo()
        {
            AssetManager.backgroundImage = Image.FromFile($"{AssetManager.Path}background/{AppGlobals.CurrentUser!.BackgroundImage}.png");
            AppGlobals.UpdateBackground();
            AppGlobals.leaderBoards!.profilePicture!.BackgroundImage = AppGlobals.CurrentUser.ProfilePicture;
            SetLabelText(AppGlobals.leaderBoards!.UserName!, AppGlobals.CurrentUser!.UserName!);
            SetLabelText(AppGlobals.leaderBoards!.Coins!, AppGlobals.CurrentUser!.Coins!.ToString());
            SetLabelText(AppGlobals.leaderBoards!.HighestScore!, AppGlobals.CurrentUser!.HighestScore!.ToString());
            SetLabelText(AppGlobals.leaderBoards!.Admin!, AppGlobals.CurrentUser!.Admin!.ToString());
        }

        // Helper method to set label text
        private static void SetLabelText(Label label, string text) => label.Text = text;

        // Handle sign out button click by deleting token file and returning to authentication
        public static void Btn_SignOut_Click(Form form)
        {
            TokenManager.DeleteTokenFile();
            AppGlobals.Authentication(form);
        }
    }
}
