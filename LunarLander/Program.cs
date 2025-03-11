// Lander.cs
using Raylib_cs;
using System;
using System.Numerics;

namespace LunarLander
{
    class Lander
    {
        const int screenWidth = 800;
        const int screenHeight = 600;
        const float gravity = 1.3f;
        const int landingZoneHeight = 550;

        Ship ship;
        bool gameRunning = true;
        string message = "";

        static void Main()
        {
            new Lander().Run();
        }

        void Run()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Lunar Lander");
            Raylib.SetTargetFPS(60);

            ship = new Ship(new Vector2(screenWidth / 2, 100));

            while (!Raylib.WindowShouldClose())
            {
                if (gameRunning)
                {
                    Update();
                }

                Draw();
            }

            Raylib.CloseWindow();
        }

        void Update()
        {
            ship.Update(gravity);

            // Tarkista laskeutuminen
            if (ship.Position.Y >= landingZoneHeight)
            {
                gameRunning = false;
                if (ship.Velocity.Y < 3)
                {
                    message = "Onnistunut laskeutuminen!";
                }
                else
                {
                    message = "Alus tuhoutui!";
                }
                ship.Velocity = Vector2.Zero;
            }
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Piirrä laskeutumisalusta
            Raylib.DrawRectangle(200, landingZoneHeight, 400, 10, Color.Green);

            // Piirrä alus
            ship.Draw();

            // Polttoaineen määrä
            Raylib.DrawText($"Polttoaine: {ship.Fuel:F1}", 10, 10, 20, Color.White);

            // Näytä voittoviesti
            if (!gameRunning)
            {
                Raylib.DrawText(message, screenWidth / 2 - 150, screenHeight / 2, 30, Color.Yellow);
            }

            Raylib.EndDrawing();
        }
    }

    // Ship.cs
    class Ship
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Fuel = 100f;
        const float Thrust = -0.1f;
        const float FuelConsumption = 10f;

        public Ship(Vector2 startPosition)
        {
            Position = startPosition;
            Velocity = Vector2.Zero;
        }

        public void Update(float gravity)
        {
            if (Fuel > 0 && Raylib.IsKeyDown(KeyboardKey.Space))
            {
                Velocity.Y += Thrust;
                Fuel -= FuelConsumption * Raylib.GetFrameTime();
            }

            // Lisää painovoima
            Velocity.Y += gravity * Raylib.GetFrameTime();
            Position += Velocity;
        }

        public void Draw()
        {
            // Piirrä kolmio (alus)
            Vector2 p1 = new Vector2(Position.X, Position.Y - 10);
            Vector2 p2 = new Vector2(Position.X - 10, Position.Y + 10);
            Vector2 p3 = new Vector2(Position.X + 10, Position.Y + 10);

            Raylib.DrawTriangle(p1, p2, p3, Color.White);

            // Piirrä moottorin liekki, jos moottori on päällä
            if (Fuel > 0 && Raylib.IsKeyDown(KeyboardKey.Space))
            {
                Vector2 flame1 = new Vector2(Position.X, Position.Y + 15);
                Vector2 flame2 = new Vector2(Position.X - 5, Position.Y + 10);
                Vector2 flame3 = new Vector2(Position.X + 5, Position.Y + 10);
                Raylib.DrawTriangle(flame1, flame2, flame3, Color.Orange);
            }
        }
    }
}
