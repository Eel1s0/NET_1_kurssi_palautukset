/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using RayGuiCreator;
using System;

namespace CaveShooter
{
    public class PauseMenu
    {
        private MenuCreator mc;
        public event Action? ResumeGame;
        public event Action? OpenSettings;
        public event Action? GoToMainMenu;

        public PauseMenu(int screenWidth)
        {
            mc = new MenuCreator(screenWidth / 2 - 150, 180, 36, 300);
        }

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
            {
                ResumeGame?.Invoke();
            }
        }

        public void Draw(int screenWidth, int screenHeight)
        {
            mc = new MenuCreator(screenWidth / 2 - 150, 180, 36, 300);

            mc.Label("PAUSED");

            if (mc.Button("Resume"))
            {
                ResumeGame?.Invoke();
            }

            if (mc.Button("Settings"))
            {
                OpenSettings?.Invoke();
            }

            if (mc.Button("Main Menu"))
            {
                GoToMainMenu?.Invoke();
            }

            mc.EndMenu();
        }
    }
}