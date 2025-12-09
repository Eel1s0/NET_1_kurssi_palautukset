namespace CaveShooter.GameStates
{
    /// <summary>
    /// Settings state for adjusting game options like volume.
    /// </summary>
    public class SettingsState : IGameState
    {
        #region Private Fields

        private readonly SettingsMenu _settingsMenu;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the settings state and wires up menu events.
        /// </summary>
        /// <param name="game">Reference to the main game instance.</param>
        public SettingsState(Game game)
        {
            _settingsMenu = new SettingsMenu();
            _settingsMenu.Back += () => game.GoToPreviousState();
        }

        #endregion

        #region IGameState Implementation

        /// <summary>
        /// Updates the settings menu UI.
        /// </summary>
        public void Update(Game game)
        {
            _settingsMenu.Update();
        }

        /// <summary>
        /// Renders gameplay background (if coming from gameplay/pause) with settings overlay.
        /// </summary>
        public void Draw(Game game)
        {
            // Draw the game world if coming from pause or gameplay
            if (game.PreviousState is GameplayState || game.PreviousState is PausedState)
            {
                game.DrawGameplay();
            }
            _settingsMenu.Draw(Game.ScreenWidth, Game.ScreenHeight);
        }

        #endregion
    }
}