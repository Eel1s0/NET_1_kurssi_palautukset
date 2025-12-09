/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using RayGuiCreator;
using System;

namespace CaveShooter
{
    /// <summary>
    /// Settings menu UI for adjusting game options like volume.
    /// </summary>
    public class SettingsMenu
    {
        #region Events

        public event Action? Back;

        #endregion

        #region Properties

        public float SoundVolume => soundVolume;

        #endregion

        #region Private Fields

        private float soundVolume = 1.0f;

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks for escape key to go back.
        /// </summary>
        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                Back?.Invoke();
            }
        }

        /// <summary>
        /// Renders the settings menu with volume slider and back button.
        /// </summary>
        /// <param name="screenWidth">Screen width for layout.</param>
        /// <param name="screenHeight">Screen height for layout.</param>
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

        #endregion
    }
}