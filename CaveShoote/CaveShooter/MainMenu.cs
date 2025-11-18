/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using RayGuiCreator;
using System;

namespace CaveShooter
{
    public class MainMenu
    {
        private MenuCreator mc;
        public event Action? StartGame;
        public event Action? OpenSettings;
        public event Action? ExitGame;

        public MainMenu(int screenWidth)
        {
            // layout: centered menu
            mc = new MenuCreator(screenWidth / 2 - 150, 180, 36, 300);
        }

        public void Update()
        {
            // nothing to do here for keyboard navigation — MenuCreator handles input as Draw is called,
            // but keep Update for parity with the main loop
        }

        public void Draw(int screenWidth, int screenHeight)
        {
            // Recreate MenuCreator each frame with correct Y origin so layout stays consistent
            mc = new MenuCreator(screenWidth / 2 - 150, 180, 36, 300);

            mc.Label("Cave Shooter");

            if (mc.Button("Start Game"))
            {
                StartGame?.Invoke();
            }

            if (mc.Button("Settings"))
            {
                OpenSettings?.Invoke();
            }

            if (mc.Button("Exit"))
            {
                ExitGame?.Invoke();
            }

            mc.EndMenu();
        }
    }
}