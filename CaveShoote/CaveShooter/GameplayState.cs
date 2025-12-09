using Raylib_cs;

namespace CaveShooter.GameStates
{
    /// <summary>
    /// Active gameplay state handling player updates and rendering.
    /// </summary>
    public class GameplayState : IGameState
    {
        #region IGameState Implementation

        /// <summary>
        /// Updates all players, bullets, and checks for pause input.
        /// </summary>
        public void Update(Game game)
        {
            float deltaTime = Raylib.GetFrameTime();

            UpdatePlayers(game, deltaTime);
            game.BulletManager.Update(deltaTime, game.Map);

            CheckPauseInput(game);
        }

        /// <summary>
        /// Renders the split-screen gameplay view.
        /// </summary>
        public void Draw(Game game)
        {
            game.DrawGameplay();
        }

        #endregion

        #region Private Methods

        private void UpdatePlayers(Game game, float deltaTime)
        {
            game.Players[0].Update(deltaTime, game.Map, game.BulletManager);
            game.Players[1].Update(deltaTime, game.Map, game.BulletManager);
        }

        private void CheckPauseInput(Game game)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
            {
                game.ChangeState(game.PausedState);
            }
        }

        #endregion
    }
}