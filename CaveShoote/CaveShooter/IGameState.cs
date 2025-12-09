namespace CaveShooter.GameStates
{
    public interface IGameState
    {
        void Update(Game game);
        void Draw(Game game);
    }
}