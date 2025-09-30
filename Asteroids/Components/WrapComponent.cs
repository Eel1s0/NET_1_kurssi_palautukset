/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System.Numerics;
using Raylib_cs;

namespace Asteroids.Components
{
    public static class WrapComponent
    {
        // Wraps position around Program.screenWidth / screenHeight
        public static void Wrap(ref Vector2 position)
        {
            if (position.X < 0) position.X = Program.screenWidth;
            else if (position.X > Program.screenWidth) position.X = 0;

            if (position.Y < 0) position.Y = Program.screenHeight;
            else if (position.Y > Program.screenHeight) position.Y = 0;
        }
    }
}