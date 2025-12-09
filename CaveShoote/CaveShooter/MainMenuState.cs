using Raylib_cs;

namespace CaveShooter.GameStates
{
    public class MainMenuState : IGameState
    {
        private readonly MainMenu _mainMenu;

        public MainMenuState(Game game)
        {
            _mainMenu = new MainMenu(Game.ScreenWidth);
            _mainMenu.StartGame += () => game.ChangeState(game.GameplayState);
            _mainMenu.OpenSettings += () => game.ChangeState(game.SettingsState);
            _mainMenu.ExitGame += () => Raylib.CloseWindow();
        }

        public void Draw(Game game)
        {
            _mainMenu.Draw(Game.ScreenWidth, Game.ScreenHeight);
        }

        public void Update(Game game)
        {
            _mainMenu.Update();
        }
    }
}