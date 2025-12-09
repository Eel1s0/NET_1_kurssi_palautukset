namespace CaveShooter.GameStates
{
    /// <summary>
    /// Interface for game states in the state machine pattern.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Updates the state logic each frame.
        /// </summary>
        /// <param name="game">Reference to the main game instance.</param>
        void Update(Game game);

        /// <summary>
        /// Renders the state's visuals.
        /// </summary>
        /// <param name="game">Reference to the main game instance.</param>
        void Draw(Game game);
    }
}