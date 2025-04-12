using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using SpaceShooter.Globals;
using SpaceShooter.Helpers.Design;
using SpaceShooter.Utilities;

namespace SpaceShooter
{
    public partial class LeaderBoards : Form
    {
        // Fields for UI elements
        private DataGridView? board;        // DataGridView for displaying leaderboards
        private TextBox? Tb_Score;          // TextBox for entering score filter
        public ComboBox? Cbx_Background;    // ComboBox for selecting background
        public PictureBox? profilePicture;  // PictureBox for user profile picture

        // Labels for displaying user information
        public Label? UserName, Coins, HighestScore, Admin;

        // Constructor initializes the form and sets up the menu
        public LeaderBoards() { 
            InitializeComponent();
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetUpMenu(); 
        }

        // Sets up the UI elements and menu
        private void SetUpMenu()
        {
            
            // Initialize and set design for the form
            DesignHelpers.SetDesign(this);

            // Create and configure profile picture PictureBox
            profilePicture = new PictureBox()
            {
                BackColor = Color.Transparent,
                BackgroundImageLayout = ImageLayout.Zoom,
                Location = new Point(12, 12),
                Size = new Size(198, 191),
                Cursor = Cursors.Hand
            };

            // Configures profile picture settings and event handler
            int AccountStartPosx = 620 + 40, AccountStartPosy = 280, AccountSpacey = 38, AccountSpacex = 140;
            profilePicture.Left = AccountStartPosx + profilePicture.Width/2 - 40;
            profilePicture.Top = AccountStartPosy - (int)(profilePicture.Height * 1.3);
            profilePicture.Click += ProfilePicture_Click!;
            Controls.Add(profilePicture);

            // Create and configure labels for user information
            Label? NameLb, CoinsLb, HighestScoreLb, AdminLb;
            NameLb = DesignHelpers.CreateLabel("NAME", AccountStartPosx, AccountStartPosy); Controls.Add(NameLb);
            UserName = DesignHelpers.CreateLabel("", NameLb!.Right + AccountSpacex, AccountStartPosy); Controls.Add(UserName);
            CoinsLb = DesignHelpers.CreateLabel("COINS", AccountStartPosx, AccountStartPosy + AccountSpacey); Controls.Add(CoinsLb);
            Coins = DesignHelpers.CreateLabel("", CoinsLb!.Right + AccountSpacex, AccountStartPosy + AccountSpacey); Controls.Add(Coins);
            HighestScoreLb = DesignHelpers.CreateLabel("HIGHEST SCORE", AccountStartPosx, AccountStartPosy + AccountSpacey * 2); Controls.Add(HighestScoreLb);
            HighestScore = DesignHelpers.CreateLabel("", HighestScoreLb!.Right + AccountSpacex - 90, AccountStartPosy + AccountSpacey * 2); Controls.Add(HighestScore);
            AdminLb = DesignHelpers.CreateLabel("ADMIN", AccountStartPosx, AccountStartPosy + AccountSpacey * 3); Controls.Add(AdminLb);
            Admin = DesignHelpers.CreateLabel("", AdminLb!.Right + AccountSpacex, AccountStartPosy + AccountSpacey * 3); Controls.Add(Admin);

            // Create ComboBox for selecting background
            Label setBackground = DesignHelpers.CreateLabel("SET BACKGROUND", AccountStartPosx, AdminLb.Bottom + 45); Controls.Add(setBackground);
            Cbx_Background = new ComboBox()
            {
                Font = new Font(DesignHelpers.FontFamily, 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = DesignHelpers.PrimaryColor,
                Location = new Point(setBackground.Right + AccountSpacex - 108, setBackground.Location.Y - 5),
                Size = new Size(200, 26),
            }; Controls.Add(Cbx_Background);
            Cbx_Background.MouseClick += Cbx_Background_MouseClick;
            Cbx_Background.SelectedIndexChanged += Cbx_Background_SelectedIndexChanged!;

            // Create sign out button
            Controls.Add(DesignHelpers.CreateButton("SIGN OUT", AccountStartPosx, setBackground.Bottom + AccountSpacey, true, Btn_SignOut_Click!));

            // Create DataGridView for displaying leaderboards
            board = DesignHelpers.CreateCustomDataGridView( 64, 56, 251, 370, (sender, e) => { }); Controls.Add(board);
            
            
            // Create score filter UI elements
            int LeaderBoardStartPosx = board.Right + 24;
            int LeaderBoardStartPosy = board.Top;
            Label sortLb = DesignHelpers.CreateLabel("FILTER BY", LeaderBoardStartPosx, LeaderBoardStartPosy); Controls.Add(sortLb);
            Label scoreLb = DesignHelpers.CreateLabel("SCORE", LeaderBoardStartPosx, sortLb.Bottom + 24); Controls.Add(scoreLb);
            Tb_Score = DesignHelpers.CreateTextBox("Tb_Coins", LeaderBoardStartPosx + scoreLb.Width + 10, sortLb.Bottom + 24, 120, 26);
            Controls.Add(Tb_Score);
            Button Btn_Update = DesignHelpers.CreateButton("UPDATE", LeaderBoardStartPosx, scoreLb.Bottom + 24, true, Score_OnChange!); Controls.Add(Btn_Update);
            //RefreshGrid();
        }

        // Event handler for clicking profile picture to select image
        private void ProfilePicture_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png";
            openFileDialog.FileName = "";
            string fileName;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                profilePicture!.BackgroundImage = Image.FromFile(fileName);
                profilePicture.SizeMode = PictureBoxSizeMode.Zoom;
            } else return;

            // Update profile picture in database
            DatabaseConnection.Open();
            using (SqlCommand cmd = DatabaseConnection.CreateCommand("UPDATE [User] SET [ProfilePicture] = @FileName WHERE ID = @UserID"))
            {
                cmd.Parameters.AddWithValue("@FileName", fileName);
                cmd.Parameters.AddWithValue("@UserID", AppGlobals.CurrentUser!.ID);
                cmd.ExecuteNonQuery();
            }
            DatabaseConnection.Close();
        }

        // Event handler for selecting background image
        private void Cbx_Background_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Cbx_Background!.SelectedItem != null)
            {
                // Set background image and update in database
                AssetManager.backgroundImage = Image.FromFile($"{AssetManager.Path}background/{Cbx_Background.SelectedItem}.png");
                AppGlobals.UpdateBackground();
                try
                {
                    DatabaseConnection.Open();
                    SqlCommand cmd = DatabaseConnection.CreateCommand($"update [User] set [Background] = @1 where ID=@2");
                    cmd.Parameters.AddWithValue("@1", Cbx_Background.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@2", AppGlobals.CurrentUser!.ID);
                    cmd.ExecuteNonQuery();
                }
                catch { AppGlobals.ErrorMessageBox("Make sure to select a record."); }
                finally { DatabaseConnection.Close(); }
                AppGlobals.CurrentUser!.BackgroundImage = Cbx_Background.SelectedItem!.ToString()!;
            }
        }

        // Event handler for mouse click on background ComboBox
        private void Cbx_Background_MouseClick(object? sender, MouseEventArgs e)
        {
            // Populate ComboBox with user's inventory items
            Cbx_Background!.Items.Clear(); Cbx_Background.Items.AddRange(AppGlobals.CurrentUser?.Inventory.ToArray()!); 
        }

        // Refreshes the leaderboard grid with user data
        public void RefreshGrid()
        {
            DataTable dt = new();
            string querySelect = "select [Name], [HighestScore] from [User]";
            SqlDataAdapter adp = DatabaseConnection.CreateDataAdapter(querySelect);
            adp.Fill(dt);
            board!.DataSource = dt;
            board.Sort(board.Columns["HighestScore"], ListSortDirection.Descending);
            int totalRowHeight = board.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            board.Height = totalRowHeight + board.ColumnHeadersHeight;
            board.Height = board.Height < 671 ? board.Height : 671;
            /*int totalColumnWidth = dataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
            dataGridView.Width = totalColumnWidth + dataGridView.RowHeadersWidth;*/
            Tb_Score!.Clear();
        }

        // Event handler for score filter update button click
        private void Score_OnChange(object sender, EventArgs e)
        {
            try
            {
                // Parse score filter value and update leaderboard grid
                int score = int.Parse(Tb_Score!.Text);
                DatabaseConnection.Open();
                string query = "Select [Name], [HighestScore] from [User] where [HighestScore] >= " + score;
                SqlDataAdapter adapter = DatabaseConnection.CreateDataAdapter(query);
                DataTable dt = new();
                adapter.Fill(dt);
                board!.DataSource = dt;
                // board.Sort(board.Columns["HighestScore"], Rb_Ascending.Checked ? ListSortDirection.Ascending : ListSortDirection.Descending);
                DatabaseConnection.Close();
            }
            catch { AppGlobals.ErrorMessageBox("Score can't be empty."); }
        }

        // Event handler for sign out button click
        private void Btn_SignOut_Click(object sender, EventArgs e)
        {
            // Perform sign out actions
            UserManager.Btn_SignOut_Click(this);
            AppGlobals.Authentication(this);
        }
    }
}
