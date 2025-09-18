using Raylib_cs;
using RayGuiCreator;

namespace Asteroids
{
    public class SettingsMenu
    {
        private float soundVolume = 1.0f; // Range: 0.0f - 1.0f
        public float SoundVolume => soundVolume;

        private bool backRequested = false;

        public enum SettingsResult { None, Back }

        // Keep keyboard-based quick-exit (Escape) and return the Back result if requested by GUI
        public SettingsResult Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                return SettingsResult.Back;
            }

            if (backRequested)
            {
                backRequested = false;
                return SettingsResult.Back;
            }

            return SettingsResult.None;
        }

        // Use RayGuiCreator.MenuCreator's Slider and Button to draw and interact
        public void Draw(int screenWidth, int screenHeight)
        {
            // MenuCreator will handle layout and RayGui calls
            var mc = new MenuCreator(screenWidth / 2 - 150, 140, 36, 300);

            mc.Label("SETTINGS");

            // Slider shows 0%..100% and updates soundVolume directly
            mc.Slider("0%", "100%", ref soundVolume, 0.0f, 1.0f);

            // Apply volume immediately from SettingsMenu (RayGui doesn't manage global audio)
            Raylib.SetMasterVolume(soundVolume);

            // Back button — set a flag that Update() will pick up (safe even though Draw runs after Update)
            if (mc.Button("Back"))
            {
                backRequested = true;
            }

            mc.EndMenu();
        }
    }
}