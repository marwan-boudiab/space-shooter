namespace SpaceShooter.Modals
{
    // Projectile class inheriting from Entity
    internal class Projectile : Entity
    {
        // Fields specific to Projectile
        private new readonly int speed = 16;        // Speed of the projectile (overrides base speed)
        private readonly System.Windows.Forms.Timer movementTimer = new(); // Timer for movement updates

        // Constructor Initialize the projectile with its position, image, and game reference
        public Projectile(int posX, int posY, Image image, Game game) : base(posX, posY, image, game) { }

        // Method to setup the projectile's movement towards a target point
        public void ProjectileSetup(Point target)
        {
            SetTarget(target);  // Set velocity towards the target
            movementTimer.Interval = 16;  // Set timer interval (milliseconds)
            movementTimer.Tick += new EventHandler(BulletTimerEvent!);  // Hook up timer event handler
            movementTimer.Start();  // Start the movement timer
        }

        // Event handler for the movement timer
        private void BulletTimerEvent(object sender, EventArgs e)
        {
            // Update projectile position based on velocity
            position.X += (int)Math.Round(velocity.X);
            position.Y += (int)Math.Round(velocity.Y);
        }

        // Method to check if the projectile should be destroyed
        public bool Destroy()
        {
            /*image.Dispose();
            bulletTimer = null;
            image = null;*/

            // Check if projectile is out of bounds
            if (position.X < 10 || position.X + image.Width > 1250 || position.Y < 10 || position.Y > 861)
            {
                // Stop and dispose of movement timer
                movementTimer.Stop();
                movementTimer.Dispose();
                return true; // Signal projectile destruction
            }
            return false; // Projectile is still active
        }

        // Method to set the velocity of the projectile towards a target point
        private void SetTarget(Point target)
        {
            // Calculate angle towards the target
            double angle = Math.Atan2(target.Y - position.Y, target.X - position.X);

            // Set velocity components based on speed and angle
            velocity.X = (float)(speed * Math.Cos(angle));
            velocity.Y = (float)(speed * Math.Sin(angle));
        }

    }
}
