using SpaceShooter.Utilities;

namespace SpaceShooter.Modals
{
    // Player class inheriting from Entity
    internal class Player : Entity
    {

        // Fields specific to Player
        public int ammo = 10;                            // Current ammo count
        public int health = 100;                         // Current health
        private readonly Image projectileImage;          // Image used for projectiles

        // Constructor
        public Player(int posX, int posY, Image image, Game game) :
            base(posX, posY, image, game)
        {
            // Initialize the projectile image from file
            projectileImage = Image.FromFile($"{AssetManager.Path}Sprites\\projectile.png");
        }

        // Method to shoot a bullet
        public void ShootBullet(List<Projectile> projectiles, Point target)
        {
            if (ammo > 0)
            {
                // Create a new Projectile instance at the center of the player
                Projectile shootBullet = new(position.X + image.Width / 2, position.Y + image.Height / 2, projectileImage, game);

                // Setup the projectile with the target direction
                shootBullet.ProjectileSetup(target);

                // Add the projectile to the list
                projectiles.Add(shootBullet);

                // Decrease ammo count
                ammo--;

                // Notify the game if ammo drops below 1
                if (ammo < 1) game.DropAmmo();
            }
        }

        // Method to update player position and rotation
        public void Update(Point target)
        {
            // Update position based on directional input from the game
            if (game.directionLeft && position.X > 0)
                position.X -= speed;
            else if (game.directionRight && position.X + image.Width < game.ClientSize.Width)
                position.X += speed;

            if (game.directionUp && position.Y > 85)
                position.Y -= speed;
            else if (game.directionDown && position.Y + image.Height < game.ClientSize.Height)
                position.Y += speed;

            // Calculate the angle to rotate the player towards the target
            double angle = Math.Atan2(target.Y - (position.Y + image.Height / 2), target.X - (position.X + image.Width / 2));

            // Rotate the player's image
            image = RotateImage(angle);

        }
    }
}
