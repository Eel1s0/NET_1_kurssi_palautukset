/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Asteroids.Components;
using RayGuiCreator;
using Raylib_cs;

namespace Asteroids
{
    enum GameState
    {
        MainMenu,
        Playing,
        GameOver,
        Settings,
        Paused
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

        public static int level = 1;
        public static Random random = new Random();
        public static Stack<GameState> gameStates = new Stack<GameState>();
        public static MainMenu mainMenu;
        public static SettingsMenu settingsMenu;
        public static PauseMenu pauseMenu;

        public static Player player;
        public static List<Asteroid> asteroids;
        public static List<Bullet> bullets;
        public static List<Bullet> enemyBullets;
        public static Enemy enemy;

        public static bool IsDebugMode = false;

        /// <summary>
        /// The main entry point for the application. Initializes the game window, loads resources, and runs the main game loop.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args)
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Asteroids");
            Raylib.SetTargetFPS(60);
            Raylib.SetExitKey(KeyboardKey.Null);

            LoadTextures();

            mainMenu = new MainMenu();
            settingsMenu = new SettingsMenu();
            pauseMenu = new PauseMenu();

            // Simplified state transition logic using helper methods
            mainMenu.StartGame += () => ChangeState(GameState.Playing, true);
            mainMenu.OpenSettings += () => PushState(GameState.Settings);
            mainMenu.ExitGame += () => PopState();
            settingsMenu.Back += () => PopState();
            pauseMenu.ResumeGame += () => PopState();
            pauseMenu.OpenSettings += () => PushState(GameState.Settings);
            pauseMenu.GoToMainMenu += () => ChangeState(GameState.MainMenu);

            PushState(GameState.MainMenu);
            SetupLevel(true);

            while (!Raylib.WindowShouldClose() && gameStates.Count > 0)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.F3))
                {
                    IsDebugMode = !IsDebugMode;
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.DrawTexturePro(backgroundTexture, new Rectangle(0, 0, backgroundTexture.Width, backgroundTexture.Height), new Rectangle(0, 0, screenWidth, screenHeight), Vector2.Zero, 0, Color.White);

                GameState currentState = gameStates.Peek();
                switch (currentState)
                {
                    case GameState.MainMenu:
                        mainMenu.Update();
                        mainMenu.Draw(screenWidth, screenHeight);
                        break;
                    case GameState.Settings:
                        settingsMenu.Update();
                        settingsMenu.Draw(screenWidth, screenHeight);
                        break;
                    case GameState.Playing:
                        UpdatePlayingState();
                        DrawPlayingState();
                        break;
                    case GameState.Paused:
                        DrawPausedState();
                        break;
                    case GameState.GameOver:
                        DrawGameOverState();
                        HandleGameOverInput();
                        break;
                }

                Raylib.EndDrawing();
            }

            UnloadTextures();
            Raylib.CloseWindow();
        }

        #region State Management

        /// <summary>
        /// Pushes a new state onto the game state stack.
        /// </summary>
        /// <param name="state">The new state to add.</param>
        static void PushState(GameState state)
        {
            gameStates.Push(state);
        }

        /// <summary>
        /// Pops the current state from the game state stack.
        /// </summary>
        static void PopState()
        {
            if (gameStates.Count > 0)
            {
                gameStates.Pop();
            }
        }

        /// <summary>
        /// Clears the current state stack and sets a new root state.
        /// </summary>
        /// <param name="state">The new root state.</param>
        /// <param name="isNewGame">Indicates if a new game should be set up.</param>
        static void ChangeState(GameState state, bool isNewGame = false)
        {
            while (gameStates.Count > 0)
            {
                gameStates.Pop();
            }
            gameStates.Push(state);
            if (isNewGame)
            {
                SetupLevel(true);
            }
        }

        #endregion

        /// <summary>
        /// Updates the game logic while in the 'Playing' state.
        /// </summary>
        static void UpdatePlayingState()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.P) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                PushState(GameState.Paused);
                return;
            }

            player.Update();
            enemy.Update(enemyBullets);
            foreach (var bullet in bullets) bullet.Update();
            foreach (var enemyBullet in enemyBullets) enemyBullet.Update();

            bullets.RemoveAll(b => !b.IsOnScreen() || b.IsExpired());
            enemyBullets.RemoveAll(b => !b.IsOnScreen() || b.IsExpired());
            foreach (var asteroid in asteroids) asteroid.Update();

            CheckPlayerCollisions();
            if (gameStates.Peek() == GameState.GameOver) return;

            CheckBulletCollisions();

            if (asteroids.Count == 0 && enemy.IsDestroyed)
            {
                level++;
                SetupLevel(false);
            }

            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                player.Shoot(bullets, bulletTexture);
            }
        }

        /// <summary>
        /// Draws all game objects while in the 'Playing' state.
        /// </summary>
        static void DrawPlayingState()
        {
            player.Draw();
            enemy.Draw();
            foreach (var bullet in bullets) bullet.Draw();
            foreach (var enemyBullet in enemyBullets) enemyBullet.Draw();
            foreach (var asteroid in asteroids) asteroid.Draw();
            Raylib.DrawText($"Level: {level}", 10, 10, 20, Color.White);
        }

        /// <summary>
        /// Draws the screen when the game is paused, showing an overlay.
        /// </summary>
        static void DrawPausedState()
        {
            DrawPlayingState();
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 150));
            pauseMenu.Update();
            pauseMenu.Draw(screenWidth, screenHeight);
        }

        /// <summary>
        /// Draws the 'Game Over' screen.
        /// </summary>
        static void DrawGameOverState()
        {
            DrawPlayingState();
            Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 150));
            DrawCenteredText("Game Over!", screenHeight / 2 - 50, 40, Color.Red);
            DrawCenteredText("Press 'R' to Restart", screenHeight / 2, 30, Color.White);
            DrawCenteredText("Press 'M' for Main Menu", screenHeight / 2 + 40, 30, Color.White);
        }

        /// <summary>
        /// Handles user input on the 'Game Over' screen.
        /// </summary>
        static void HandleGameOverInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                PopState(); // Pop GameOver, return to Playing
                SetupLevel(true);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.M))
            {
                ChangeState(GameState.MainMenu);
            }
        }

        /// <summary>
        /// A utility function to draw text centered horizontally on the screen.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="y">The Y-coordinate for the text position.</param>
        /// <param name="fontSize">The font size of the text.</param>
        /// <param name="color">The color of the text.</param>
        static void DrawCenteredText(string text, int y, int fontSize, Color color)
        {
            int textWidth = Raylib.MeasureText(text, fontSize);
            Raylib.DrawText(text, screenWidth / 2 - textWidth / 2, y, fontSize, color);
        }

        /// <summary>
        /// Loads all necessary game textures from files.
        /// </summary>
        static void LoadTextures()
        {
            Directory.CreateDirectory(ResourceManager.ImagesBasePath);
            playerTexture = ResourceManager.LoadTexture("player.png");
            asteroidTexture = ResourceManager.LoadTexture("asteroid.png");
            bulletTexture = ResourceManager.LoadTexture("bullet.png");
            enemyTexture = ResourceManager.LoadTexture("enemy.png");
            enemyBulletTexture = ResourceManager.LoadTexture("enemybullet.png");
            backgroundTexture = ResourceManager.LoadTexture("background.png");
        }

        /// <summary>
        /// Unloads all loaded textures to free up memory.
        /// </summary>
        static void UnloadTextures()
        {
            ResourceManager.UnloadAll();
        }

        /// <summary>
        /// Creates a list of asteroids with random positions and velocities, avoiding the player's immediate vicinity.
        /// </summary>
        /// <param name="count">The number of asteroids to create.</param>
        /// <param name="texture">The texture to use for the asteroids.</param>
        /// <param name="playerPosition">The player's current position.</param>
        /// <param name="safeZoneRadius">The radius around the player where asteroids should not spawn.</param>
        /// <returns>A list of newly created asteroids.</returns>
        static List<Asteroid> CreateAsteroids(int count, Texture2D texture, Vector2 playerPosition, float safeZoneRadius)
        {
            var createdAsteroids = new List<Asteroid>();
            const float minSpeed = 40f;
            const float maxSpeed = 140f;

            for (int i = 0; i < count; i++)
            {
                Vector2 position;
                do
                {
                    position = new Vector2(random.Next(screenWidth), random.Next(screenHeight));
                } while (IsWithinSafeZone(position, playerPosition, safeZoneRadius));

                float angle = (float)(random.NextDouble() * Math.PI * 2.0);
                float speed = minSpeed + (float)random.NextDouble() * (maxSpeed - minSpeed);
                Vector2 velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * speed;
                createdAsteroids.Add(new Asteroid(texture, position, velocity, AsteroidSize.Large));
            }
            return createdAsteroids;
        }

        /// <summary>
        /// Checks if a position is within a specified radius of the player's position.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="playerPosition">The player's position.</param>
        /// <param name="safeZoneRadius">The radius of the safe zone.</param>
        /// <returns>True if the position is within the safe zone, otherwise false.</returns>
        static bool IsWithinSafeZone(Vector2 position, Vector2 playerPosition, float safeZoneRadius)
        {
            return Vector2.Distance(position, playerPosition) < safeZoneRadius;
        }

        /// <summary>
        /// Splits a large or medium asteroid into two smaller ones upon destruction.
        /// </summary>
        /// <param name="asteroid">The asteroid to split.</param>
        static void SplitAsteroid(Asteroid asteroid)
        {
            if (asteroid.Size == AsteroidSize.Small) return;

            AsteroidSize nextSize = asteroid.Size + 1;
            var baseVel = asteroid.Velocity;
            if (baseVel.Length() < 0.1f) baseVel = new Vector2(1f, 0f);

            float spreadMin = 20f * Raylib.DEG2RAD;
            float spreadMax = 60f * Raylib.DEG2RAD;
            float a1 = (float)(random.NextDouble() * (spreadMax - spreadMin)) + spreadMin;
            float a2 = -(float)((random.NextDouble() * (spreadMax - spreadMin)) + spreadMin);

            Vector2 v1 = Raymath.Vector2Rotate(baseVel, a1) * 0.8f;
            Vector2 v2 = Raymath.Vector2Rotate(baseVel, a2) * 0.8f;

            asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, v1, nextSize));
            asteroids.Add(new Asteroid(asteroid.Texture, asteroid.Position, v2, nextSize));
        }

        /// <summary>
        /// Sets up the game objects for the current level.
        /// </summary>
        /// <param name="isNewGame">True if starting a new game, which resets the level to 1.</param>
        static void SetupLevel(bool isNewGame)
        {
            if (isNewGame)
            {
                level = 1;
            }
            player = new Player(screenWidth / 2, screenHeight / 2, playerTexture);
            asteroids = CreateAsteroids(4 + level, asteroidTexture, player.Position, 150);
            bullets = new List<Bullet>();
            enemyBullets = new List<Bullet>();
            enemy = new Enemy(enemyTexture, enemyBulletTexture, new Vector2(100 + (level - 1) * 10, 100 + (level - 1) * 10));
        }

        /// <summary>
        /// Checks for collisions between the player and asteroids or enemy bullets.
        /// </summary>
        static void CheckPlayerCollisions()
        {
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                if (player.CollidesWith(asteroids[i]))
                {
                    PushState(GameState.GameOver);
                    return;
                }
            }
            for (int i = enemyBullets.Count - 1; i >= 0; i--)
            {
                if (player.CollidesWith(enemyBullets[i]))
                {
                    PushState(GameState.GameOver);
                    return;
                }
            }
        }

        /// <summary>
        /// Checks for collisions between player bullets and asteroids or the enemy.
        /// </summary>
        static void CheckBulletCollisions()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bool bulletRemoved = false;
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (bullets[i].CollidesWith(asteroids[j]))
                    {
                        SplitAsteroid(asteroids[j]);
                        asteroids.RemoveAt(j);
                        bullets.RemoveAt(i);
                        bulletRemoved = true;
                        break;
                    }
                }
                if (bulletRemoved) continue;

                if (!enemy.IsDestroyed && bullets[i].CollidesWith(enemy))
                {
                    enemy.Destroy();
                    bullets.RemoveAt(i);
                }
            }
        }
    }
}