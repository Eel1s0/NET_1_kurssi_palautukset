using System;
using System.Numerics;
using Raylib_cs;

namespace CaveShooter.Entities
{
    /// <summary>
    /// Represents the player's vehicle/ship.
    /// </summary>
    public class Ship
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; } // In radians
        public float Radius { get; set; }
        public Color Color { get; set; }
        public bool IsAlive { get; set; }

        private const float ACCELERATION = 300f;
        private const float MAX_SPEED = 400f;
        private const float ROTATION_SPEED = 3f;
        private const float DRAG = 0.98f;

        public Ship(Vector2 startPosition, Color color)
        {
            Position = startPosition;
            Velocity = Vector2.Zero;
            Rotation = 0f;
            Radius = 15f;
            Color = color;
            IsAlive = true;
        }

        /// <summary>
        /// Applies thrust in the direction the ship is facing.
        /// </summary>
        public void Thrust(float deltaTime)
        {
            Vector2 direction = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
            Velocity += direction * ACCELERATION * deltaTime;

            // Clamp velocity to max speed
            if (Velocity.Length() > MAX_SPEED)
            {
                Velocity = Vector2.Normalize(Velocity) * MAX_SPEED;
            }
        }

        /// <summary>
        /// Rotates the ship left.
        /// </summary>
        public void RotateLeft(float deltaTime)
        {
            Rotation -= ROTATION_SPEED * deltaTime;
        }

        /// <summary>
        /// Rotates the ship right.
        /// </summary>
        public void RotateRight(float deltaTime)
        {
            Rotation += ROTATION_SPEED * deltaTime;
        }

        /// <summary>
        /// Updates ship physics.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!IsAlive) return;

            // Apply drag
            Velocity *= DRAG;

            // Update position
            Position += Velocity * deltaTime;
        }

        /// <summary>
        /// Draws the ship as a triangle pointing in the direction of rotation.
        /// </summary>
        public void Draw()
        {
            if (!IsAlive) return;

            // Calculate triangle points for ship
            float angle = Rotation;
            Vector2 front = Position + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * Radius;
            Vector2 back1 = Position + new Vector2(MathF.Cos(angle + 2.5f), MathF.Sin(angle + 2.5f)) * Radius * 0.7f;
            Vector2 back2 = Position + new Vector2(MathF.Cos(angle - 2.5f), MathF.Sin(angle - 2.5f)) * Radius * 0.7f;

            Raylib.DrawTriangle(back1, back2, front, Color);
            Raylib.DrawCircleV(Position, 3, Color.White); // Center dot
        }

        /// <summary>
        /// Gets the forward direction the ship is facing.
        /// </summary>
        public Vector2 GetForwardDirection()
        {
            return new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
        }

        /// <summary>
        /// Wraps ship position around screen boundaries.
        /// </summary>
        public void WrapAroundScreen(int screenWidth, int screenHeight)
        {
            if (Position.X < 0) Position = new Vector2(screenWidth, Position.Y);
            if (Position.X > screenWidth) Position = new Vector2(0, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, screenHeight);
            if (Position.Y > screenHeight) Position = new Vector2(Position.X, 0);
        }
    }
}
