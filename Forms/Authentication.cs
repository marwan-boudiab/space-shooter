using System.Data.SqlClient;
using SpaceShooter.Globals;
using SpaceShooter.Helpers.Design;
using SpaceShooter.Utilities;

namespace SpaceShooter
{
    public partial class Authentication : Form
    {
        // Controls
        public TextBox? Tb_Name, Tb_Password;
        private Button? signInBtn, signUpBtn;
        private DateTimePicker? dateTimePicker;
        private CheckBox? checkBox;
        private Label? dobLb, adminLb, Title;

        // State variables
        private bool signIn = true; // Flag to track if sign-in or sign-up mode
        private int startposx, startposy; 
        private readonly int spacey = 55, offset = 20;

        // Constructor
        public Authentication() { 
            InitializeComponent();
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetUpForm(); 
        }

        // Form setup method
        private void SetUpForm()
        {
            DesignHelpers.SetDesign(this); // Applies custom design
            ClientSize = new Size(558, 476); // Sets form size
            //Region = Region.FromHrgn(Design.CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
            Title = DesignHelpers.CreateLabel("SIGN IN", this.ClientSize.Width / 2 - 220 / 2, 34); // Creates title label
            Title.Font = new Font(Title.Font.FontFamily, 34); // Sets title font
            Controls.Add(Title); // Adds title label to form

            startposx = this.ClientSize.Width / 2 - 191; // X position for controls
            startposy = 140; // Y position for controls

            // Adds textboxes for name and password
            (Tb_Name, Tb_Password) = DesignHelpers.AddNameAndPasswordTextBoxes(this, startposx, startposy, spacey, offset);

            // Creates sign-in and sign-up buttons
            signInBtn = DesignHelpers.CreateButton(" SIGN IN ", startposx, startposy + (spacey * 2), true, Btn_SignIn_Click!);
            Controls.Add(signInBtn);
            signUpBtn = DesignHelpers.CreateButton(" SIGN UP ", startposx + signInBtn.Width + 140, startposy + (spacey * 2), false, Btn_SignUp_Click!);
            Controls.Add(signUpBtn);

            // Creates labels and controls for date of birth and admin checkbox
            dobLb = DesignHelpers.CreateLabel("DATE OF BIRTH", startposx, startposy + (spacey * 2));
            adminLb = DesignHelpers.CreateLabel("ADMIN", startposx, startposy + (spacey * 3));
            (dateTimePicker, checkBox) = DesignHelpers.AddDateTimePickerAndCheckBox(this, startposx, startposy + spacey * 2, spacey, offset, false);
        }

        // Sign-in button click event
        private void Btn_SignIn_Click(object sender, EventArgs e)
        {
            if (signIn) // If in sign-in mode
            {
                if (Tb_Name!.Text.Trim().Length == 0 || Tb_Password!.Text.Trim().Length == 0)
                    AppGlobals.ErrorMessageBox("Make sure to fill all fields."); // Error if fields are empty
                else
                {
                    if (CheckAccount(Tb_Name.Text, Tb_Password.Text)) // Checks if account exists
                    {
                        AppGlobals.MainMenu(this); // Opens main menu
                        UserManager.SetToken(Tb_Password.Text.ToUpper() + Tb_Name.Text.ToUpper()); // Sets authentication token
                    }
                    else AppGlobals.ErrorMessageBox("Invalid username or password. Please try again."); // Error for invalid credentials
                }
            }
            else // Switches to sign-in mode
            {
                Title!.Text = "SIGN IN";
                signIn = true;
                this.Invalidate();
                UpdateButtons(signIn);
            }
        }

        // Method to check if account exists in the database
        public bool CheckAccount(string username, string password)
        {
            bool accountExists = false;
            try
            {
                DatabaseConnection.Open(); // Opens database connection
                

                SqlCommand cmd = DatabaseConnection.CreateCommand($"SELECT COUNT(*) FROM [User] WHERE Name = '{username}' AND Password = '{password}'");
                int count = (int)cmd.ExecuteScalar()!;
                accountExists = (count > 0); // Checks if account exists
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            finally { DatabaseConnection.Close(); } // Closes database connection
            return accountExists;
        }

        // Sign-up button click event
        private void Btn_SignUp_Click(object sender, EventArgs e)
        {
            if (!signIn) // If in sign-up mode
            {
                if (Tb_Name!.Text.Trim().Length == 0 || Tb_Password!.Text.Trim().Length == 0)
                    AppGlobals.ErrorMessageBox("Make sure to fill all fields."); // Error if fields are empty
                else
                {
                    DatabaseConnection.Open(); // Opens database connection
                    SqlCommand checkCmd = DatabaseConnection.CreateCommand("SELECT COUNT(*) FROM [User] WHERE [Name] = @1"); // Checks if username exists
                    checkCmd.Parameters.AddWithValue("@1", Tb_Name.Text);

                    int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (userCount > 0) AppGlobals.ErrorMessageBox("User with the same name already exists. Choose a different name."); // Error if username already exists
                    else
                    {
                        // Inserts new user into database
                        SqlCommand cmd = DatabaseConnection.CreateCommand("INSERT INTO [User] ([Name], [Password], [Token], [Coins], [HighestScore], [Dob], [Admin], [Inventory]) VALUES (@1, @2, @3, @4, @5, @6, @7, @8)");
                        cmd.Parameters.AddWithValue("@1", Tb_Name.Text);
                        cmd.Parameters.AddWithValue("@2", Tb_Password.Text);
                        cmd.Parameters.AddWithValue("@3", TokenManager.GenerateToken(Tb_Password.Text.ToUpper() + Tb_Name.Text.ToUpper()));
                        cmd.Parameters.AddWithValue("@4", 0);
                        cmd.Parameters.AddWithValue("@5", 0);
                        cmd.Parameters.AddWithValue("@6", dateTimePicker!.Value.ToShortDateString());
                        cmd.Parameters.AddWithValue("@7", checkBox!.Checked ? true : false);
                        cmd.Parameters.AddWithValue("@8", "[\"BlueSpace\"]");
                        cmd.ExecuteNonQuery(); // Executes the SQL command

                        AppGlobals.SuccessMessageBox("Account created successfully!"); // Success message
                    }
                    DatabaseConnection.Close(); // Closes database connection
                }
            }
            else // Switches to sign-up mode
            {
                Title!.Text = "SIGN UP";
                signIn = false;
                this.Invalidate();
                UpdateButtons(signIn);
            }
        }

        // Updates UI based on sign-in or sign-up mode
        private void UpdateButtons(bool signIn)
        {
            if (!signIn) {
                // Shows date of birth and admin controls
                Controls.Add(dobLb); Controls.Add(dateTimePicker); 
                Controls.Add(adminLb); Controls.Add(checkBox); 
            }
            else {
                // Hides date of birth and admin controls
                Controls.Remove(dobLb); 
                Controls.Remove(dateTimePicker); 
                Controls.Remove(adminLb); 
                Controls.Remove(checkBox); 
            }

            // Adjusts sign-in and sign-up button positions and colors based on mode
            signInBtn!.Location = new Point(signIn ? startposx : startposx + signInBtn.Width + 140, signIn ? startposy + (spacey * 2) : startposy + (spacey * 4));
            signInBtn.BackColor = signIn ? DesignHelpers.PrimaryColor : DesignHelpers.SecondaryColor;
            signUpBtn!.Location = new Point(signIn ? startposx + signInBtn.Width + 140 : startposx, signIn ? startposy + (spacey * 2) : startposy + (spacey * 4));
            signUpBtn.BackColor = signIn ? DesignHelpers.SecondaryColor : DesignHelpers.PrimaryColor;
        }
    }
}
