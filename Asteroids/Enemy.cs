using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace Asteroids
{
    class Enemy
    {
        public Vector2 Position;
        public Texture2D Texture;
        public Texture2D BulletTexture;
        private Vector2 Velocity;
        private float ShootCooldown = 2.0f; // Time between shots
        private float TimeUntilNextShot = 0.0f;
        public bool IsDestroyed { get; private set; } = false;

        private Random random = new Random();

        // Visual / collision scale (single source of truth, no magic numbers)
        private float DrawScale = 0.6f;

        public Enemy(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition)
        {
            Texture = texture;
            BulletTexture = bulletTexture;
            Position = startPosition;
            Velocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
        }

        public void Update(List<Bullet> enemyBullets)
        {
            if (IsDestroyed) return;

            float dt = Raylib.GetFrameTime();

            Position += Velocity * dt;

            // Wrap around the screen
            if (Position.X < 0) Position.X = Program.screenWidth;
            else if (Position.X > Program.screenWidth) Position.X = 0;

            if (Position.Y < 0) Position.Y = Program.screenHeight;
            else if (Position.Y > Program.screenHeight) Position.Y = 0;

            // Randomly change direction
            if (random.NextDouble() < 0.01)
            {
                Velocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
            }

            // Shoot bullets
            if (TimeUntilNextShot <= 0)
            {
                Shoot(enemyBullets);
                TimeUntilNextShot = ShootCooldown;
            }
            else
            {
                TimeUntilNextShot -= dt;
            }
        }

        public void Shoot(List<Bullet> enemyBullets)
        {
            Vector2 direction = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
            if (direction.Length() == 0f) direction = new Vector2(1f, 0f);
            enemyBullets.Add(new Bullet(Position, direction, BulletTexture));
        }

        public void Draw()
        {
            if (IsDestroyed) return;

            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            float scaledW = Texture.Width * DrawScale;
            float scaledH = Texture.Height * DrawScale;

            Raylib.DrawTexturePro(Texture,
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                new Rectangle(Position.X - scaledW / 2f, Position.Y - scaledH / 2f, scaledW, scaledH),
                origin,
                0,
                Color.White);
        }

        // Expose radius so collisions use an explicit, named value (no magic numbers)
        public float GetRadius()
        {
            return (Texture.Width * DrawScale) / 2f;
        }

        public bool CollidesWith(Bullet bullet)
        {
            // Use Raylib collision helper and explicit radii
            return Raylib.CheckCollisionCircles(Position, GetRadius(), bullet.Position, bullet.GetRadius());
        }

        public void Destroy()
        {
            IsDestroyed = true;
        }
    }
}