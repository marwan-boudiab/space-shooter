using SpaceShooter.Globals;
using SpaceShooter.Helpers.Design;

namespace SpaceShooter
{
    public partial class MainMenu : Form
    {
        public MainMenu() 
        { 
            InitializeComponent(); // Initializes the form components
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetUpMenu(); // Sets up the main menu UI
        }
        
        private void SetUpMenu()
        {
            DesignHelpers.SetDesign(this); // Sets the design of the form using custom Design class

            int screenWidth = this.ClientSize.Width / 2; // Calculates center of the screen width
            int offsety = 140; // Vertical offset for button positioning

            // Title label for the game
            Label Title = DesignHelpers.CreateLabel("SPACE SHOOTER", this.ClientSize.Width / 2 - 364 / 2, 34);
            Title.Font = new Font(Title.Font.FontFamily, 34);
            Controls.Add(Title); // Adds title label to the form

            // Buttons for different menu options
            Controls.Add(DesignHelpers.CreateButton(" START ", screenWidth - 91 / 2, 12 + offsety, true, (sender, e) => AppGlobals.Game(this)));
            Controls.Add(DesignHelpers.CreateButton(" STORE ", screenWidth - 91 / 2, 72 + offsety, true, (sender, e) => AppGlobals.Store(this)));
            Controls.Add(DesignHelpers.CreateButton(" CREDITS ", screenWidth - 108 / 2, 132 + offsety, true, (sender, e) => AppGlobals.Credits(this)));
            Controls.Add(DesignHelpers.CreateButton(" LEADERBOARDS ", screenWidth - 190 / 2, 192 + offsety, true, (sender, e) => AppGlobals.LeaderBoards(this)));
            Controls.Add(DesignHelpers.CreateButton(" MANAGE ACCOUNTS ", screenWidth - 245 / 2, 252 + offsety, true, ManageAccountsClick!));
            Controls.Add(DesignHelpers.CreateButton(" EXIT ", screenWidth - 75 / 2, 312 + offsety, true, (sender, e) => Application.Exit()));
        }

        // Event handler for managing accounts button click
        private void ManageAccountsClick(object sender, EventArgs e)
        {
            // Checks if the current user is an admin and opens manage accounts if true
            if (AppGlobals.CurrentUser!.Admin) 
                AppGlobals.ManageAccount(this);
            else
                AppGlobals.ErrorMessageBox("Only admins can manage accounts."); // Shows error message if user is not admin
        }
    }
}
