using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace CaveShooter
{
    public class StandardBullet
    {
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public bool IsActive { get; set; } = true;
        private float radius = 1.5f;
        private Rectangle collisionRect;

        public StandardBullet(Vector2 position, Vector2 direction, float speed)
        {
            Position = position;
            Velocity = Vector2.Normalize(direction) * speed;
            collisionRect = new Rectangle(position.X - radius, position.Y - radius, radius * 2, radius * 2);
        }

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

        public void Draw()
        {
            Raylib.DrawCircleV(Position, radius, Color.Yellow);
        }
    }
}