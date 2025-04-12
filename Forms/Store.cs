using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using SpaceShooter.Controls;
using SpaceShooter.Globals;
using SpaceShooter.Helpers.Design;
using SpaceShooter.Utilities;

namespace SpaceShooter
{
    public partial class Store : Form
    {
        private int TotalPrice = 0, pictureIndex = 1; // Private fields for total price and picture index
        private readonly int pricePerItem = 5000; // Constant price per item
        private Label? TotalPriceLb; // Nullable label for displaying total price
        private RoundedPictureBox? BgPreview; // Nullable rounded picture box for displaying background preview
        private readonly List<string> CartPaths = []; // List to store paths of items in the cart

        public Store()
        {
            InitializeComponent(); // Initializes form components
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            SetUpForm(); // Sets up the UI for the store
        }

        private void SetUpForm()
        {
            DesignHelpers.SetDesign(this); // Sets the form design
            BgPreview = new RoundedPictureBox // Initializes and configures the background preview picture box
            {
                Size = new Size(505, 234),
                Location = new Point(370, 31),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            Controls.Add(BgPreview); // Adds background preview picture box to the form

            int startposx = BgPreview.Location.X;

            // Creates and adds buttons for navigation and interaction
            Controls.Add(DesignHelpers.CreateButton(" NEXT ", startposx, BgPreview.Top + BgPreview.Height + 24, true, Btn_Next_Click!));
            Controls.Add(DesignHelpers.CreateButton("ADD TO CART ", startposx + 128, BgPreview.Top + BgPreview.Height + 24, true, Btn_Add_To_Cart!));
            Controls.Add(DesignHelpers.CreateLabel("CART", Cart.Location.X, Cart.Location.Y - 35)); // Creates and adds labels
            Controls.Add(DesignHelpers.CreateLabel("PURCHASED", Purchased.Location.X, Purchased.Location.Y - 35));
            Controls.Add(DesignHelpers.CreateButton(" > ", startposx + BgPreview.Width / 2 - 42, 415, true, Btn_Purchase_Selected_Item!)); // Creates and adds purchase buttons
            Controls.Add(DesignHelpers.CreateButton(" >> ", startposx + BgPreview.Width / 2 - 42, 475, true, Btn_Purchase_All!));
        }

        private void Store_Load(object sender, EventArgs e)
        {
            RefreshItems(); // Loads purchased items
            BgPreview!.Image = Image.FromFile(AssetManager.background_images[1]); // Sets initial background preview image
            BgPreview.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void RefreshItems()
        {
            Purchased.Items.Clear(); // Clears purchased items list
            foreach (var item in AppGlobals.CurrentUser!.Inventory)
            {
                Purchased.Items.Add(Path.GetFileName(item)); // Adds purchased items to the list
            }
        }

        private void Btn_Next_Click(object sender, EventArgs e)
        {
            if (pictureIndex == AssetManager.background_images.Count - 1) // Checks if at the end of available images
                pictureIndex = 1;
            else
                pictureIndex++;

            BgPreview!.Image = Image.FromFile(AssetManager.background_images[pictureIndex]); // Updates background preview image
            BgPreview.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        
        private void Purchase(int price, string? filePath = null)
        {
            try
            {
                DatabaseConnection.Open(); // Opens database connection
                SqlCommand retrieveCmd = DatabaseConnection.CreateCommand($"SELECT [Inventory] FROM [User] WHERE ID = {AppGlobals.CurrentUser!.ID}"); // SQL command to retrieve inventory
                string? currentInventory = retrieveCmd.ExecuteScalar() as string; // Executes command and retrieves current inventory as string
                List<string> inventoryList = currentInventory != null ? JsonConvert.DeserializeObject<List<string>>(currentInventory)!: new List<string>(); // Deserializes inventory list
                if (filePath == null) inventoryList.AddRange(CartPaths); // Adds items from cart to inventory
                else inventoryList!.Add(filePath); // Adds single item to inventory
                string updatedInventory = JsonConvert.SerializeObject(inventoryList); // Serializes updated inventory
                SqlCommand updateCmd = DatabaseConnection.CreateCommand($"UPDATE [User] SET [Coins] = [Coins] - {price}, [Inventory] = '{updatedInventory}' WHERE ID = {AppGlobals.CurrentUser.ID}"); // SQL command to update user coins and inventory
                updateCmd.ExecuteNonQuery(); // Executes update command
                AppGlobals.SuccessMessageBox($"Purchased. {price} Coins have been deduced from your inventory"); // Success message
                AppGlobals.CurrentUser.Coins -= price; // Updates current user's coins
                AppGlobals.CurrentUser.Inventory = inventoryList; // Updates current user's inventory
                UserManager.UpdateUserInfo(); // Updates user information
            }
            catch { }
            finally {
                DatabaseConnection.Close();  // Closes database connection
            }
        }

        private void Btn_Add_To_Cart(object sender, EventArgs e)
        {
            string fullPath = AssetManager.background_images[pictureIndex]; // Gets full path of selected image
            string filename = Path.GetFileName(fullPath); // Extracts filename from path
            string justFilename = filename.Substring(0, filename.LastIndexOf('.')); // Removes file extension


            if (!Cart.Items.Contains(justFilename))
            {
                if (!AppGlobals.CurrentUser!.Inventory.Contains(justFilename)) // Checks if item is already purchased
                {
                    CartPaths.Add(justFilename); // Adds item to cart paths
                    Cart.Items.Add(justFilename); // Adds item to cart list

                    Controls.Remove(TotalPriceLb); // Removes total price label from controls
                    TotalPrice += pricePerItem; // Updates total price
                    TotalPriceLb = DesignHelpers.CreateLabel($"total price {TotalPrice}", Cart.Location.X, Cart.Location.Y + Cart.Height); // Creates total price label
                    Controls.Add(TotalPriceLb); // Adds total price label to controls
                }
                else AppGlobals.ErrorMessageBox($"{justFilename} is already in the purchased."); // Error message if item is already purchased
            }
            else AppGlobals.ErrorMessageBox($"{justFilename} is already in the cart."); // Error message if item is already in cart
        }

        private void Btn_Purchase_Selected_Item(object sender, EventArgs e)
        {
            if (Cart.SelectedItem != null)
            {
                if (AppGlobals.CurrentUser!.Coins >= pricePerItem) // Checks if user has enough coins
                {
                    Purchased.Items.Add(Cart.SelectedItem); // Adds item to purchased list
                    Purchase(pricePerItem, Cart.SelectedItem.ToString()!); // Executes purchase

                    CartPaths.Remove(Cart.SelectedItem.ToString()!); // Removes item from cart paths
                    Cart.Items.Remove(Cart.SelectedItem); // Removes item from cart list

                    Controls.Remove(TotalPriceLb); // Removes total price label from controls
                    TotalPrice -= pricePerItem; // Updates total price
                    TotalPriceLb = DesignHelpers.CreateLabel($"total price {TotalPrice}", Cart.Location.X, Cart.Location.Y + Cart.Height); // Creates total price label
                    Controls.Add(TotalPriceLb); // Adds total price label to controls
                }
                else AppGlobals.ErrorMessageBox("You Don't have enough Coins."); // Error message if user doesn't have enough coins
            }
            else AppGlobals.ErrorMessageBox("Make Sure to Select an Item."); // Error message if no item is selected
        }

        private void Btn_Purchase_All(object sender, EventArgs e)
        {
            if (Cart.Items.Count > 0)
            {
                if (AppGlobals.CurrentUser!.Coins >= TotalPrice) // Checks if user has enough coins
                {
                    Purchase(TotalPrice); // Executes purchase for all items in cart
                    Controls.Remove(TotalPriceLb); // Removes total price label from controls

                    for (int i = 0; i < Cart.Items.Count; i++)
                        Purchased.Items.Add(Cart.Items[i]); // Moves all items to purchased list

                    Cart.Items.Clear(); // Clears cart items
                    CartPaths.Clear(); // Clears cart paths
                    TotalPrice = 0; // Resets total price
                    TotalPriceLb = DesignHelpers.CreateLabel($"total price {TotalPrice}", Cart.Location.X, Cart.Location.Y + Cart.Height); // Creates total price label
                    Controls.Add(TotalPriceLb); // Adds total price label to controls
                } else AppGlobals.ErrorMessageBox("You Don't have enough Coins."); // Error message if user doesn't have enough coins
            } else AppGlobals.ErrorMessageBox("Cart is Empty."); // Error message if cart is empty
        }

    }
}
