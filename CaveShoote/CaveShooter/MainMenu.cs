/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using RayGuiCreator;
using System;

namespace CaveShooter
{
    /// <summary>
    /// Main menu UI with options to start game, open settings, or exit.
    /// </summary>
    public class MainMenu
    {
        #region Events

        public event Action? StartGame;
        public event Action? OpenSettings;
        public event Action? ExitGame;

        #endregion

        #region Private Fields

        private MenuCreator mc;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new main menu centered on the screen.
        /// </summary>
        /// <param name="screenWidth">Screen width for centering.</param>
        public MainMenu(int screenWidth)
        {
            mc = new MenuCreator(screenWidth / 2 - 150, 180, 36, 300);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates menu state (reserved for future input handling).
        /// </summary>
        public void Update()
        {
            // MenuCreator handles input during Draw
        }

        /// <summary>
        /// Renders the main menu and handles button interactions.
        /// </summary>
        /// <param name="screenWidth">Screen width for layout.</param>
        /// <param name="screenHeight">Screen height for layout.</param>
        public void Draw(int screenWidth, int screenHeight)
        {
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

        #endregion
    }
}