/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System.Numerics;

namespace Asteroids.Components
{
    public class TransformComponent
    {
        public Vector2 Position;
        public float Rotation; // degrees

        public TransformComponent(float x = 0, float y = 0, float rotation = 0)
        {
            Position = new Vector2(x, y);
            Rotation = rotation;
        }
    }
}