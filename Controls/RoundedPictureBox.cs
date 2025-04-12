using System.Drawing.Drawing2D;


namespace SpaceShooter.Controls
{
    internal class RoundedPictureBox : PictureBox
    {
        private int cornerRadius = 30; // Radius for rounded corners
        private int borderWidth = 6; // Width of the border
        private Color borderColor = Color.White; // Color of the border

        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                Invalidate(); // Redraw control when radius changes
            }
        }

        public int BorderWidth
        {
            get { return borderWidth; }
            set
            {
                borderWidth = value;
                Invalidate(); // Redraw control when border width changes
            }
        }

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate(); // Redraw control when border color changes
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw border
            using (var pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                GraphicsPath path = CreateRoundedPath();
                e.Graphics.DrawPath(pen, path);
            }

            // Draw image content
            using (var path = CreateRoundedPath())
            {
                Region = new Region(path);
            }
        }

        private GraphicsPath CreateRoundedPath()
        {
            GraphicsPath path = new GraphicsPath();

            int width = Width - 1;
            int height = Height - 1;
            int radius = cornerRadius * 2;
            //int diameter = radius - borderWidth;

            path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
            path.AddArc(width - radius, 0, radius, radius, 270, 90); // Top-right corner
            path.AddArc(width - radius, height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(0, height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            return path;
        }
    }
}
