using Raylib_cs;
using System.Numerics;
using System;

namespace DVDLogo
{
    internal class Program
    {

        static void Main(string[] args)
        {
            int screen_width = 800;
            int screen_height = 800;
            Color bg_color = Color.Black;
            Raylib.InitWindow(screen_width, screen_height, "DVD");

            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();

            Vector2 position = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 direction = new Vector2(1, 1);
            float speed = 200.0f;

            string text = "DVD";
            int fontSize = 32;
            float spacing = 2.0f;

            Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, fontSize, spacing);

            Random random = new Random();
            Color textColor = RandomColor(random);

            while (!Raylib.WindowShouldClose())
            {
                float deltatime = Raylib.GetFrameTime();

                position += direction * speed * deltatime;
                // Tehty AI:n avulla
                if (position.X < 0 || position.X + textSize.X > screenWidth)
                {
                    direction.X *= -1;
                    position.X = Math.Clamp(position.X, 0, screenWidth - textSize.X);
                    textColor = RandomColor(random);
                    speed += 100.0f;
                }

                if (position.Y < 0 || position.Y + textSize.Y > screenHeight)
                {
                    direction.Y *= -1;
                    position.Y = Math.Clamp(position.Y, 0, screenWidth - textSize.Y);
                    textColor = RandomColor(random);
                    speed += 100.0f;

                }
                // AI koodi loppuu
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.Black);

                Raylib.DrawTextEx(Raylib.GetFontDefault(), text, position, fontSize, spacing, textColor);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();


        }

        static Color RandomColor(Random random) 
        {
            return new Color(random.Next(256), random.Next(256), random.Next(256), 255);
        }

    }
}
