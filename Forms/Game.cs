using System.Data.SqlClient;
using SpaceShooter.Globals;
using SpaceShooter.Helpers.Design;
using SpaceShooter.Modals;
using SpaceShooter.Utilities;

namespace SpaceShooter
{
    public partial class Game : Form
    {

        private readonly Random random = new(); // Random number generator for various random operations
        private Image? ammoNumberImages, scoreNumberImages; // Images for displaying ammo count and score
        private readonly Image
            scoreLb = Image.FromFile($"{AssetManager.Path}UI\\ScoreLb.png"), // Image for score label
            ammoLb = Image.FromFile($"{AssetManager.Path}UI\\AmmoLb.png"), // Image for ammo label
            noAmmoLb = Image.FromFile($"{AssetManager.Path}UI\\NoMoreAmmo.png"), // Image for no ammo warning
            gameOverLb = Image.FromFile($"{AssetManager.Path}UI\\GameOver.png"), // Image for game over screen
            healthBarBorder = Image.FromFile($"{AssetManager.Path}UI\\healthBar.png"), // Image for health bar border
            healthBarPoint = Image.FromFile($"{AssetManager.Path}UI\\healthPoint.png"), // Image for health bar point
            playerImage = Image.FromFile($"{AssetManager.Path}Sprites\\player.png"), // Image for player
            ammoBoxImage = Image.FromFile($"{AssetManager.Path}Sprites\\ammoBox.png"); // Image for ammo box

        private readonly List<string> enemies_images = [.. Directory.GetFiles($"{AssetManager.Path}Sprites\\enemies", "*.png")]; // List of enemy images

        private Point playerInitialPosition; // Initial position of the player
        private readonly int AmmoInitial = 25, PlayerHealthInitial = 100; // Initial values for ammo and player health
        private readonly int AmmoBonus = 25, EnemyCount = 7; // Ammo bonus amount and number of enemies

        private int score; // Current score of the game
        public bool directionLeft, directionRight, directionUp, directionDown, gameOver; // Movement direction flags and game over flag

        private Player? player; // Player object
        private Entity? ammo; // Ammo entity
        private readonly List<Enemy> enemies = []; // List of enemies
        private readonly List<Projectile> projectiles = []; // List of projectiles

        private MenuStrip? menuStrip; // Menu strip for game controls


        public Game()
        {
            InitializeComponent();
            //DoubleBuffered = true; // Enables double buffering to reduce flickering
            DesignHelpers.SetDesign(this); // Sets visual design of the form
            menuStrip = DesignHelpers.SetupMenuStrip(this); // Sets up menu strip with game controls
        }

        // Sets up initial game world settings
        public void SetUpGameWorld()
        {
            
            // Optimizes painting for smoother graphics
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            // Centers player initially
            playerInitialPosition = new Point(this.ClientSize.Width / 2 - playerImage.Width, this.ClientSize.Height / 2 - playerImage.Height);
            // Creates player object
            player = new Player(playerInitialPosition.X, playerInitialPosition.Y , playerImage, this);
            // Combines images for displaying ammo count
            ammoNumberImages = AssetManager.CombineImages(player.ammo.ToString());
            // Combines images for displaying score
            scoreNumberImages = AssetManager.CombineImages(score.ToString());
            RestartGame(); // Initializes game state
        }

        // Pauses or resumes the game
        public void PauseResumeGame()
        {
            if (!gameOver)
            {
                if (GameTimer.Enabled) { 
                    GameTimer.Stop(); // Pauses game timer 
                    menuStrip!.Items[0].Text = "RESUME"; // Changes menu strip text to "RESUME"
                }
                else { 
                    GameTimer.Start(); // Resumes game timer
                    menuStrip!.Items[0].Text = "PAUSE";  // Changes menu strip text to "PAUSE"
                } 
            }
            else RestartGame(); // Restarts the game if it's over
        }

        // Restarts the game
        private void RestartGame()
        {
            menuStrip!.Items[0].Text = "PAUSE"; // Sets menu strip text to "PAUSE"
            //player.Image = Properties.Resources.player;
            score = 0; // Resets score
            player!.ammo = AmmoInitial; // Resets player ammo
            player!.position = playerInitialPosition; // Resets player position
            ammo = null; // Clears existing ammo
            enemies.Clear(); // Clears existing enemies
            projectiles.Clear(); // Clears existing projectiles
            player.health = PlayerHealthInitial; // Resets player health
            for (int i = 0; i < EnemyCount; i++) CreateEnemyInstance(); // Creates initial set of enemies
            directionUp = directionDown = directionLeft = directionRight = gameOver = false; // Resets movement directions and game over flag
            GameTimer.Start(); // Starts the game timer
        }

        // Updates player statistics in the database
        private void UpdatePlayerStatus()
        {
            try
            {
                DatabaseConnection.Open(); // Opens database connection

                // SQL query to update user statistic
                string updateQuery = "UPDATE [User] SET [Coins] = [Coins] + @CoinsToAdd, [HighestScore] = @Score WHERE [Name] = @UserName"; 
                int highestScore = score > AppGlobals.CurrentUser!.HighestScore ? score : AppGlobals.CurrentUser!.HighestScore; // Calculates highest score
                SqlCommand cmd = DatabaseConnection.CreateCommand(updateQuery); // Initializes SQL command
                cmd.Parameters.AddWithValue("@CoinsToAdd", score); // Adds parameter for coins to add
                cmd.Parameters.AddWithValue("@Score", highestScore); // Adds parameter for highest score
                cmd.Parameters.AddWithValue("@UserName", AppGlobals.CurrentUser!.UserName); // Adds parameter for username
                cmd.ExecuteNonQuery(); // Executes SQL command
                AppGlobals.CurrentUser.Coins += score; // Updates user's coins
                AppGlobals.CurrentUser.HighestScore = highestScore; // Updates user's highest score
            }
            catch (Exception ex) { MessageBox.Show($"Error updating user stats: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } 
            finally {
                DatabaseConnection.Close(); // Closes database connection
            }
        }

        // Event handler for painting graphics on the form
        private void GamePaintEvent(object sender, PaintEventArgs e)
        {
            // Draws health bar border
            e.Graphics.DrawImage(healthBarBorder, new Point(this.ClientSize.Width - 220, 56));
            // Draws ammo label and number images
            e.Graphics.DrawImage(ammoLb, new Point(4, 44));
            e.Graphics.DrawImage(ammoNumberImages!, new Point(96, 44));
            // Draws score label and number images
            e.Graphics.DrawImage(scoreLb, new Point(150, 44));
            e.Graphics.DrawImage(scoreNumberImages!, new Point(242, 44));

            // Draws ammo box and "no ammo" warning if ammo is empty
            if (ammo != null && !gameOver)
            {
                e.Graphics.DrawImage(ammo.image, ammo.position);
                e.Graphics.DrawImage(noAmmoLb, new Point(this.ClientSize.Width / 2 - 82, 4));
            }

            // Draws projectiles on the screen
            foreach (Projectile projectile in projectiles)
                e.Graphics.DrawImage(projectile.image, projectile.position);

            // Draws enemies on the screen
            foreach (Entity ship in enemies)
                e.Graphics.DrawImage(ship.image, ship.position);

            // Draws player on the screen
            e.Graphics.DrawImage(player!.image, player.position);

            // Draws game over screen if game is over
            if (gameOver) e.Graphics.DrawImage(gameOverLb, new Point(this.ClientSize.Width / 2 - gameOverLb.Width / 2, this.ClientSize.Height / 2 - gameOverLb.Height / 2));
            else
            {
                // Draws health points on the health bar
                for (int i = 0; i < player.health; i++)
                    e.Graphics.DrawImage(healthBarPoint, new Point(this.ClientSize.Width - 220 + (2 * i), 56));
            }
        }

        // Event handler for key down event
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver) return; // Exits if game is over
            // Sets movement direction flags based on key pressed
            if (e.KeyCode == Keys.A) directionLeft = true;
            if (e.KeyCode == Keys.D) directionRight = true;
            if (e.KeyCode == Keys.W) directionUp = true;
            if (e.KeyCode == Keys.S) directionDown = true;
        }

        // Event handler for key up event
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            // Resets movement direction flags based on key released
            if (e.KeyCode == Keys.A) directionLeft = false;
            if (e.KeyCode == Keys.D) directionRight = false;
            if (e.KeyCode == Keys.W) directionUp = false;
            if (e.KeyCode == Keys.S) directionDown = false;
        }

        // Event handler for mouse down event (shoots projectiles)
        private void Game_MouseDown(object sender, MouseEventArgs e)
        {
            // Invokes shooting action on player object based on mouse position
            if (InvokeRequired) Invoke(new Action(() => player!.ShootBullet(projectiles, this.PointToClient(MousePosition))));
            else player!.ShootBullet(projectiles, this.PointToClient(MousePosition)); // Shoots bullets towards the mouse position
        }

        // Updates game status, including ammo and score display
        private void UpdateGameStatus()
        {
            ammoNumberImages = AssetManager.CombineImages(player!.ammo.ToString()); // Updates ammo count images
            scoreNumberImages = AssetManager.CombineImages(score.ToString()); // Updates score images

            // Checks if player health is zero or less
            if (player.health < 1)
            {
                menuStrip!.Items[0].Text = "RESTART"; // Changes menu strip text to "RESTART"
                GameTimer.Stop(); // Stops game timer
                gameOver = true; // Sets game over flag
                UpdatePlayerStatus(); // Updates player statistics in the database
                UserManager.UpdateUserInfo(); // Updates user information globally
                //player.Image = Resources.player; // Resets player image (if necessary)
            }
        }

        // Event handler for game timer tick event
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            this.Invalidate(); // Invalidates the form to trigger repaint
            UpdateGameStatus(); // Updates game status (ammo, score, health)
            player!.Update(this.PointToClient(MousePosition)); // Updates player position based on mouse position

            try
            {
                // Iterates through each enemy and performs actions
                foreach (Enemy enemy in enemies)
                {
                    // Moves enemy towards player if not close enough
                    if (enemy.CheckDistanceWith(player.position) > 30)
                        enemy.MoveTowardsTarget();
                    enemy.AimOnTarget(); // Aims enemy towards player
                    // Checks collision between enemy and player
                    if (enemy.DetectCollisionWith(player))
                        player.health -= 1; // Decreases player health on collision

                    // Iterates through each projectile and performs actions
                    foreach (Projectile projectile in projectiles)
                    {
                        // Removes projectile if destroyed
                        if (projectile.Destroy())
                            projectiles.Remove(projectile);
                        // Checks collision between enemy and projectile
                        else if (enemy.DetectCollisionWith(projectile))
                        {
                            score += 100; // Increases score on hitting enemy
                            enemies.Remove(enemy); // Removes enemy
                            projectiles.Remove(projectile); // Removes projectile
                            CreateEnemyInstance(); // Creates new enemy instance
                        }
                    }
                }
            }
            catch { };

            // Drops ammo if player collides with ammo entity
            if (ammo != null && player.DetectCollisionWith(ammo))
            {
                player.ammo += AmmoBonus; // Increases player ammo
                ammo = null; // Clears ammo entity
            }
        }

        // Creates a new instance of enemy
        private void CreateEnemyInstance()
        {
            Image enemyImage = GetRandomEnemyImage(); // Gets random enemy image
            int enemyWidth = enemyImage.Width, enemyHeight = enemyImage.Height, spawnMargin = enemyImage.Width - 14; // Enemy dimensions and spawn margin
            int side = random.Next(4); // Randomly selects spawn side
            int randomX, randomY;

            // Calculates random spawn position based on selected side
            switch (side)
            {
                case 0: // Top side
                    randomX = random.Next(this.ClientSize.Width - enemyWidth);
                    randomY = -spawnMargin; break;
                case 1: // Right side
                    randomX = this.ClientSize.Width + spawnMargin;
                    randomY = random.Next(this.ClientSize.Height - enemyHeight); break;
                case 2: // Bottom side
                    randomX = random.Next(this.ClientSize.Width - enemyWidth);
                    randomY = this.ClientSize.Height + spawnMargin; break;
                case 3: // Left side
                    randomX = -spawnMargin;
                    randomY = random.Next(this.ClientSize.Height - enemyHeight); break;
                default: return;
            }

            // Creates new enemy object and adds to the list
            Enemy enemy = new Enemy(randomX, randomY, enemyImage, this, player!);
            enemies.Add(enemy);
        }

        // Drops ammo entity in the game world
        public void DropAmmo() =>
            ammo = new Entity(random.Next(104, this.ClientSize.Width - 64), random.Next(104, this.ClientSize.Height - 64), ammoBoxImage, this);

        // Retrieves a random enemy image from the list
        public Image GetRandomEnemyImage() => Image.FromFile(enemies_images[random.Next(0, enemies_images.Count)]);

    }
}
