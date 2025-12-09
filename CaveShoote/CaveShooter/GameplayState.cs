using Raylib_cs;

namespace CaveShooter.GameStates
{
    public class GameplayState : IGameState
    {
        public void Update(Game game)
        {
            float deltaTime = Raylib.GetFrameTime();
            game.Players[0].Update(deltaTime, game.Map, game.BulletManager);
            game.Players[1].Update(deltaTime, game.Map, game.BulletManager);
            game.BulletManager.Update(deltaTime, game.Map);

            if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
            {
                game.ChangeState(game.PausedState);
            }
        }

        public void Draw(Game game)
        {
            game.DrawGameplay();
        }
    }
}