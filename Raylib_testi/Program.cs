using Raylib_cs;
using System.Numerics;

namespace Raylib_testi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize points
            Vector2 A = new Vector2(800 / 2, 0);
            Vector2 B = new Vector2(0, 800 / 2);
            Vector2 C = new Vector2(800, 800 * 3 / 4);

            // Direction vectors
            Vector2 directionA = new Vector2(1, 1); // Right and down
            Vector2 directionB = new Vector2(1, -1); // Right and up
            Vector2 directionC = new Vector2(-1, 1); // Left and down

            // Speed of movement
            float speed = 200.0f; // Pixels per second

            Raylib.InitWindow(800, 800, "Raylib_testi");

            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime(); // Time per frame

                // Update points
                A += directionA * speed * deltaTime;
                B += directionB * speed * deltaTime;
                C += directionC * speed * deltaTime;

                // AI KOODI
                if (A.X < 0 || A.X > 800) directionA.X *= -1;
                if (A.Y < 0 || A.Y > 800) directionA.Y *= -1;

                if (B.X < 0 || B.X > 800) directionB.X *= -1;
                if (B.Y < 0 || B.Y > 800) directionB.Y *= -1;

                if (C.X < 0 || C.X > 800) directionC.X *= -1;
                if (C.Y < 0 || C.Y > 800) directionC.Y *= -1;
                // AI KOODI LOPPUU

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw lines between points
                Raylib.DrawLineV(A, B, Color.Green);
                Raylib.DrawLineV(B, C, Color.Yellow);
                Raylib.DrawLineV(C, A, Color.SkyBlue);

                // Draw text
                Raylib.DrawText("WAAAA", 220, 60, 32, Color.White);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
