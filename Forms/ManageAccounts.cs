using System.Data;
using System.Data.SqlClient;
using SpaceShooter.Forms;
using SpaceShooter.Globals;
using SpaceShooter.Helpers.Design;
using SpaceShooter.Utilities;

namespace SpaceShooter
{
    public partial class ManageAccounts : Form
    {
        private string? ID; // Private field to store the ID of the selected user
        public TextBox? Tb_Name, Tb_Password, Tb_Coins; // Public fields for textboxes used in account management
        public DateTimePicker? DateTimePicker; // Public field for date picker used in account management
        public CheckBox? CheckBox; // Public field for checkbox used in account management
        private DataGridView? dataGridView; // Private field for the DataGridView used to display user accounts

        public ManageAccounts()
        {
            InitializeComponent(); // Initializes the form components
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetUpForm(); // Sets up the UI for managing accounts
        }

        private void Clear()
        {
            ID = null;
            Tb_Name!.Text = Tb_Password!.Text = ""; // Clears textboxes
            DateTimePicker!.ResetText(); // Resets DateTimePicker
            Tb_Coins!.ResetText(); // Resets Coins textbox
            CheckBox!.Checked = false; // Clears checkbox
        }

        private void SetUpForm()
        {
            DesignHelpers.SetDesign(this); // Sets the design of the form using a custom Design class
            
            // Creates a DataGridView for displaying user accounts
            dataGridView = DesignHelpers.CreateCustomDataGridView(34, 56, 626, 370, dataGridView_CellClick);
            Controls.Add(dataGridView); // Adds DataGridView to form

            int startposx = dataGridView.Right + 34, startposy = ClientSize.Height / 2 - 140, spacey = 44, offset = 20;

            // Creates textboxes, date picker, and checkbox for account management
            (Tb_Name, Tb_Password, DateTimePicker, CheckBox, Tb_Coins) = DesignHelpers.AccountForm(this, startposx, startposy, spacey, offset);

            // Creates Update and Delete buttons
            Button updateBtn = DesignHelpers.CreateButton(" UPDATE ", startposx, startposy + (spacey * 4), true, Btn_Update_Click!);
            Controls.Add(updateBtn);
            Controls.Add(DesignHelpers.CreateButton(" DELETE ", startposx + updateBtn.Width + 146, startposy + (spacey * 4), false, Btn_Delete_Click!));

            RefreshGrid(); // Loads data into the DataGridView
        }

        // Method to refresh the DataGridView with user account data
        public void RefreshGrid()
        {
            DataTable dt = new(); // Creates a new DataTable
            string querySelect = "select [ID], [Name], [Password], [Coins], [Dob], [Admin] from [User]"; // SQL query to select user data
            SqlDataAdapter adp = DatabaseConnection.CreateDataAdapter(querySelect); // SqlDataAdapter to fetch data
            adp.Fill(dt); // Fills DataTable with data from the database
            dataGridView!.DataSource = dt; // Sets DataGridView's data source to the DataTable
            dataGridView.Columns["ID"].Visible = false; // Hides the ID column from the user interface

            // Adjusts the height of the DataGridView based on its contents
            int totalRowHeight = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            dataGridView.Height = totalRowHeight + dataGridView.ColumnHeadersHeight;
            dataGridView.Height = dataGridView.Height < 671 ? dataGridView.Height : 671;
            /*int totalColumnWidth = dataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
            dataGridView.Width = totalColumnWidth + dataGridView.RowHeadersWidth -50;*/
        }

        // Event handler for clicking on a cell in the DataGridView
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /*try
            {
                ID = dataGridView!.Rows[e.RowIndex].Cells[0].Value.ToString(); // Retrieves the ID of the selected user
                Tb_Name!.Text = dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString(); // Retrieves and sets Name textbox
                Tb_Password!.Text = dataGridView.Rows[e.RowIndex].Cells[2].Value.ToString(); // Retrieves and sets Password textbox
                Tb_Coins!.Text = dataGridView.Rows[e.RowIndex].Cells[3].Value.ToString(); // Retrieves and sets Coins textbox
                DateTimePicker!.Value = (DateTime)dataGridView.Rows[e.RowIndex].Cells[4].Value; // Retrieves and sets Dob (date) picker
                CheckBox!.Checked = (bool?)dataGridView.Rows[e.RowIndex].Cells[5].Value ?? false; // Retrieves and sets Admin checkbox
            }
            catch (Exception ex)
            {
                AppGlobals.ErrorMessageBox("Error retrieving data: " + ex.Message); // Shows error message if data retrieval fails
            }*/

            try
            {
                // Check if the clicked cell is a valid data cell
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView!.Rows.Count)
                {
                    ID = dataGridView!.Rows[e.RowIndex].Cells["ID"].Value?.ToString(); // Retrieves the ID of the selected user
                    Tb_Name!.Text = dataGridView.Rows[e.RowIndex].Cells["Name"].Value?.ToString() ?? ""; // Retrieves and sets Name textbox
                    Tb_Password!.Text = dataGridView.Rows[e.RowIndex].Cells["Password"].Value?.ToString() ?? ""; // Retrieves and sets Password textbox
                    Tb_Coins!.Text = dataGridView.Rows[e.RowIndex].Cells["Coins"].Value?.ToString() ?? ""; // Retrieves and sets Coins textbox

                    // Safely parse and set the DateTimePicker value
                    if (DateTime.TryParse(dataGridView.Rows[e.RowIndex].Cells["Dob"].Value?.ToString(), out DateTime dob))
                    {
                        DateTimePicker!.Value = dob; // Retrieves and sets Dob (date) picker
                    }
                    else
                    {
                        DateTimePicker!.ResetText(); // Reset if the date is invalid
                    }

                    // Safely set the checkbox value
                    CheckBox!.Checked = dataGridView.Rows[e.RowIndex].Cells["Admin"].Value is bool isAdmin && isAdmin;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data: " + ex.Message); // Shows error message if data retrieval fails
            }
        }

        // Event handler for Update button click
        private void Btn_Update_Click(object sender, EventArgs e)
        {
            // Validates input fields
            if (Tb_Name!.Text.Trim().Length == 0 || Tb_Password!.Text.Trim().Length == 0 || Tb_Coins!.Text.Trim().Length == 0)
            { 
                AppGlobals.ErrorMessageBox("Make sure to fill all fields.");  // Shows error if any field is empty
                return; 
            }

            DatabaseConnection.Open(); // Opens database connection if closed
            string query = "SELECT ID FROM [USER] WHERE Name = @Name"; // SQL query to check if user exists
            using (SqlCommand selectCommand = DatabaseConnection.CreateCommand(query))
            {
                selectCommand.Parameters.AddWithValue("@Name", Tb_Name!.Text); // Adds parameter for user Name
                object result = selectCommand.ExecuteScalar()!; // Executes query and retrieves result

                // Checks if user exists or if updating existing user
                if (result != null || ID != null)
                {
                    ID = result?.ToString() ?? ID; // Sets ID to retrieved or existing ID

                    // Updates user information in the database
                    using (SqlCommand updateCommand = DatabaseConnection.CreateCommand("UPDATE [User] SET [Name] = @1, [Password] = @2, [Token] = @3, [Coins] = @4, [Dob] = @5, [Admin] = @6 WHERE ID=@7"))
                    {
                        string tokenString = Tb_Password?.Text.ToUpper() + Tb_Name?.Text.ToUpper(); // Generates token string
                        updateCommand.Parameters.AddWithValue("@1", Tb_Name!.Text); // Adds parameter for Name
                        updateCommand.Parameters.AddWithValue("@2", Tb_Password!.Text); // Adds parameter for Password
                        updateCommand.Parameters.AddWithValue("@3", TokenManager.GenerateToken(tokenString)); // Adds parameter for Token
                        updateCommand.Parameters.AddWithValue("@4", int.Parse(Tb_Coins?.Text ?? "0")); // Adds parameter for Coins
                        updateCommand.Parameters.AddWithValue("@5", DateTimePicker?.Value.ToShortDateString()); // Adds parameter for Dob
                        updateCommand.Parameters.AddWithValue("@6", CheckBox?.Checked ?? false); // Adds parameter for Admin
                        updateCommand.Parameters.AddWithValue("@7", ID); // Adds parameter for ID

                        try
                        {
                            updateCommand.ExecuteNonQuery(); // Executes the update query
                            if (ID == AppGlobals.CurrentUser?.ID) UserManager.SetToken(tokenString); // Updates token if current user
                            RefreshGrid(); // Refreshes the DataGridView
                            AppGlobals.SuccessMessageBox("User information updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            AppGlobals.ErrorMessageBox("Error updating user information: " + ex.Message);
                        }
                    }
                }
                else AppGlobals.ErrorMessageBox("User not found. Make sure to enter a valid user."); // Shows error if user not found
            }
            DatabaseConnection.Close(); // Closes database connection if open
            Clear(); // Clears input fields after update
        }

        // Event handler for Delete button click
        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            DatabaseConnection.Open(); // Opens database connection if closed
            string query = "SELECT ID FROM [USER] WHERE Name = @Name"; // SQL query to check if user exists
            using (SqlCommand selectCommand = DatabaseConnection.CreateCommand(query))
            {
                selectCommand.Parameters.AddWithValue("@Name", Tb_Name!.Text); // Adds parameter for user Name
                object result = selectCommand.ExecuteScalar()!; // Executes query and retrieves result

                // Checks if user exists or if deleting existing user
                if (result != null || ID != null)
                {
                    ID = result!.ToString(); // Sets ID to retrieved ID
                    using (SqlCommand cmd = DatabaseConnection.CreateCommand("DELETE from [User] where ID=@1"))
                    {
                        cmd.Parameters.AddWithValue("@1", ID); // Adds parameter for user ID
                        bool currentUser = ID == AppGlobals.CurrentUser!.ID; // Checks if deleting current user
                        string prompt = currentUser ? "your" : "this"; // Sets prompt message based on user deletion

                        // Shows confirmation dialog for account deletion
                        DialogResult ans = CustomMessageBoxForm.Show("Confirmation", $"Are you sure you want to delete {prompt} account?");
                        if (ans == DialogResult.Yes)
                        {
                            cmd.ExecuteNonQuery(); // Executes delete query
                            RefreshGrid(); // Refreshes the DataGridView
                            Clear(); // Clears input fields after deletion
                            AppGlobals.SuccessMessageBox("Account removed successfully!" + (currentUser ? " You'll be redirected to the Sign In Screen" : "")); // Shows success message
                            DatabaseConnection.Close();
                            if (currentUser) AppGlobals.Authentication(this); // Redirects to sign-in screen if deleting current user
                        }
                    };
                }
                else AppGlobals.ErrorMessageBox("Record not found. Make sure to select a valid record."); // Shows error if user not found
            }
        }
    }
}
