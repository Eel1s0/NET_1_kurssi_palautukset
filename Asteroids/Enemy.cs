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

            Position += Velocity;

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
                TimeUntilNextShot -= Raylib.GetFrameTime();
            }
        }

        public void Shoot(List<Bullet> enemyBullets)
        {
            Vector2 direction = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
            enemyBullets.Add(new Bullet(Position, direction, BulletTexture));
        }

        public void Draw()
        {
            if (IsDestroyed) return;

            float scale = 0.6f; // Enemy is slightly smaller
            Vector2 origin = new Vector2(Texture.Width / 4, Texture.Height / 4);
            Raylib.DrawTexturePro(Texture,
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                new Rectangle(Position.X, Position.Y, Texture.Width * scale, Texture.Height * scale),
                origin,
                0,
                Color.White);
        }

        public bool CollidesWith(Bullet bullet)
        {
            float dx = Position.X - bullet.Position.X;
            float dy = Position.Y - bullet.Position.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            return distance < Texture.Width * 0.3f; // Approximate collision radius
        }

        public void Destroy()
        {
            IsDestroyed = true;
        }
    }
}