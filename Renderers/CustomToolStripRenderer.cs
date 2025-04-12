using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Renderers
{
    public class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        // Constructor to initialize with custom color table
        public CustomToolStripRenderer() : base(new CustomColorTable()) { }

        // Override to modify background color when rendering item background
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(Brushes.Transparent, e.Item.ContentRectangle);
                e.ToolStrip!.Cursor = Cursors.Hand;
            }
            else
            {
                e.ToolStrip!.Cursor = Cursors.Default;
                base.OnRenderMenuItemBackground(e);
            }
        }
    }

    public class CustomColorTable : ProfessionalColorTable
    {
        // Override to set custom colors for various parts of the ToolStrip
        public override Color MenuItemSelected
        {
            get { return Color.Transparent; }  // Transparent background when selected
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.Transparent; }  // Transparent gradient begin color
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.Transparent; }  // Transparent gradient end color
        }

        public override Color MenuItemBorder
        {
            get { return Color.Transparent; }  // Transparent border color
        }
    }
}
