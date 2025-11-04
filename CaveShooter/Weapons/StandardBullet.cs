using System.Numerics;
using Raylib_cs;

namespace CaveShooter.Weapons
{
    /// <summary>
    /// Represents a basic projectile type.
    /// </summary>
    public class StandardBullet
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Radius { get; set; }
        public Color Color { get; set; }
        public bool IsActive { get; set; }
        public int OwnerId { get; set; } // Player ID who fired this bullet

        public StandardBullet(Vector2 position, Vector2 velocity, float radius, Color color, int ownerId)
        {
            Position = position;
            Velocity = velocity;
            Radius = radius;
            Color = color;
            IsActive = true;
            OwnerId = ownerId;
        }

        /// <summary>
        /// Updates the bullet position based on velocity.
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds.</param>
        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            Position += Velocity * deltaTime;
        }

        /// <summary>
        /// Draws the bullet on screen.
        /// </summary>
        public void Draw()
        {
            if (IsActive)
            {
                Raylib.DrawCircleV(Position, Radius, Color);
            }
        }

        /// <summary>
        /// Checks if bullet is out of bounds.
        /// </summary>
        public bool IsOutOfBounds(int screenWidth, int screenHeight)
        {
            return Position.X < -50 || Position.X > screenWidth + 50 ||
                   Position.Y < -50 || Position.Y > screenHeight + 50;
        }
    }
}
