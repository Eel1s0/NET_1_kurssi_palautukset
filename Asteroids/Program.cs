using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroids
{
    class Program
    {
        public static int screenWidth = 800;
        public static int screenHeight = 600;
        public static Vector2 gravity = new Vector2(0, 0.1f);

        static void Main(string[] args)
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Asteroids Game");
            Raylib.SetTargetFPS(60);

            Player player = new Player(screenWidth / 2, screenHeight / 2);
            Asteroid[] asteroids = new Asteroid[10];
            for (int i = 0; i < asteroids.Length; i++)
            {
                asteroids[i] = new Asteroid();
            }

            while (!Raylib.WindowShouldClose())
            {
                player.Update();
                foreach (var asteroid in asteroids)
                {
                    asteroid.Update();
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                player.Draw();
                foreach (var asteroid in asteroids)
                {
                    asteroid.Draw();
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }

    class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Speed;

        public Player(float x, float y)
        {
            Position = new Vector2(x, y);
            Velocity = new Vector2(0, 0);
            Speed = 5.0f;
        }

        public void Update()
        {
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                Velocity.Y = -Speed;
            }
            else if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                Velocity.Y = Speed;
            }
            else
            {
                Velocity.Y = Program.gravity.Y;
            }

            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                Velocity.X = -Speed;
            }
            else if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                Velocity.X = Speed;
            }
            else
            {
                Velocity.X = 0;
            }

            Position.X += Velocity.X;
            Position.Y += Velocity.Y;

            WrapPosition();
        }

        public void Draw()
        {
            Raylib.DrawCircle((int)Position.X, (int)Position.Y, 10, Color.White);
        }

        private void WrapPosition()
        {
            if (Position.X < 0) Position.X = Program.screenWidth;
            else if (Position.X > Program.screenWidth) Position.X = 0;

            if (Position.Y < 0) Position.Y = Program.screenHeight;
            else if (Position.Y > Program.screenHeight) Position.Y = 0;
        }
    }

    class Asteroid
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Radius;
        private static Random random = new Random();

        public Asteroid()
        {
            Position = new Vector2(random.Next(Program.screenWidth), random.Next(Program.screenHeight));
            Velocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
            Radius = 20;
        }

        public void Update()
        {
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;

            WrapPosition();
        }

        public void Draw()
        {
            Raylib.DrawCircle((int)Position.X, (int)Position.Y, Radius, Color.Gray);
        }

        private void WrapPosition()
        {
            if (Position.X < 0) Position.X = Program.screenWidth;
            else if (Position.X > Program.screenWidth) Position.X = 0;

            if (Position.Y < 0) Position.Y = Program.screenHeight;
            else if (Position.Y > Program.screenHeight) Position.Y = 0;
        }
    }
}