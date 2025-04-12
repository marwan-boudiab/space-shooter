using SpaceShooter.Helpers.Design;

namespace SpaceShooter
{
    public partial class Credits : Form
    {
        private Label? ArtistCredits; // Nullable Label to display artist credits

        // Constructor for the Credits class
        public Credits() 
        { 
            InitializeComponent(); // Initializes the form components
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetUpMenu(); // Sets up the menu for the credits form
        }

        // Method to set up the menu for the credits form
        private void SetUpMenu()
        {
            DesignHelpers.SetDesign(this); // Applies a design to the form using a custom Design class

            // Creates and configures a title label
            Label Title =  DesignHelpers.CreateLabel("MARWAN BOU DIAB", this.ClientSize.Width / 2 - 1080 / 2, 34);
            Title.Font = new Font(Title.Font.FontFamily, 34); // Sets the font size of the title
            Controls.Add(Title); // Adds the title label to the form's controls

            // Creates and configures a label for artist credits
            ArtistCredits = DesignHelpers.CreateLabel("" +
                "Game assets\n" +
                "Pack Name:  Superpowers Space Shooter Asset Pack\n" +
                "Artist:  Pixel boy\n" +
                "Attribution:  https://www.patreon.com/SparklinLabs?ty=h\n\n", 56, 144); 
            Controls.Add(ArtistCredits); // Adds the artist credits label to the form's controls
        }
    }
}
