namespace CaveShooter.GameStates
{
    public class PausedState : IGameState
    {
        private readonly PauseMenu _pauseMenu;

        public PausedState(Game game)
        {
            _pauseMenu = new PauseMenu(Game.ScreenWidth);
            _pauseMenu.ResumeGame += () => game.ChangeState(game.GameplayState);
            _pauseMenu.OpenSettings += () => game.ChangeState(game.SettingsState);
            _pauseMenu.GoToMainMenu += () => game.ChangeState(game.MainMenuState);
        }

        public void Update(Game game)
        {
            _pauseMenu.Update();
        }

        public void Draw(Game game)
        {
            game.DrawGameplay(); // Draw the game world behind the pause menu
            _pauseMenu.Draw(Game.ScreenWidth, Game.ScreenHeight);
        }
    }
}