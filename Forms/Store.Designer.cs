using SpaceShooter.Helpers.Design;

namespace SpaceShooter
{
    partial class Store
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Cart = new ListBox();
            Purchased = new ListBox();
            SuspendLayout();
            // 
            // Cart
            // 
            Cart.BackColor = Color.LightSeaGreen;
            Cart.Font = new Font(DesignHelpers.FontFamily, 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Cart.ForeColor = Color.White;
            Cart.FormattingEnabled = true;
            Cart.Location = new Point(35, 382);
            Cart.Name = "Cart";
            Cart.Size = new Size(480, 184);
            Cart.TabIndex = 1;
            // 
            // Purchased
            // 
            Purchased.BackColor = Color.LightSeaGreen;
            Purchased.Font = new Font(DesignHelpers.FontFamily, 10.8F, FontStyle.Bold);
            Purchased.ForeColor = Color.White;
            Purchased.FormattingEnabled = true;
            Purchased.Location = new Point(727, 382);
            Purchased.Name = "Purchased";
            Purchased.Size = new Size(480, 184);
            Purchased.TabIndex = 2;
            // 
            // Store
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1242, 673);
            Controls.Add(Purchased);
            Controls.Add(Cart);
            Name = "Store";
            Text = "Store";
            Load += Store_Load;
            ResumeLayout(false);
        }

        #endregion
        private ListBox Cart;
        private ListBox Purchased;
    }
}