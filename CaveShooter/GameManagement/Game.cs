using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using CaveShooter.Entities;
using CaveShooter.Weapons;
using CaveShooter.Menus;

namespace CaveShooter.GameManagement
{
    /// <summary>
    /// Game states for state management.
    /// </summary>
    public enum GameState
    {
        StartMenu,
        InGame,
        Paused,
        Scoreboard,
        GameOver
    }

    /// <summary>
    /// Main game class managing the game loop and states.
    /// </summary>
    public class Game
    {
        private const int SCREEN_WIDTH = 1200;
        private const int SCREEN_HEIGHT = 800;

        private GameState currentState;
        private List<Player> players = null!;
        private Map gameMap = null!;
        private BulletManager bulletManager = null!;
        private Camera gameCamera = null!;
        private StartMenu startMenu = null!;
        private int numberOfPlayers;

        public Game()
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "CaveShooter - Multiplayer Game");
            Raylib.SetTargetFPS(60);

            Initialize();
        }

        /// <summary>
        /// Initializes game components.
        /// </summary>
        private void Initialize()
        {
            currentState = GameState.StartMenu;
            players = new List<Player>();
            gameMap = new Map(SCREEN_WIDTH, SCREEN_HEIGHT);
            bulletManager = new BulletManager(SCREEN_WIDTH, SCREEN_HEIGHT);
            gameCamera = new Camera(SCREEN_WIDTH, SCREEN_HEIGHT);
            startMenu = new StartMenu();
            numberOfPlayers = 1; // Default
        }

        /// <summary>
        /// Starts a new game with specified number of players.
        /// </summary>
        public void StartNewGame(int playerCount)
        {
            numberOfPlayers = Math.Clamp(playerCount, 1, 4);
            players.Clear();
            bulletManager.Clear();

            // Define starting positions and colors for each player
            Vector2[] startPositions = new Vector2[]
            {
                new Vector2(SCREEN_WIDTH / 4, SCREEN_HEIGHT / 2),
                new Vector2(3 * SCREEN_WIDTH / 4, SCREEN_HEIGHT / 2),
                new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 4),
                new Vector2(SCREEN_WIDTH / 2, 3 * SCREEN_HEIGHT / 4)
            };

            Color[] playerColors = new Color[]
            {
                Color.Blue,
                Color.Green,
                Color.Purple,
                Color.Orange
            };

            // Create players
            for (int i = 0; i < numberOfPlayers; i++)
            {
                // Check if gamepad is available for this player
                bool useGamepad = Raylib.IsGamepadAvailable(i);
                Player player = new Player(i + 1, startPositions[i], playerColors[i], useGamepad, i);
                players.Add(player);
            }

            // Set camera to follow first player
            if (players.Count > 0)
            {
                gameCamera.SetTarget(players[0].Ship);
            }

            // Load map
            gameMap.LoadBasicCave();

            currentState = GameState.InGame;
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

                Update(deltaTime);
                Draw();
            }

            Cleanup();
        }

        /// <summary>
        /// Updates game state based on current state.
        /// </summary>
        private void Update(float deltaTime)
        {
            switch (currentState)
            {
                case GameState.StartMenu:
                    UpdateStartMenu();
                    break;
                case GameState.InGame:
                    UpdateInGame(deltaTime);
                    break;
                case GameState.Paused:
                    UpdatePaused();
                    break;
                case GameState.Scoreboard:
                    UpdateScoreboard();
                    break;
                case GameState.GameOver:
                    UpdateGameOver();
                    break;
            }
        }

        /// <summary>
        /// Updates start menu state.
        /// </summary>
        private void UpdateStartMenu()
        {
            // Simple menu - press number keys to select player count
            if (Raylib.IsKeyPressed(KeyboardKey.One))
            {
                StartNewGame(1);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Two))
            {
                StartNewGame(2);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Three))
            {
                StartNewGame(3);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Four))
            {
                StartNewGame(4);
            }
        }

        /// <summary>
        /// Updates in-game state.
        /// </summary>
        private void UpdateInGame(float deltaTime)
        {
            // Pause game
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                currentState = GameState.Paused;
                return;
            }

            // Update all players
            foreach (var player in players)
            {
                player.Update(deltaTime, bulletManager, SCREEN_WIDTH, SCREEN_HEIGHT);
            }

            // Update bullets
            bulletManager.Update(deltaTime);

            // Update camera
            gameCamera.Update(deltaTime);

            // Check collisions
            CheckCollisions();

            // Check if all players are dead
            bool anyAlive = false;
            foreach (var player in players)
            {
                if (player.IsActive && player.Ship.IsAlive)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                currentState = GameState.GameOver;
            }
        }

        /// <summary>
        /// Checks collisions between entities and walls.
        /// </summary>
        private void CheckCollisions()
        {
            // Check player-wall collisions
            foreach (var player in players)
            {
                if (player.IsActive && player.Ship.IsAlive)
                {
                    if (gameMap.CheckCircleCollision(player.Ship.Position, player.Ship.Radius))
                    {
                        player.LoseLife(new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2));
                    }
                }
            }

            // Check bullet-wall collisions
            foreach (var bullet in bulletManager.GetBullets())
            {
                if (bullet.IsActive && gameMap.CheckCircleCollision(bullet.Position, bullet.Radius))
                {
                    bullet.IsActive = false;
                }
            }
        }

        /// <summary>
        /// Updates paused state.
        /// </summary>
        private void UpdatePaused()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                currentState = GameState.InGame;
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Q))
            {
                currentState = GameState.StartMenu;
            }
        }

        /// <summary>
        /// Updates scoreboard state.
        /// </summary>
        private void UpdateScoreboard()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                currentState = GameState.StartMenu;
            }
        }

        /// <summary>
        /// Updates game over state.
        /// </summary>
        private void UpdateGameOver()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                currentState = GameState.Scoreboard;
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                currentState = GameState.StartMenu;
            }
        }

        /// <summary>
        /// Draws the game based on current state.
        /// </summary>
        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            switch (currentState)
            {
                case GameState.StartMenu:
                    DrawStartMenu();
                    break;
                case GameState.InGame:
                    DrawInGame();
                    break;
                case GameState.Paused:
                    DrawPaused();
                    break;
                case GameState.Scoreboard:
                    DrawScoreboard();
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            Raylib.EndDrawing();
        }

        /// <summary>
        /// Draws the start menu.
        /// </summary>
        private void DrawStartMenu()
        {
            startMenu.Draw(SCREEN_WIDTH, SCREEN_HEIGHT);
        }

        /// <summary>
        /// Draws the in-game state.
        /// </summary>
        private void DrawInGame()
        {
            // Use camera for world rendering
            gameCamera.BeginMode();

            // Draw map
            gameMap.Draw();

            // Draw bullets
            bulletManager.Draw();

            // Draw players
            foreach (var player in players)
            {
                player.Draw();
            }

            gameCamera.EndMode();

            // Draw UI (not affected by camera)
            DrawUI();
        }

        /// <summary>
        /// Draws the UI overlay.
        /// </summary>
        private void DrawUI()
        {
            int yOffset = 10;
            foreach (var player in players)
            {
                if (player.IsActive)
                {
                    string info = $"P{player.PlayerId} - Score: {player.Score} Lives: {player.Lives}";
                    Raylib.DrawText(info, 10, yOffset, 20, Color.White);
                    yOffset += 25;
                }
            }
        }

        /// <summary>
        /// Draws the paused state.
        /// </summary>
        private void DrawPaused()
        {
            DrawInGame();
            Raylib.DrawRectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, new Color(0, 0, 0, 150));
            DrawCenteredText("PAUSED", SCREEN_HEIGHT / 2 - 40, 60, Color.White);
            DrawCenteredText("Press ESC to Resume", SCREEN_HEIGHT / 2 + 20, 30, Color.White);
            DrawCenteredText("Press Q to Quit to Menu", SCREEN_HEIGHT / 2 + 55, 25, Color.Gray);
        }

        /// <summary>
        /// Draws the scoreboard.
        /// </summary>
        private void DrawScoreboard()
        {
            DrawCenteredText("SCOREBOARD", 100, 60, Color.Yellow);

            int yOffset = 200;
            // Sort players by score
            var sortedPlayers = new List<Player>(players);
            sortedPlayers.Sort((a, b) => b.Score.CompareTo(a.Score));

            foreach (var player in sortedPlayers)
            {
                string scoreText = $"Player {player.PlayerId}: {player.Score} points";
                DrawCenteredText(scoreText, yOffset, 40, Color.White);
                yOffset += 50;
            }

            DrawCenteredText("Press ENTER or ESC to return to menu", SCREEN_HEIGHT - 100, 25, Color.Gray);
        }

        /// <summary>
        /// Draws the game over state.
        /// </summary>
        private void DrawGameOver()
        {
            DrawInGame();
            Raylib.DrawRectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, new Color(0, 0, 0, 180));
            DrawCenteredText("GAME OVER", SCREEN_HEIGHT / 2 - 60, 70, Color.Red);
            DrawCenteredText("Press ENTER to see scores", SCREEN_HEIGHT / 2 + 20, 30, Color.White);
            DrawCenteredText("Press ESC to return to menu", SCREEN_HEIGHT / 2 + 55, 25, Color.Gray);
        }

        /// <summary>
        /// Draws centered text on screen.
        /// </summary>
        private void DrawCenteredText(string text, int y, int fontSize, Color color)
        {
            int textWidth = Raylib.MeasureText(text, fontSize);
            Raylib.DrawText(text, (SCREEN_WIDTH - textWidth) / 2, y, fontSize, color);
        }

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        private void Cleanup()
        {
            Raylib.CloseWindow();
        }
    }
}
