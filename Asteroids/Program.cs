using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroids
{

    enum GameState
    {
        MainMenu,
        Playing,
        GameOver,
        Settings,
        Paused,
        Quit
    }
    class Program
    {

        public static int screenWidth = 800;
        public static int screenHeight = 600;

        public static Texture2D playerTexture;
        public static Texture2D asteroidTexture;
        public static Texture2D bulletTexture;
        public static Texture2D enemyTexture;
        public static Texture2D enemyBulletTexture;
        public static Texture2D backgroundTexture;
        public static Vector2 gravity = new Vector2(0, 0.1f);

        public static int level = 1; // Track the current level
        public static Random random = new Random();
        public static GameState currentState = GameState.MainMenu;
        public static MainMenu mainMenu; // instantiate and subscribe in Main
        public static SettingsMenu settingsMenu = new SettingsMenu();

        // Moved game objects to Program-level static fields so helper methods can access them directly
        public static Player player;
        public static List<Asteroid> asteroids;
        public static List<Bullet> bullets;
        public static List<Bullet> enemyBullets;
        public static Enemy enemy;


        static void Main(string[] args)
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Asteroids");
            Raylib.SetTargetFPS(60);

            // Disable the default ESC-as-exit behavior in Raylib so Escape can be used in-game
            Raylib.SetExitKey(KeyboardKey.Null);

            // Load textures once
            LoadTextures();

            // Create menu and subscribe to its events (event-driven state changes)
            mainMenu = new MainMenu();

            // Event handlers
            mainMenu.StartGame += () =>
            {
                // Centralized reset and switch to Playing
                ResetGame();
                currentState = GameState.Playing;
            };
            mainMenu.OpenSettings += () =>
            {
                currentState = GameState.Settings;
            };
            mainMenu.ExitGame += () =>
            {
                // Request quit by setting game state -> loop will exit cleanly
                currentState = GameState.Quit;
            };

            // Initialize game objects for the first time
            ResetGame();

            // Loop until the window should close or the game state becomes Quit
            while (!Raylib.WindowShouldClose() && currentState != GameState.Quit)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw background
                Raylib.DrawTexturePro(
                    backgroundTexture,
                    new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height),
                    new Rectangle(0, 0, screenWidth, screenHeight),
                    new Vector2(0, 0),
                    0,
                    Color.White
                );

                switch (currentState)
                {
                    case GameState.MainMenu:
                        // Event-driven: Update may fire StartGame/OpenSettings/ExitGame
                        mainMenu.Update();
                        mainMenu.Draw(screenWidth, screenHeight);
                        break;

                    case GameState.Settings:
                        var settingsResult = settingsMenu.Update();
                        settingsMenu.Draw(screenWidth, screenHeight);

                        // (Moved) sound volume is now applied by SettingsMenu.Update()
                        if (settingsResult == SettingsMenu.SettingsResult.Back)
                        {
                            currentState = GameState.MainMenu;
                        }
                        break;

                    case GameState.Playing:
                        // Pause logic
                        if (Raylib.IsKeyPressed(KeyboardKey.P) || Raylib.IsKeyPressed(KeyboardKey.Escape))
                        {
                            currentState = GameState.Paused;
                            break;
                        }

                        // Update entities
                        player.Update();
                        enemy.Update(enemyBullets);

                        foreach (var bullet in bullets) bullet.Update();
                        foreach (var enemyBullet in enemyBullets) enemyBullet.Update();

                        bullets.RemoveAll(b => !b.IsOnScreen() || b.IsExpired());
                        enemyBullets.RemoveAll(b => !b.IsOnScreen() || b.IsExpired());

                        foreach (var asteroid in asteroids) asteroid.Update();

                        // Check collisions (this now updates currentState to GameOver when needed)
                        CheckPlayerCollisions(player, asteroids, enemyBullets);

                        // If a collision turned the state into GameOver, skip the rest of the frame's playing logic
                        if (currentState == GameState.GameOver) break;

                        CheckBulletCollisions(bullets, asteroids, enemy);

                        // Check if level is complete
                        if (asteroids.Count == 0 && enemy.IsDestroyed)
                        {
                            level++; // Increase the level
                            StartNextLevel();
                        }

                        // Shoot bullets
                        if (Raylib.IsKeyDown(KeyboardKey.Space)) player.Shoot(bullets, bulletTexture);

                        // Draw game objects
                        player.Draw();
                        enemy.Draw();
                        foreach (var bullet in bullets) bullet.Draw();
                        foreach (var enemyBullet in enemyBullets) enemyBullet.Draw();
                        foreach (var asteroid in asteroids) asteroid.Draw();

                        // Display the current level
                        Raylib.DrawText($"Level: {level}", 10, 10, 20, Color.White);

                        break;

                    case GameState.Paused:
                        // Draw the paused overlay
                        Raylib.DrawText("PAUSED", screenWidth / 2 - 80, screenHeight / 2 - 60, 50, Color.Yellow);
                        Raylib.DrawText("Press 'P' to Resume", screenWidth / 2 - 180, screenHeight / 2, 30, Color.White);
                        Raylib.DrawText("Press 'M' for Main Menu", screenWidth / 2 - 150, screenHeight / 2 + 40, 30, Color.White);

                        if (Raylib.IsKeyPressed(KeyboardKey.P) || Raylib.IsKeyPressed(KeyboardKey.Escape))
                        {
                            currentState = GameState.Playing;
                        }
                        else if (Raylib.IsKeyPressed(KeyboardKey.M))
                        {
                            currentState = GameState.MainMenu;
                        }
                        break;

                    case GameState.GameOver:
                        Raylib.DrawText("Game Over!", screenWidth / 2 - 100, screenHeight / 2 - 50, 40, Color.Red);
                        Raylib.DrawText("Press 'R' to Restart", screenWidth / 2 - 150, screenHeight / 2, 30, Color.White);
                        Raylib.DrawText("Press 'M' for Main Menu", screenWidth / 2 - 150, screenHeight / 2 + 40, 30, Color.White);

                        if (Raylib.IsKeyPressed(KeyboardKey.R))
                        {
                            // Reuse ResetGame to restart cleanly
                            ResetGame();
                            currentState = GameState.Playing;
                        }
                        else if (Raylib.IsKeyPressed(KeyboardKey.M))
                        {
                            currentState = GameState.MainMenu;
                        }
                        break;
                }

                Raylib.EndDrawing();
            }

            // Clean shutdown: unload resources and then close the window once we are outside BeginDrawing/EndDrawing
            UnloadTextures();
            Raylib.CloseWindow();
        }

        // ... rest of Program.cs remains unchanged ...
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

            // Only split if it's not already the smallest size
            if (asteroid.Size != AsteroidSize.Small)
            {
                // Enum is backed by int; cast to get the next smaller size
                AsteroidSize nextSize = (AsteroidSize)((int)asteroid.Size - 1);

                asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, asteroid.Velocity * 0.8f, nextSize));
                asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, asteroid.Velocity * -0.8f, nextSize));
            }
        }


        // Centralized reset used for starting/restarting the game
        static void ResetGame(int initialAsteroidCount = 5)
        {
            level = 1; // Reset the level

            player = new Player(screenWidth / 2, screenHeight / 2, playerTexture);
            asteroids = CreateAsteroids(initialAsteroidCount, asteroidTexture, player.Position, 150);
            bullets = new List<Bullet>();
            enemyBullets = new List<Bullet>();
            enemy = new Enemy(enemyTexture, enemyBulletTexture, new Vector2(100, 100));
        }

        // Start next level without ref parameters — uses Program-level fields
        static void StartNextLevel()
        {
            // Reposition player and increase asteroid count according to level
            player = new Player(screenWidth / 2, screenHeight / 2, playerTexture);
            asteroids = CreateAsteroids(5 + level, asteroidTexture, player.Position, 150); // Increase asteroid count
            bullets.Clear();
            enemyBullets.Clear();
            enemy = new Enemy(enemyTexture, enemyBulletTexture, new Vector2(100 + level * 10, 100 + level * 10)); // Slightly move the enemy
        }

        static void CheckPlayerCollisions(Player player, List<Asteroid> asteroids, List<Bullet> enemyBullets)
        {
            foreach (var asteroid in asteroids)
                if (player.CollidesWith(asteroid)) { currentState = GameState.GameOver; return; }

            foreach (var enemyBullet in enemyBullets)
                if (player.CollidesWith(enemyBullet)) { currentState = GameState.GameOver; return; }
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