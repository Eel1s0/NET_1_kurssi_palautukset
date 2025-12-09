using Raylib_cs;
using System.Numerics;
using CaveShooter.GameStates;

namespace CaveShooter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }

    /// <summary>
    /// Main game class that manages the game window, state machine, players, and rendering.
    /// </summary>
    public class Game
    {
        #region Constants

        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;

        #endregion

        #region Game Objects

        public Map Map { get; private set; }
        public Player[] Players { get; private set; }
        public BulletManager BulletManager { get; private set; }

        #endregion

        #region State Management

        private IGameState _currentState;
        public IGameState PreviousState { get; private set; }
        public IGameState MainMenuState { get; private set; }
        public IGameState GameplayState { get; private set; }
        public IGameState PausedState { get; private set; }
        public IGameState SettingsState { get; private set; }

        #endregion

        #region Rendering

        private RenderTexture2D[] playerRenderTextures;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the game window, loads resources, creates players with input configs,
        /// sets up render textures for split-screen, and initializes all game states.
        /// </summary>
        public Game()
        {
            InitializeWindow();
            InitializeGameObjects();
            InitializePlayers();
            InitializeRenderTextures();
            InitializeStates();
        }

        private void InitializeWindow()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Cave Shooter");
            Raylib.SetTargetFPS(60);
        }

        private void InitializeGameObjects()
        {
            Map = new Map("picomap_cave.png");
            BulletManager = new BulletManager();
        }

        private void InitializePlayers()
        {
            var player1Input = new InputConfig
            {
                Up = KeyboardKey.W,
                Down = KeyboardKey.S,
                Left = KeyboardKey.A,
                Right = KeyboardKey.D,
                Shoot = KeyboardKey.Space
            };

            var player2Input = new InputConfig
            {
                Up = KeyboardKey.Up,
                Down = KeyboardKey.Down,
                Left = KeyboardKey.Left,
                Right = KeyboardKey.Right,
                Shoot = KeyboardKey.RightControl
            };

            Vector2 safeSpawn1 = FindSafeSpawnPosition(new Vector2(50, 50));
            Vector2 safeSpawn2 = FindSafeSpawnPosition(new Vector2(70, 50));

            Players = new Player[2];
            Players[0] = new Player(safeSpawn1, player1Input, ScreenWidth, ScreenHeight / 2f);
            Players[1] = new Player(safeSpawn2, player2Input, ScreenWidth, ScreenHeight / 2f);
        }

        private void InitializeRenderTextures()
        {
            playerRenderTextures = new RenderTexture2D[2];
            playerRenderTextures[0] = Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight / 2);
            playerRenderTextures[1] = Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight / 2);
        }

        private void InitializeStates()
        {
            MainMenuState = new MainMenuState(this);
            GameplayState = new GameplayState();
            PausedState = new PausedState(this);
            SettingsState = new SettingsState(this);

            _currentState = MainMenuState;
        }

        #endregion

        #region Game Loop

        /// <summary>
        /// Main game loop. Runs update and draw cycles until the window is closed,
        /// then unloads all resources.
        /// </summary>
        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Cleanup();
        }

        /// <summary>
        /// Delegates update logic to the current game state.
        /// </summary>
        private void Update()
        {
            _currentState.Update(this);
        }

        /// <summary>
        /// Clears the screen and delegates drawing to the current game state.
        /// </summary>
        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            _currentState.Draw(this);
            Raylib.EndDrawing();
        }

        private void Cleanup()
        {
            Map.Unload();
            Raylib.UnloadRenderTexture(playerRenderTextures[0]);
            Raylib.UnloadRenderTexture(playerRenderTextures[1]);
            Raylib.CloseWindow();
        }

        #endregion

        #region State Management Methods

        /// <summary>
        /// Transitions to a new game state, storing the current state as previous.
        /// </summary>
        /// <param name="newState">The state to transition to.</param>
        public void ChangeState(IGameState newState)
        {
            PreviousState = _currentState;
            _currentState = newState;
        }

        /// <summary>
        /// Returns to the previously active game state (e.g., unpausing).
        /// </summary>
        public void GoToPreviousState()
        {
            if (PreviousState != null)
            {
                _currentState = PreviousState;
            }
        }

        #endregion

        #region Rendering Methods

        /// <summary>
        /// Renders split-screen gameplay with each player's perspective in separate viewports,
        /// draws a dividing line, and overlays UI elements (health, FPS).
        /// </summary>
        public void DrawGameplay()
        {
            RenderPlayerViews();
            DrawSplitScreen();
            DrawUI();
        }

        private void RenderPlayerViews()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Raylib.BeginTextureMode(playerRenderTextures[i]);
                Raylib.ClearBackground(Color.Black);
                DrawScene(Players[i].Camera.Instance);
                Raylib.EndTextureMode();
            }
        }

        /// <summary>
        /// Renders the game world (map, players, bullets) using the specified camera.
        /// </summary>
        /// <param name="camera">The camera defining the view transform.</param>
        private void DrawScene(Camera2D camera)
        {
            Raylib.BeginMode2D(camera);
            Map.Draw();
            foreach (var player in Players)
            {
                player.Draw();
            }
            BulletManager.Draw();
            Raylib.EndMode2D();
        }

        private void DrawSplitScreen()
        {
            // Player 1's view on top half
            Raylib.DrawTextureRec(
                playerRenderTextures[0].Texture,
                new Rectangle(0, 0, ScreenWidth, -ScreenHeight / 2f),
                new Vector2(0, 0),
                Color.White);

            // Player 2's view on bottom half
            Raylib.DrawTextureRec(
                playerRenderTextures[1].Texture,
                new Rectangle(0, 0, ScreenWidth, -ScreenHeight / 2f),
                new Vector2(0, ScreenHeight / 2f),
                Color.White);

            // Dividing line
            Raylib.DrawLine(0, ScreenHeight / 2, ScreenWidth, ScreenHeight / 2, Color.White);
        }

        private void DrawUI()
        {
            Raylib.DrawText($"Player 1 Health: {Players[0].Ship.Health}", 20, 20, 20, Color.White);
            Raylib.DrawText($"Player 2 Health: {Players[1].Ship.Health}", 20, ScreenHeight / 2 + 20, 20, Color.White);
            Raylib.DrawFPS(ScreenWidth - 100, 20);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Finds a valid spawn position that doesn't collide with the map.
        /// Searches in expanding circles around the desired position if blocked.
        /// </summary>
        /// <param name="desired">The preferred spawn position.</param>
        /// <returns>A collision-free spawn position, or the original if none found.</returns>
        private Vector2 FindSafeSpawnPosition(Vector2 desired)
        {
            const float shipSize = 4f;
            Rectangle testRect = new Rectangle(desired.X - shipSize, desired.Y - shipSize, shipSize * 2, shipSize * 2);

            if (!Map.CheckCollision(testRect))
                return desired;

            for (int radius = 8; radius < 200; radius += 8)
            {
                for (float angle = 0; angle < 360; angle += 45)
                {
                    float rad = angle * MathF.PI / 180f;
                    Vector2 test = desired + new Vector2(MathF.Cos(rad) * radius, MathF.Sin(rad) * radius);
                    testRect = new Rectangle(test.X - shipSize, test.Y - shipSize, shipSize * 2, shipSize * 2);

                    if (!Map.CheckCollision(testRect))
                        return test;
                }
            }

            return desired;
        }

        #endregion
    }
}