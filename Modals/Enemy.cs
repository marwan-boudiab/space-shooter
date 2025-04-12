namespace SpaceShooter
{
    // Enemy class inheriting from Entity
    internal class Enemy : Entity
    {
        // Fields specific to Enemy
        private new readonly int speed = 3;    // Speed of the enemy (overrides base speed)
        private readonly Entity target;        // Target entity (usually the player)

        // Constructor
        public Enemy(int posX, int posY, Image image, Game game, Entity target) : base(posX, posY, image, game)
        {
            this.target = target; // Initialize the target entity
        }

        // Method to move towards the target
        public void MoveTowardsTarget()
        {
            // Move horizontally towards the target's X position
            if (position.X > target.position.X) position.X -= speed;
            if (position.X < target.position.X) position.X += speed;

            // Move vertically towards the target's Y position
            if (position.Y > target.position.Y) position.Y -= speed;
            if (position.Y < target.position.Y) position.Y += speed;
        }

        // Method to aim at the target
        public void AimOnTarget()
        {
            // Calculate the angle to rotate towards the target
            double angle = Math.Atan2(target.position.Y - (position.Y + image.Height / 2), target.position.X - (position.X + image.Width / 2));

            // Rotate the enemy's image
            image = RotateImage(angle);
        }
    }
}
