using Raylib_cs;

namespace CaveShooter.Menus
{
    /// <summary>
    /// Start menu for the game.
    /// </summary>
    public class StartMenu
    {
        public StartMenu()
        {
        }

        /// <summary>
        /// Draws the start menu.
        /// </summary>
        public void Draw(int screenWidth, int screenHeight)
        {
            // Title
            string title = "CAVESHOOTER";
            int titleSize = 80;
            int titleWidth = Raylib.MeasureText(title, titleSize);
            Raylib.DrawText(title, (screenWidth - titleWidth) / 2, 100, titleSize, Color.SkyBlue);

            // Subtitle
            string subtitle = "Multiplayer Cave Combat";
            int subtitleSize = 30;
            int subtitleWidth = Raylib.MeasureText(subtitle, subtitleSize);
            Raylib.DrawText(subtitle, (screenWidth - subtitleWidth) / 2, 200, subtitleSize, Color.White);

            // Menu options
            int yStart = 320;
            int spacing = 50;

            DrawMenuOption("Press 1 for 1 Player", yStart, screenWidth, Color.White);
            DrawMenuOption("Press 2 for 2 Players", yStart + spacing, screenWidth, Color.White);
            DrawMenuOption("Press 3 for 3 Players", yStart + spacing * 2, screenWidth, Color.White);
            DrawMenuOption("Press 4 for 4 Players", yStart + spacing * 3, screenWidth, Color.White);

            // Controls info
            string controlInfo = "Controls: Arrow Keys/WASD to move, Space to fire | Gamepad supported";
            int controlSize = 20;
            int controlWidth = Raylib.MeasureText(controlInfo, controlSize);
            Raylib.DrawText(controlInfo, (screenWidth - controlWidth) / 2, screenHeight - 80, controlSize, Color.Gray);

            // Additional player controls
            string multiInfo = "P2: Arrow Keys | P3: IJKL | P4: Numpad";
            int multiSize = 18;
            int multiWidth = Raylib.MeasureText(multiInfo, multiSize);
            Raylib.DrawText(multiInfo, (screenWidth - multiWidth) / 2, screenHeight - 50, multiSize, Color.DarkGray);
        }

        /// <summary>
        /// Draws a menu option centered on screen.
        /// </summary>
        private void DrawMenuOption(string text, int y, int screenWidth, Color color)
        {
            int fontSize = 35;
            int textWidth = Raylib.MeasureText(text, fontSize);
            Raylib.DrawText(text, (screenWidth - textWidth) / 2, y, fontSize, color);
        }
    }
}
