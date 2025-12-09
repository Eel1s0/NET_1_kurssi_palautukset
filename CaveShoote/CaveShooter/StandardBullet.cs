using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace CaveShooter
{
    /// <summary>
    /// Represents a standard projectile that travels in a straight line and destroys walls on impact.
    /// </summary>
    public class StandardBullet
    {
        #region Constants

        private const float DefaultRadius = 1.5f;

        #endregion

        #region Properties

        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public bool IsActive { get; set; } = true;

        #endregion

        #region Private Fields

        private float radius = DefaultRadius;
        private Rectangle collisionRect;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new bullet traveling in the specified direction.
        /// </summary>
        /// <param name="position">Starting position.</param>
        /// <param name="direction">Direction vector (will be normalized).</param>
        /// <param name="speed">Travel speed in pixels per second.</param>
        public StandardBullet(Vector2 position, Vector2 direction, float speed)
        {
            Position = position;
            Velocity = Vector2.Normalize(direction) * speed;
            collisionRect = new Rectangle(position.X - radius, position.Y - radius, radius * 2, radius * 2);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates bullet position and checks for wall collisions.
        /// Destroys walls on impact and deactivates the bullet.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        /// <param name="map">Map for collision detection and wall destruction.</param>
        public void Update(float deltaTime, Map map)
        {
            Position += Velocity * deltaTime;
            collisionRect.X = Position.X - radius;
            collisionRect.Y = Position.Y - radius;

            // Check for collision and get the specific walls that were hit
            if (map.CheckCollision(collisionRect, out List<Rectangle> collidedWalls))
            {
                IsActive = false;
                map.DestroyWalls(collidedWalls);
            }
        }

        /// <summary>
        /// Renders the bullet as a yellow circle.
        /// </summary>
        public void Draw()
        {
            Raylib.DrawCircleV(Position, radius, Color.Yellow);
        }

        #endregion
    }
}