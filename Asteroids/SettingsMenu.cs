using Raylib_cs;

namespace Asteroids
{
    public class SettingsMenu
    {
        private int selectedIndex = 0;
        private readonly string[] options = { "Sound Volume", "Back" };
        public float SoundVolume { get; private set; } = 1.0f; // Range: 0.0f - 1.0f

        public enum SettingsResult { None, Back }

        public SettingsResult Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Down)) selectedIndex = (selectedIndex + 1) % options.Length;
            if (Raylib.IsKeyPressed(KeyboardKey.Up)) selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;

            if (selectedIndex == 0)
            {
                if (Raylib.IsKeyDown(KeyboardKey.Right)) SoundVolume = MathF.Min(1.0f, SoundVolume + 0.01f);
                if (Raylib.IsKeyDown(KeyboardKey.Left)) SoundVolume = MathF.Max(0.0f, SoundVolume - 0.01f);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                if (selectedIndex == 1)
                    return SettingsResult.Back;
            }
            return SettingsResult.None;
        }

        public void Draw(int screenWidth, int screenHeight)
        {
            Raylib.DrawText("SETTINGS", screenWidth / 2 - 80, 100, 40, Color.White);

            for (int i = 0; i < options.Length; i++)
            {
                string text = options[i];
                if (i == 0) text += $": {(int)(SoundVolume * 100)}%";
                var color = (i == selectedIndex) ? Color.Yellow : Color.White;
                Raylib.DrawText(text, screenWidth / 2 - 100, 200 + i * 40, 30, color);
            }
        }
    }
}
