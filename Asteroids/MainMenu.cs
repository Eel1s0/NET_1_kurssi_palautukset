using Raylib_cs;
using System;
using System.Numerics;

namespace Asteroids
{
    public class MainMenu
    {
        private int selectedIndex = 0;
        private readonly string[] options = { "Start Game", "Settings", "Exit" };

        public enum MenuResult { None, StartGame, Settings, Exit }

        public MenuResult Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) selectedIndex = (selectedIndex + 1) % options.Length;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;

            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                switch (selectedIndex)
                {
                    case 0: return MenuResult.StartGame;
                    case 1: return MenuResult.Settings;
                    case 2: return MenuResult.Exit;
                }
            }
            return MenuResult.None;
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
