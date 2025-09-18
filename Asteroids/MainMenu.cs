using Raylib_cs;
using System;
using System.Numerics;

namespace Asteroids
{
    public class MainMenu
    {
        private int selectedIndex = 0;
        private readonly string[] options = { "Start Game", "Settings", "Exit" };

        // Events for menu actions
        public event Action? StartGame;
        public event Action? OpenSettings;
        public event Action? ExitGame;

        // Now Update fires events instead of returning a MenuResult.
        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) selectedIndex = (selectedIndex + 1) % options.Length;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;

            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                switch (selectedIndex)
                {
                    case 0:
                        StartGame?.Invoke();
                        break;
                    case 1:
                        OpenSettings?.Invoke();
                        break;
                    case 2:
                        ExitGame?.Invoke();
                        break;
                }
            }
        }

        public void Draw(int screenWidth, int screenHeight)
        {
            Raylib.DrawText("ASTEROIDS", screenWidth / 2 - 100, 100, 40, Color.White);
            for (int i = 0; i < options.Length; i++)
            {
                var color = (i == selectedIndex) ? Color.Yellow : Color.White;
                Raylib.DrawText(options[i], screenWidth / 2 - 60, 250 + i * 40, 30, color);
            }
        }
    }
}