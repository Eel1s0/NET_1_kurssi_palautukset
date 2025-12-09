namespace CaveShooter.GameStates
{
    public class SettingsState : IGameState
    {
        private readonly SettingsMenu _settingsMenu;

        public SettingsState(Game game)
        {
            _settingsMenu = new SettingsMenu();
            _settingsMenu.Back += () => game.GoToPreviousState();
        }

        public void Update(Game game)
        {
            _settingsMenu.Update();
        }

        public void Draw(Game game)
        {
            // Draw the game world if coming from pause or gameplay
            if (game.PreviousState is GameplayState || game.PreviousState is PausedState)
            {
                game.DrawGameplay();
            }
            _settingsMenu.Draw(Game.ScreenWidth, Game.ScreenHeight);
        }
    }
}