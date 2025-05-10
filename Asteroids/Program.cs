using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroids
{
    class Program
    {
        public static int screenWidth = 800;
        public static int screenHeight = 600;
        public static bool isGameOver = false; // Tracks game state

        public static Texture2D playerTexture;
        public static Texture2D asteroidTexture;
        public static Texture2D bulletTexture;
        public static Texture2D enemyTexture;
        public static Texture2D enemyBulletTexture;
        public static Texture2D backgroundTexture;
        public static Vector2 gravity = new Vector2(0, 0.1f);

        public static int level = 1; // New: Track the current level
        public static Random random = new Random();

        static void Main(string[] args)
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Asteroids");
            Raylib.SetTargetFPS(60);

            // Load textures once
            LoadTextures();

            Player player = new Player(screenWidth / 2, screenHeight / 2, playerTexture);
            List<Asteroid> asteroids = CreateAsteroids(5, asteroidTexture, player.Position, 150);
            List<Bullet> bullets = new List<Bullet>();
            List<Bullet> enemyBullets = new List<Bullet>();
            Enemy enemy = new Enemy(enemyTexture, enemyBulletTexture, new Vector2(100, 100)); // Enemy spaceship


            while (!Raylib.WindowShouldClose())
            {
                if (!isGameOver)
                {
                    // Update entities
                    player.Update();
                    enemy.Update(enemyBullets);

                    foreach (var bullet in bullets) bullet.Update();
                    foreach (var enemyBullet in enemyBullets) enemyBullet.Update();

                    bullets.RemoveAll(b => !b.IsOnScreen() || b.IsExpired());
                    enemyBullets.RemoveAll(b => !b.IsOnScreen() || b.IsExpired());

                    foreach (var asteroid in asteroids) asteroid.Update();

                    // Check collisions
                    CheckPlayerCollisions(player, asteroids, enemyBullets);
                    CheckBulletCollisions(bullets, asteroids, enemy);

                    // Check if level is complete
                    if (asteroids.Count == 0 && enemy.IsDestroyed)
                    {
                        level++; // Increase the level
                        StartNextLevel(ref player, ref asteroids, ref bullets, ref enemyBullets, ref enemy);
                    }

                    // Shoot bullets
                    if (Raylib.IsKeyDown(KeyboardKey.Space)) player.Shoot(bullets, bulletTexture);
                }
                else
                {
                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                        RestartGame(ref player, ref asteroids, ref bullets, ref enemyBullets, ref enemy);
                }

                // Render everything
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.DrawTexturePro(
                    backgroundTexture,
                    new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height), // Full image
                    new Rectangle(0, 0, Program.screenWidth, Program.screenHeight), // Scale to screen size
                    new Vector2(0, 0), // Origin (top-left corner)
                    0, // No rotation
                    Color.White // Keep original colors
                );


                if (!isGameOver)
                {
                    player.Draw();
                    enemy.Draw();
                    foreach (var bullet in bullets) bullet.Draw();
                    foreach (var enemyBullet in enemyBullets) enemyBullet.Draw();
                    foreach (var asteroid in asteroids) asteroid.Draw();

                    // Display the current level
                    Raylib.DrawText($"Level: {level}", 10, 10, 20, Color.White);
                }
                else
                {
                    Raylib.DrawText("Game Over!", screenWidth / 2 - 100, screenHeight / 2 - 50, 40, Color.Red);
                    Raylib.DrawText("Press 'R' to Restart", screenWidth / 2 - 150, screenHeight / 2, 30, Color.White);
                }

                Raylib.EndDrawing();
            }

            UnloadTextures();
            Raylib.CloseWindow();
        }

        static void LoadTextures()
        {
            // Preload textures
            playerTexture = Raylib.LoadTexture("Images/player.png");
            asteroidTexture = Raylib.LoadTexture("Images/asteroid.png");
            bulletTexture = Raylib.LoadTexture("Images/bullet.png");
            enemyTexture = Raylib.LoadTexture("Images/enemy.png");
            enemyBulletTexture = Raylib.LoadTexture("Images/enemybullet.png");
            backgroundTexture = Raylib.LoadTexture("Images/background.png");

        }

        static void UnloadTextures()
        {
            // Unload all textures
            Raylib.UnloadTexture(playerTexture);
            Raylib.UnloadTexture(asteroidTexture);
            Raylib.UnloadTexture(bulletTexture);
            Raylib.UnloadTexture(enemyTexture);
            Raylib.UnloadTexture(enemyBulletTexture);
            Raylib.UnloadTexture(backgroundTexture);
        }

        static List<Asteroid> CreateAsteroids(int count, Texture2D texture, Vector2 playerPosition, float safeZoneRadius)
        {
            List<Asteroid> asteroids = new List<Asteroid>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                Vector2 position;
                do
                {
                    position = new Vector2(random.Next(screenWidth), random.Next(screenHeight));
                }
                while (IsWithinSafeZone(position, playerPosition, safeZoneRadius));

                Vector2 velocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
                asteroids.Add(new Asteroid(texture, position, velocity, AsteroidSize.Large)); // Start with large asteroids
            }

            return asteroids;
        }

        static bool IsWithinSafeZone(Vector2 position, Vector2 playerPosition, float safeZoneRadius)
        {
            float dx = position.X - playerPosition.X;
            float dy = position.Y - playerPosition.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            return distance < safeZoneRadius;
        }

        static void SplitAsteroid(List<Asteroid> asteroids, Asteroid asteroid)
        {
            // Ensure the asteroid is valid before proceeding
            if (asteroid == null) return;

            // Check the size of the asteroid and split accordingly
            if (asteroid.Size == AsteroidSize.Large)
            {
                // Create two medium asteroids
                asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, asteroid.Velocity * 0.8f, AsteroidSize.Medium));
                asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, asteroid.Velocity * -0.8f, AsteroidSize.Medium));
            }
            else if (asteroid.Size == AsteroidSize.Medium)
            {
                // Create two small asteroids
                asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, asteroid.Velocity * 0.8f, AsteroidSize.Small));
                asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, asteroid.Velocity * -0.8f, AsteroidSize.Small));
            }

            // Small asteroids do not split further
        }


        static void RestartGame(ref Player player, ref List<Asteroid> asteroids, ref List<Bullet> bullets, ref List<Bullet> enemyBullets, ref Enemy enemy)
        {
            isGameOver = false;
            level = 1; // Reset the level

            player = new Player(screenWidth / 2, screenHeight / 2, playerTexture);
            asteroids = CreateAsteroids(5, asteroidTexture, player.Position, 150);
            bullets.Clear();
            enemyBullets.Clear();
            enemy = new Enemy(enemyTexture, enemyBulletTexture, new Vector2(100, 100));
        }

        static void StartNextLevel(ref Player player, ref List<Asteroid> asteroids, ref List<Bullet> bullets, ref List<Bullet> enemyBullets, ref Enemy enemy)
        {
            player = new Player(screenWidth / 2, screenHeight / 2, playerTexture);
            asteroids = CreateAsteroids(5 + level, asteroidTexture, player.Position, 150); // Increase asteroid count
            bullets.Clear();
            enemyBullets.Clear();
            enemy = new Enemy(enemyTexture, enemyBulletTexture, new Vector2(100 + level * 10, 100 + level * 10)); // Slightly move the enemy
        }

        static void CheckPlayerCollisions(Player player, List<Asteroid> asteroids, List<Bullet> enemyBullets)
        {
            foreach (var asteroid in asteroids)
                if (player.CollidesWith(asteroid)) { isGameOver = true; break; }

            foreach (var enemyBullet in enemyBullets)
                if (player.CollidesWith(enemyBullet)) { isGameOver = true; break; }
        }

        static void CheckBulletCollisions(List<Bullet> bullets, List<Asteroid> asteroids, Enemy enemy)
        {
            // Iterate over bullets in reverse to safely remove them
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bool bulletRemoved = false;

                // Check collision with asteroids
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (bullets[i].CollidesWith(asteroids[j]))
                    {
                        // Split the asteroid and remove both the bullet and asteroid
                        SplitAsteroid(asteroids, asteroids[j]);
                        asteroids.RemoveAt(j);
                        bullets.RemoveAt(i);
                        bulletRemoved = true;
                        break; // Exit asteroid loop as the bullet is removed
                    }
                }

                // If the bullet was removed, skip the enemy check
                if (bulletRemoved) continue;

                // Check collision with the enemy
                if (!enemy.IsDestroyed && bullets[i].CollidesWith(enemy))
                {
                    enemy.Destroy(); // Destroy the enemy
                    bullets.RemoveAt(i); // Remove the bullet
                }
            }
        }
    }
}


    