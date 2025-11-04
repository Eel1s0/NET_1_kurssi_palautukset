using CaveShooter.GameManagement;

namespace CaveShooter
{
    /// <summary>
    /// Main entry point for the CaveShooter game.
    /// A 2-4 player multiplayer cave shooter game built with C# and Raylib.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }
}
