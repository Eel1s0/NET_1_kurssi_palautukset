/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using RayGuiCreator;
using System;

namespace CaveShooter
{
    /// <summary>
    /// Pause menu UI with options to resume, open settings, or return to main menu.
    /// </summary>
    public class PauseMenu
    {
        #region Events

        public event Action? ResumeGame;
        public event Action? OpenSettings;
        public event Action? GoToMainMenu;

        #endregion

        #region Private Fields

        private MenuCreator mc;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pause menu centered on the screen.
        /// </summary>
        /// <param name="screenWidth">Screen width for centering.</param>
        public PauseMenu(int screenWidth)
        {
            mc = new MenuCreator(screenWidth / 2 - 150, 180, 36, 300);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks for escape/pause key to resume the game.
        /// </summary>
        public void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
            {
                ResumeGame?.Invoke();
            }
        }

        /// <summary>
        /// Renders the pause menu and handles button interactions.
        /// </summary>
        /// <param name="screenWidth">Screen width for layout.</param>
        /// <param name="screenHeight">Screen height for layout.</param>
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

        #endregion
    }
}