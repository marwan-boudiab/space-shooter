using SpaceShooter.Forms;
using SpaceShooter.Modals;
using SpaceShooter.Utilities;

namespace SpaceShooter.Globals
{
    internal static class AppGlobals
    {
        // Current user instance
        public static User? CurrentUser;

        // Forms and components initialization
        private static readonly Authentication authentication = new();
        private static readonly MainMenu mainMenu = new();
        public static readonly Game game = new();
        public static ManageAccounts? manageAccounts = new();
        private static readonly Store? store = new();
        public static LeaderBoards? leaderBoards = new();
        public static Credits? credits = new();
        private static readonly List<Form> forms = [mainMenu, credits, game, manageAccounts, leaderBoards];

        static AppGlobals()
        {
            try
            { UserManager.CheckLastLoggedInUser(); }
            catch (Exception ex)
            {
                ErrorMessageBox($"MessageBox.Show($\"Initialization error in AppGlobals: {{ex.Message}}\\n{{ex.StackTrace}}\"");
                throw;
            }
        }

        // Navigation methods between forms
        public static void Authentication(Form form) { form.Hide(); authentication.Show(); }
        public static void MainMenu(Form form) { form.Hide(); mainMenu.Show(); }
        public static void Credits(Form form) { form.Hide(); credits!.Show(); }
        public static void Game(Form form) { form.Hide(); game.Show(); game.SetUpGameWorld(); }
        public static void Store(Form form) { form.Hide(); store!.RefreshItems(); store!.Show(); }
        public static void LeaderBoards(Form form) { form.Hide(); leaderBoards!.RefreshGrid(); leaderBoards!.Show(); }
        public static void ManageAccount(Form form) { manageAccounts?.RefreshGrid(); form.Hide(); manageAccounts!.Show(); }

        // Refresh data in all forms
        public static void RefreshForms()
        { store!.RefreshItems(); leaderBoards!.RefreshGrid(); manageAccounts?.RefreshGrid(); }

        // Helper methods for displaying message boxes
        public static void SuccessMessageBox(string text) => new CustomMessageBoxForm("Success", text).ShowDialog();
        public static void ErrorMessageBox(string text) => new CustomMessageBoxForm("Error", text).ShowDialog();

        // Update background image for all forms
        public static void UpdateBackground()
        { foreach (Form form in forms) form.BackgroundImage = AssetManager.backgroundImage; }
    }
}
