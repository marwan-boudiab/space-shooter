namespace SpaceShooter
{
    // Base class for entities in the game
    internal class Entity
    {
        // Fields
        public Image image;              // Current image of the entity
        private readonly Image originalImage; // Original image of the entity
        public PointF velocity = new();  // Velocity of the entity
        public Point position = new();   // Position of the entity
        protected int speed = 10;        // Speed of the entity
        protected Game game;             // Reference to the game instance

        // Constructor
        public Entity(int posX, int posY, Image image, Game game)
        {
            this.image = image;
            this.originalImage = image; // Store the original image for rotations
            // Initialize position centered on posX, posY
            this.position.X = posX - image.Width / 2;
            this.position.Y = posY - image.Height / 2;
            this.game = game; // Store the reference to the game
        }

        // Method to rotate the entity's image
        public Image RotateImage(double angle)
        {
            int width = originalImage.Width;
            int height = originalImage.Height;

            Bitmap rotatedImage = new(width, height);
            //rotatedImage.SetResolution(image.HorizontalResolution - 8, image.VerticalResolution - 8);

            // Rotate the image using Graphics
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(width / 2, height / 2); // Translate to the center of the image
                g.RotateTransform((float)(angle * (180.0 / Math.PI))); // Rotate by the specified angle
                g.TranslateTransform(-width / 2, -height / 2); // Translate back
                g.DrawImage(originalImage, new Point(0, 0)); // Draw the original image
            }

            return rotatedImage;
        }

        // Method to detect collision with another entity
        public bool DetectCollisionWith(Entity entity)
        {
            // Check if there is no collision by comparing bounding boxes
            if (position.X + image.Width <= entity.position.X ||
                position.X >= entity.position.X + entity.image.Width ||
                position.Y + image.Height <= entity.position.Y ||
                position.Y >= entity.position.Y + entity.image.Height)
                return false;
            else
                return true;
        }

        // Method to calculate distance to a point
        public float CheckDistanceWith(Point p2) => (float)Math.Sqrt((Math.Pow(position.X - p2.X, 2) + Math.Pow(position.Y - p2.Y, 2)));
    }
}
