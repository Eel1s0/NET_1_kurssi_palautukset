using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroids
{
    class Bullet
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private Texture2D Texture;
        private float Speed = 500.0f; // pixels/s
        private float Rotation; // degrees
        private float Lifespan = 2.0f; // seconds
        private float Age = 0.0f;

        // Visual / collision scale exposed via GetRadius() (no hidden magic numbers in collision calls)
        private float DrawScale = 0.25f;

        public Bullet(Vector2 position, Vector2 direction, Texture2D texture)
        {
            Position = position;
            Texture = texture;
            Vector2 dirNorm = Vector2.Normalize(direction);
            Velocity = dirNorm * Speed;
            Rotation = MathF.Atan2(dirNorm.Y, dirNorm.X) * Raylib.RAD2DEG;
        }

        public void Update()
        {
            float dt = Raylib.GetFrameTime();
            Position += Velocity * dt;
            Age += dt;
        }

        public bool IsExpired()
        {
            return Age >= Lifespan;
        }

        public void Draw()
        {
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            float scaledW = Texture.Width * DrawScale;
            float scaledH = Texture.Height * DrawScale;

            Raylib.DrawTexturePro(
                Texture,
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                new Rectangle(Position.X - scaledW / 2f, Position.Y - scaledH / 2f, scaledW, scaledH),
                origin,
                Rotation,
                Color.White
            );
        }

        public bool IsOnScreen()
        {
            return Position.X >= 0 && Position.X <= Program.screenWidth && Position.Y >= 0 && Position.Y <= Program.screenHeight;
        }

        // Expose radius explicitly so collisions use consistent, named scale instead of magic numbers
        public float GetRadius()
        {
            return (Texture.Width * DrawScale) / 2f;
        }

        public bool CollidesWith(Asteroid asteroid)
        {
            return Raylib.CheckCollisionCircles(Position, GetRadius(), asteroid.Position, asteroid.GetRadius());
        }

        public bool CollidesWith(Enemy enemy)
        {
            // Assume Enemy would also expose GetRadius() — use its collision helper if available
            return Raylib.CheckCollisionCircles(Position, GetRadius(), enemy.Position, enemy.GetRadius());
        }
    }
}