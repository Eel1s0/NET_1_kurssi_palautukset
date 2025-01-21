using Raylib_cs;
using System.Numerics;

namespace Raylib_testi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Vector2 A = new Vector2 (800/2,0);
            Vector2 B = new Vector2 (0,800/2);
            Vector2 C = new Vector2 (800,800*3/4);

            Vector2 directionA = new Vector2(1, 1); // Right and down
            Vector2 directionB = new Vector2(1, -1); // Right and up
            Vector2 directionC = new Vector2(-1, 1); // Left and down

            // Speed of movement
            float speed = 200.0f; // Pixels per second

            Raylib.InitWindow(800, 800, "Raylib_testi");
            while(Raylib.WindowShouldClose() == false)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);


                Raylib.DrawLineV(A, B, Color.Green);
                Raylib.DrawLineV(B, C, Color.Yellow);
                Raylib.DrawLineV(C, A, Color.SkyBlue);


                Raylib.DrawText("WAAAA", 220, 60, 32, Color.White);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
    }
}
