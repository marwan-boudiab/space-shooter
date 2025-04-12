using SpaceShooter.Helpers.Design;
namespace SpaceShooter.Controls
{
    public class TransparentToolStripMenuItem : ToolStripMenuItem
    {
        public TransparentToolStripMenuItem(string text, Size size, EventHandler? clickHandler = null)
        {
            InitializeMenuItem(text, size, clickHandler);
        }

        private void InitializeMenuItem(string text, Size size, EventHandler? clickHandler)
        {
            Text = text;
            Font = new Font(DesignHelpers.FontFamily, 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Size = size;
            Margin = new Padding(0, Margin.Top, Margin.Right, Margin.Bottom); // Set X location to 0

            if (clickHandler != null)
            {
                Click += clickHandler;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the background
            e.Graphics.FillRectangle(new SolidBrush(BackColor), Bounds);

            // Draw the text with the desired ForeColor
            TextRenderer.DrawText(e.Graphics, Text, Font, Bounds, ForeColor, Color.Transparent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // Prevent background color change on hover
            BackColor = Color.Transparent;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // Prevent background color change on hover
            BackColor = Color.Transparent;
            base.OnMouseLeave(e);
        }
    }


}
