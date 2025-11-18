using Raylib_cs;
using System.Numerics;

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

    public enum GameState
    {
        MainMenu,
        GamePlay,
        Paused,
        Settings
    }

    public class Game
    {
        private const int ScreenWidth = 1280;
        private const int ScreenHeight = 720;

        private Map map;
        private Player[] players;
        private BulletManager bulletManager;
        private RenderTexture2D[] playerRenderTextures;

        private GameState currentState;
        private GameState previousState;
        private MainMenu mainMenu;
        private PauseMenu pauseMenu;
        private SettingsMenu settingsMenu;

        public Game()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Cave Shooter");
            Raylib.SetTargetFPS(60);

            map = new Map();
            bulletManager = new BulletManager();

            // Define input configurations for two players
            var player1Input = new InputConfig { Up = KeyboardKey.W, Down = KeyboardKey.S, Left = KeyboardKey.A, Right = KeyboardKey.D, Shoot = KeyboardKey.Space };
            var player2Input = new InputConfig { Up = KeyboardKey.Up, Down = KeyboardKey.Down, Left = KeyboardKey.Left, Right = KeyboardKey.Right, Shoot = KeyboardKey.RightControl };

            players = new Player[2];
            // Initialize players with the correct viewport dimensions for their cameras
            players[0] = new Player(new Vector2(40 * 1.5f, 40 * 1.5f), player1Input, ScreenWidth, ScreenHeight / 2f);
            players[1] = new Player(new Vector2(40 * 3.5f, 40 * 1.5f), player2Input, ScreenWidth, ScreenHeight / 2f);

            // Create render textures for each player's view
            playerRenderTextures = new RenderTexture2D[2];
            playerRenderTextures[0] = Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight / 2);
            playerRenderTextures[1] = Raylib.LoadRenderTexture(ScreenWidth, ScreenHeight / 2);

            // Initialize menus and game state
            currentState = GameState.MainMenu;
            mainMenu = new MainMenu(ScreenWidth);
            pauseMenu = new PauseMenu(ScreenWidth);
            settingsMenu = new SettingsMenu();

            // Wire up menu events
            mainMenu.StartGame += () => currentState = GameState.GamePlay;
            mainMenu.OpenSettings += () => { previousState = currentState; currentState = GameState.Settings; };
            mainMenu.ExitGame += () => Raylib.CloseWindow();

            pauseMenu.ResumeGame += () => currentState = GameState.GamePlay;
            pauseMenu.OpenSettings += () => { previousState = currentState; currentState = GameState.Settings; };
            pauseMenu.GoToMainMenu += () => currentState = GameState.MainMenu;

            settingsMenu.Back += () => currentState = previousState;
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                // Update and Draw are now handled by the state machine
                Update();
                Draw();
            }

            // Unload resources
            map.Unload();
            Raylib.UnloadRenderTexture(playerRenderTextures[0]);
            Raylib.UnloadRenderTexture(playerRenderTextures[1]);
            Raylib.CloseWindow();
        }

        private void Update()
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    mainMenu.Update();
                    break;
                case GameState.GamePlay:
                    float deltaTime = Raylib.GetFrameTime();
                    players[0].Update(deltaTime, map, bulletManager);
                    players[1].Update(deltaTime, map, bulletManager);
                    bulletManager.Update(deltaTime, map);

                    if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
                    {
                        currentState = GameState.Paused;
                    }
                    break;
                case GameState.Paused:
                    pauseMenu.Update();
                    break;
                case GameState.Settings:
                    settingsMenu.Update();
                    break;
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            switch (currentState)
            {
                case GameState.MainMenu:
                    mainMenu.Draw(ScreenWidth, ScreenHeight);
                    break;
                case GameState.GamePlay:
                    DrawGamePlay();
                    break;
                case GameState.Paused:
                    DrawGamePlay(); // Draw the game world behind the pause menu
                    pauseMenu.Draw(ScreenWidth, ScreenHeight);
                    break;
                case GameState.Settings:
                    if (previousState == GameState.Paused)
                    {
                        DrawGamePlay(); // Draw game world if coming from pause
                    }
                    settingsMenu.Draw(ScreenWidth, ScreenHeight);
                    break;
            }

            Raylib.EndDrawing();
        }

        private void DrawGamePlay()
        {
            // --- Player 1 Rendering ---
            Raylib.BeginTextureMode(playerRenderTextures[0]);
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginMode2D(players[0].Camera.Instance);
            map.Draw();
            players[0].Draw();
            players[1].Draw(); // Draw other player
            bulletManager.Draw();
            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            // --- Player 2 Rendering ---
            Raylib.BeginTextureMode(playerRenderTextures[1]);
            Raylib.ClearBackground(Color.Black);
            Raylib.BeginMode2D(players[1].Camera.Instance);
            map.Draw();
            players[0].Draw(); // Draw other player
            players[1].Draw();
            bulletManager.Draw();
            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            // --- Main Drawing ---
            // Draw player 1's view on the top half
            Raylib.DrawTextureRec(playerRenderTextures[0].Texture, new Rectangle(0, 0, ScreenWidth, -ScreenHeight / 2f), new Vector2(0, 0), Color.White);
            // Draw player 2's view on the bottom half
            Raylib.DrawTextureRec(playerRenderTextures[1].Texture, new Rectangle(0, 0, ScreenWidth, -ScreenHeight / 2f), new Vector2(0, ScreenHeight / 2f), Color.White);

            // Draw a line to separate the screens
            Raylib.DrawLine(0, ScreenHeight / 2, ScreenWidth, ScreenHeight / 2, Color.White);

            // Draw UI
            Raylib.DrawText("Player 1", 20, 20, 20, Color.White);
            Raylib.DrawText("Player 2", 20, ScreenHeight / 2 + 20, 20, Color.White);
            Raylib.DrawFPS(ScreenWidth - 100, 20);
        }
    }
}