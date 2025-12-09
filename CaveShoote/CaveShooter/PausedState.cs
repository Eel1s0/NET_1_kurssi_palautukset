namespace CaveShooter.GameStates
{
    /// <summary>
    /// Paused game state showing pause menu over the gameplay.
    /// </summary>
    public class PausedState : IGameState
    {
        #region Private Fields

        private readonly PauseMenu _pauseMenu;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the paused state and wires up menu events.
        /// </summary>
        /// <param name="game">Reference to the main game instance.</param>
        public PausedState(Game game)
        {
            _pauseMenu = new PauseMenu(Game.ScreenWidth);
            _pauseMenu.ResumeGame += () => game.ChangeState(game.GameplayState);
            _pauseMenu.OpenSettings += () => game.ChangeState(game.SettingsState);
            _pauseMenu.GoToMainMenu += () => game.ChangeState(game.MainMenuState);
        }

        #endregion

        #region IGameState Implementation

        /// <summary>
        /// Updates the pause menu UI.
        /// </summary>
        public void Update(Game game)
        {
            _pauseMenu.Update();
        }

        /// <summary>
        /// Renders the gameplay behind the pause menu overlay.
        /// </summary>
        public void Draw(Game game)
        {
            game.DrawGameplay();
            _pauseMenu.Draw(Game.ScreenWidth, Game.ScreenHeight);
        }

        #endregion
    }
}