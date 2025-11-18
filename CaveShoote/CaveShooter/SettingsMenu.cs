/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using RayGuiCreator;
using System;

namespace CaveShooter
{
    public class SettingsMenu
    {
        private float soundVolume = 1.0f;
        public float SoundVolume => soundVolume;

        public event Action? Back;

        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                Back?.Invoke();
            }
        }

        public void Draw(int screenWidth, int screenHeight)
        {
            var mc = new MenuCreator(screenWidth / 2 - 150, 140, 36, 300);
            mc.Label("SETTINGS");
            mc.Slider("0%", "100%", ref soundVolume, 0.0f, 1.0f);
            Raylib.SetMasterVolume(soundVolume);

            if (mc.Button("Back"))
            {
                Back?.Invoke();
            }
            mc.EndMenu();
        }
    }
}