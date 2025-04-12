using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SpaceShooter.Forms;
using SpaceShooter.Globals;
using SpaceShooter.Renderers;
using SpaceShooter.Utilities;

namespace SpaceShooter.Helpers.Design
{
    internal class DesignHelpers
    {
        public static string FontFamily { get; private set; } = "TW Cen MT";
        public static Color PrimaryColor = Color.LightSeaGreen, SecondaryColor = Color.SkyBlue;
        public static int ClientWidth = 1242, ClientHeight = 673;
        private static readonly Image CloseIcon = Image.FromFile($"{AssetManager.Path}ui\\close-white.png");
        public static Color ErrorColor = Color.FromArgb(252, 3, 65);

        // Importing the CreateRoundRectRgn function from Gdi32.dll
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        // Draw a custom border for a form
        private static void DrawCustomBorder(Form form, PaintEventArgs e)
        {
            if (form != null)
                using (Pen borderPen = new Pen(Color.White, 4)) { e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, form.ClientSize.Width - 1, form.ClientSize.Height - 1)); }
        }

        // Draws a custom border around the specified Windows Form with rounded corners.
        private static void DrawCustomBorder(Form form, PaintEventArgs e, int cornerRadius)
        {
            if (form != null)
            {
                // Define border properties
                int borderWidth = 4; // Thickness of the border
                Color borderColor = Color.White; // Color of the border

                // Create a GraphicsPath to define the border shape
                GraphicsPath borderPath = new GraphicsPath();
                borderPath.StartFigure();

                // Top-left corner (thicker)
                borderPath.AddArc(new Rectangle(0, 0, cornerRadius, cornerRadius), 180, 90);

                // Top edge (thicker)
                borderPath.AddLine(cornerRadius, 0, form.ClientSize.Width - cornerRadius, 0);

                // Top-right corner (thicker)
                borderPath.AddArc(new Rectangle(form.ClientSize.Width - cornerRadius, 0, cornerRadius, cornerRadius), 270, 90);

                // Right edge (thicker)
                borderPath.AddLine(form.ClientSize.Width, cornerRadius, form.ClientSize.Width, form.ClientSize.Height - cornerRadius);

                // Bottom-right corner (thicker)
                borderPath.AddArc(new Rectangle(form.ClientSize.Width - cornerRadius, form.ClientSize.Height - cornerRadius, cornerRadius, cornerRadius), 0, 90);

                // Bottom edge (thicker)
                borderPath.AddLine(form.ClientSize.Width - cornerRadius, form.ClientSize.Height, cornerRadius, form.ClientSize.Height);

                // Bottom-left corner (thicker)
                borderPath.AddArc(new Rectangle(0, form.ClientSize.Height - cornerRadius, cornerRadius, cornerRadius), 90, 90);

                // Left edge (thicker)
                borderPath.AddLine(0, form.ClientSize.Height - cornerRadius, 0, cornerRadius);

                borderPath.CloseFigure();

                // Draw the border
                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawPath(borderPen, borderPath);
                }
            }
        }

        // Sets up a custom border for the specified Form.
        public static void SetupBorder(Form form, int cornerRadius = 0)
        {
            form.FormBorderStyle = FormBorderStyle.None; // Remove standard border
            form.Paint += cornerRadius > 0 ? (sender, e) => DrawCustomBorder(sender as Form, e, cornerRadius) : (sender, e) => DrawCustomBorder(sender as Form, e);
            //form.Padding = new Padding(2); // Add padding to ensure border is visible
        }

        // Apply design settings to a form
        public static void SetDesign(Form form)
        {
            Program.EnableDoubleBuffering(form);
            form.BackgroundImage = AssetManager.backgroundImage;
            SetupBorder(form);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ClientSize = new Size(ClientWidth, ClientHeight);
            // Apply rounded corners to the form (currently commented out)
            //form.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, form.Width + 1, form.Height + 1, 16, 16));

            // Skip menu setup for certain forms
            if (form is not Game && form is not MainMenu && form is not Authentication && form is not CustomMessageBoxForm)
                SetupMenuStrip(form);
            else if (form is Authentication)
                SetupMenu(form, []);
        }

        // Create a custom button
        public static Button CreateButton(string text, int x, int y, bool isPrimary, EventHandler clickHandler)
        {
            Button button = new()
            {
                BackColor = isPrimary ? PrimaryColor : SecondaryColor,
                Font = new Font(FontFamily, 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                Text = text,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Location = new Point(x, y),
                AutoSize = true
            };
            button.Click += clickHandler;
            return button;
        }

        // Create a custom DateTimePicker
        public static DateTimePicker CreateDateTimePicker(int x, int y)
        {
            return new DateTimePicker()
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font(FontFamily, 13F, FontStyle.Regular),
                Width = 190,
                Location = new Point(x, y - 3)
            };
        }

        // Create a custom CheckBox
        public static CheckBox CreateCheckBox(int x, int y)
        {
            return new CheckBox()
            {
                Location = new Point(x, y),
                BackColor = Color.Transparent,
                ForeColor = SecondaryColor,
                Size = new Size(26, 26),
            };
        }

        // Create a custom Label
        public static Label CreateLabel(string text, int x, int y)
        {
            return new Label()
            {
                Text = text,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font(FontFamily, 13F, FontStyle.Bold, GraphicsUnit.Point, 0),
                Location = new Point(x, y),
                AutoSize = true,
            };
        }

        // Create a custom TextBox
        public static TextBox CreateTextBox(string name, int x, int y, int width, int height)
        {
            return new TextBox()
            {
                Name = name,
                BorderStyle = BorderStyle.Fixed3D,
                Font = new Font(FontFamily, 13F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = SecondaryColor,
                Location = new Point(x, y),
                Size = new Size(width, height),
                TextAlign = HorizontalAlignment.Center,
            };
        }

        // Create a custom PictureBox
        public static PictureBox CreatePictureBox(Image image, int x, int y)
        {
            return new PictureBox()
            {
                BackColor = Color.Transparent,
                Image = image,
                Width = image.Width,
                Location = new Point(x, y)
            };
        }

        // Add controls for account form
        public static (TextBox Tb_Name, TextBox Tb_Password, DateTimePicker dateTimePicker, CheckBox checkBox, TextBox Tb_Coins) AccountForm(Form form, int startposx, int startposy, int spacey, int offset)
        {
            (TextBox Tb_Name, TextBox Tb_Password) = AddNameAndPasswordTextBoxes(form, startposx, startposy, spacey, offset);
            (DateTimePicker dateTimePicker, CheckBox checkBox) = AddDateTimePickerAndCheckBox(form, startposx, startposy + spacey * 2, spacey, offset, true);
            Label coinsLb = CreateLabel("COINS", startposx + 138, startposy + spacey * 3); form.Controls.Add(coinsLb);
            TextBox Tb_Coins = CreateTextBox("Tb_Coins", startposx + 138 + coinsLb.Width + offset, startposy + spacey * 3, 140, 26); form.Controls.Add(Tb_Coins);
            return (Tb_Name, Tb_Password, dateTimePicker, checkBox, Tb_Coins);
        }

        // Add DateTimePicker and CheckBox controls to a form
        public static (DateTimePicker dateTimePicker, CheckBox checkBox) AddDateTimePickerAndCheckBox(Form form, int startposx, int startposy, int spacey, int offset, bool addToControls)
        {
            Label dobLb = CreateLabel("DATE OF BIRTH", startposx, startposy);
            DateTimePicker dateTimePicker = CreateDateTimePicker(startposx + dobLb.Width + offset * 4, startposy);
            Label adminLb = CreateLabel("ADMIN", startposx, startposy + spacey);
            CheckBox checkBox = CreateCheckBox(startposx + adminLb.Width, startposy + spacey);
            if (addToControls)
            {
                form.Controls.Add(dobLb);
                form.Controls.Add(dateTimePicker);
                form.Controls.Add(adminLb);
                form.Controls.Add(checkBox);
            }
            return (dateTimePicker, checkBox);
        }

        // Add Name and Password TextBoxes to a form
        public static (TextBox Tb_Name, TextBox Tb_Password) AddNameAndPasswordTextBoxes(Form form, int startposx, int startposy, int spacey, int offset)
        {
            Label nameLb = CreateLabel("NAME", startposx, startposy); form.Controls.Add(nameLb);
            TextBox Tb_Name = CreateTextBox("Tb_Name", startposx + nameLb.Width + offset, startposy, 281, 26); form.Controls.Add(Tb_Name);
            Label passLb = CreateLabel("PASSWORD", startposx, startposy + spacey); form.Controls.Add(passLb);
            TextBox Tb_Password = CreateTextBox("Tb_Password", startposx + passLb.Width + offset, startposy + spacey, 232, 26);
            Tb_Password.PasswordChar = '*'; form.Controls.Add(Tb_Password);
            return (Tb_Name, Tb_Password);
        }

        // Create a custom DataGridView with specific styles and event handler
        public static DataGridView CreateCustomDataGridView(int x, int y, int width, int height, Action<object, DataGridViewCellEventArgs> cellContentClickHandler)
        {
            DataGridView dataGridView = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                BackgroundColor = PrimaryColor,
                BorderStyle = BorderStyle.None,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                GridColor = SystemColors.HighlightText,
                Location = new Point(x, y),
                Margin = new Padding(2),
                Name = "dataGridView1",
                RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Size = new Size(width, height),
                ReadOnly = true
            };

            DataGridViewCellStyle cellStyle = new()
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = PrimaryColor,
                Font = new Font(FontFamily, 9F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.White,
                SelectionBackColor = SecondaryColor,
                SelectionForeColor = SystemColors.HighlightText,
                WrapMode = DataGridViewTriState.True
            };

            dataGridView.ColumnHeadersDefaultCellStyle = cellStyle;
            dataGridView.DefaultCellStyle = cellStyle;
            dataGridView.RowHeadersDefaultCellStyle = cellStyle;

            dataGridView.CellContentClick += new DataGridViewCellEventHandler(cellContentClickHandler!);

            return dataGridView;
        }

        // Set up a MenuStrip with specified menu items and a close button
        public static MenuStrip SetupMenu(Form form, ToolStripMenuItem[] menuItems)
        {
            MenuStrip menuStrip = new()
            {
                BackColor = Color.Transparent,
                ImageScalingSize = new Size(24, 24),
                Location = new Point(0, 0),
                Size = new Size(552, 28),
                Padding = new Padding(1, 1, 0, 0),
                Renderer = new CustomToolStripRenderer()
            };

            ToolStripMenuItem CloseToolStripMenu = new()
            {
                BackColor = Color.Transparent,
                Image = CloseIcon,
                Size = new Size(28, 28),
                Alignment = ToolStripItemAlignment.Right,
                Padding = new Padding(0, 1, 1, 0)

            };
            CloseToolStripMenu.Click += (sender, e) => Application.Exit();
            menuStrip.Items.AddRange(menuItems);
            menuStrip.Items.Add(CloseToolStripMenu);
            form.Controls.Add(menuStrip);
            return menuStrip;
        }

        // Set up MenuStrip for the Game form
        public static MenuStrip SetupMenuStrip(Game form)
        {
            ToolStripMenuItem PauseToolStripMenu = CreateToolStripMenuItem("PAUSE", new Size(62, 24), (sender, e) => form.PauseResumeGame());
            ToolStripMenuItem MainMenuToolStripMenu = CreateToolStripMenuItem("MAIN MENU", new Size(94, 24), (sender, e) => AppGlobals.MainMenu(form));
            ToolStripMenuItem SignOutToolStripMenu = CreateToolStripMenuItem("SIGN OUT", new Size(86, 24), (sender, e) => UserManager.Btn_SignOut_Click(form));
            ToolStripMenuItem[] menuItems = [PauseToolStripMenu, MainMenuToolStripMenu, SignOutToolStripMenu];
            return SetupMenu(form, menuItems);
        }

        // Set up MenuStrip for a general form
        public static MenuStrip SetupMenuStrip(Form form)
        {
            ToolStripMenuItem BackToolStripMenu = CreateToolStripMenuItem("BACK", new Size(54, 24), (sender, e) => AppGlobals.MainMenu(form));
            ToolStripMenuItem[] menuItems = [BackToolStripMenu];
            return SetupMenu(form, menuItems);
        }

        // Create a ToolStripMenuItem with specified properties
        private static ToolStripMenuItem CreateToolStripMenuItem(string text, Size size, EventHandler? clickHandler = null)
        {
            ToolStripMenuItem menuItem = new()
            {
                Font = new Font(FontFamily, 9F, FontStyle.Bold, GraphicsUnit.Point, 0),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Size = size,
                Text = text,
            };

            if (clickHandler != null) menuItem.Click += clickHandler;

            if (menuItem.Owner is ToolStrip ownerStrip)
                ownerStrip.Padding = new Padding(ownerStrip.Padding.Left, ownerStrip.Padding.Top, 0, ownerStrip.Padding.Bottom);
            return menuItem;
        }
    }
}
