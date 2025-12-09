using Raylib_cs;

namespace CaveShooter.GameStates
{
    /// <summary>
    /// Main menu state displaying game title and navigation options.
    /// </summary>
    public class MainMenuState : IGameState
    {
        #region Private Fields

        private readonly MainMenu _mainMenu;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the main menu state and wires up menu events.
        /// </summary>
        /// <param name="game">Reference to the main game instance.</param>
        public MainMenuState(Game game)
        {
            _mainMenu = new MainMenu(Game.ScreenWidth);
            _mainMenu.StartGame += () => game.ChangeState(game.GameplayState);
            _mainMenu.OpenSettings += () => game.ChangeState(game.SettingsState);
            _mainMenu.ExitGame += () => Raylib.CloseWindow();
        }

        #endregion

        #region IGameState Implementation

        /// <summary>
        /// Updates the main menu UI.
        /// </summary>
        public void Update(Game game)
        {
            _mainMenu.Update();
        }

        /// <summary>
        /// Renders the main menu.
        /// </summary>
        public void Draw(Game game)
        {
            _mainMenu.Draw(Game.ScreenWidth, Game.ScreenHeight);
        }

        #endregion
    }
}