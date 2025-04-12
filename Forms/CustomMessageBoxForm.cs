using SpaceShooter.Helpers.Design;

namespace SpaceShooter.Forms
{
    public partial class CustomMessageBoxForm : Form
    {
        // Constructor for initializing the message box with status and message
        public CustomMessageBoxForm(string status, string message)
        {
            InitializeComponent();
            Program.EnableDoubleBuffering(this);
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetupForm(status, message); // Sets up the form based on status and message
        }

        // Method to set up the form based on status and message
        private void SetupForm(string status, string message)
        {
            DesignHelpers.SetupBorder(this, 16);  // Applies custom border design
            ClientSize = new Size(420, 180); // Sets client size of the form
            BackColor = status == "Error" ? DesignHelpers.ErrorColor : Color.LightSeaGreen; // Sets background color based on status

            // Apply rounded corners to the form (currently commented out)
            Region = Region.FromHrgn(DesignHelpers.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, 16, 16));

            // Creates and adds a label for the status (error, confirmation, etc.)
            Label Title = DesignHelpers.CreateLabel(status, 20, 30);
            Controls.Add(Title);

            // Creates and adds a label for the message with specific width and height
            Label Message = DesignHelpers.CreateLabel(message, 20, 60);
            Message.AutoSize = false; // Disable automatic resizing
            Message.Width = 400;
            Message.Height = 50;
            Controls.Add(Message);

            // Adds buttons based on status
            if (status == "Confirmation")
            {
                // Adds "Yes" button with specific click event handler
                Button yesButton = DesignHelpers.CreateButton("Yes", 240, 136, false, YesButton_Click);
                Controls.Add(yesButton);

                // Adds "No" button with specific click event handler
                Button noButton = DesignHelpers.CreateButton("No", 340, 136, false, NoButton_Click);
                Controls.Add(noButton);
            }
            else
            {
                // Adds "OK" button for other statuses with specific click event handler
                Button Btn = DesignHelpers.CreateButton("OK", 340, 136, false, Ok_Btn_Click);
                Controls.Add(Btn);
            }

        }

        // Event handler for "OK" button click
        private void Ok_Btn_Click(object sender, EventArgs e) => this.Close();

        // Event handler for "Yes" button click
        private void YesButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes; // Sets dialog result to Yes
            this.Close(); // Closes the message box
        }

        // Event handler for "No" button click
        private void NoButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No; // Sets dialog result to No
            this.Close(); // Closes the message box
        }

        // Static method to show the custom message box and return the DialogResult
        public static DialogResult Show(string status, string message)
        {
            // Using statement ensures the form is disposed after use
            using (CustomMessageBoxForm customMessageBox = new CustomMessageBoxForm(status, message))
            {
                return customMessageBox.ShowDialog(); // Shows the custom message box and returns the dialog result
            }
        }
    }
}
