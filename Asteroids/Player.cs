using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using RainDrop;

namespace Asteroids
{
    class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private Texture2D Texture; // The texture for the player
        private float Acceleration = 0.2f; // Force applied by WASD keys
        private float MaxSpeed = 5.0f;     // Maximum speed limit
        private float Rotation = 0.0f;    // Rotation angle in degrees
        private float ShootCooldown = 0.1f; // Time between shots
        private float TimeUntilNextShot = 0.0f; // Countdown until next shot

        public Player(float x, float y, Texture2D texture)
        {
            Position = new Vector2(x, y);
            Velocity = new Vector2(0, 0);

            Texture = texture;

        }

        public void Update()
        {
            // Apply forces based on input (WASD keys)
            if (Raylib.IsKeyDown(KeyboardKey.W)) // Up
                Velocity.Y -= Acceleration;
            if (Raylib.IsKeyDown(KeyboardKey.S)) // Down
                Velocity.Y += Acceleration;
            if (Raylib.IsKeyDown(KeyboardKey.A)) // Left
                Velocity.X -= Acceleration;
            if (Raylib.IsKeyDown(KeyboardKey.D)) // Right
                Velocity.X += Acceleration;

            // Clamp the velocity to the maximum speed
            if (Velocity.Length() > MaxSpeed)
            {
                Velocity = Vector2.Normalize(Velocity) * MaxSpeed;
            }

            Position.X += Velocity.X;
            Position.Y += Velocity.Y;

            if (Velocity.Length() > 0) // Only update rotation if the player is moving
            {
                Rotation = MathF.Atan2(Velocity.Y, Velocity.X) * Raylib.RAD2DEG;
            }

            // Cooldown for shooting
            if (TimeUntilNextShot > 0)
            {
                TimeUntilNextShot -= Raylib.GetFrameTime();
            }

            WrapPosition();
        }

        public void Shoot(List<Bullet> bullets, Texture2D bulletTexture)
        {
            if (TimeUntilNextShot <= 0)
            {
                Vector2 direction = new Vector2(MathF.Cos(Rotation * Raylib.DEG2RAD), MathF.Sin(Rotation * Raylib.DEG2RAD));
                bullets.Add(new Bullet(Position, direction, bulletTexture));
                TimeUntilNextShot = ShootCooldown;
            }
        }

        public void Draw()
        {
            // Scale factor for reducing the size
            float scale = 0.5f;

            // Draw the texture at the player's position, rotated 90 degrees to the right and scaled
            Vector2 origin = new Vector2(Texture.Width / 4, Texture.Height / 4); // Center of the image
            Raylib.DrawTexturePro(
                Texture,
                new Rectangle(0, 0, Texture.Width, Texture.Height), // Source rectangle (full texture)
                new Rectangle(Position.X, Position.Y, Texture.Width * scale, Texture.Height * scale), // Destination rectangle (scaled size)
                origin, // Origin (rotation center)
                Rotation + 90, // Add 90 degrees to the rotation
                Color.White
            );
        }

        private void WrapPosition()
        {
            if (Position.X < 0) Position.X = Program.screenWidth;
            else if (Position.X > Program.screenWidth) Position.X = 0;

            if (Position.Y < 0) Position.Y = Program.screenHeight;
            else if (Position.Y > Program.screenHeight) Position.Y = 0;
        }

        public bool CollidesWith(Asteroid asteroid)
        {
            float dx = Position.X - asteroid.Position.X;
            float dy = Position.Y - asteroid.Position.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            return distance < asteroid.GetRadius();
        }

        public bool CollidesWith(Bullet bullet)
        {
            float dx = Position.X - bullet.Position.X;
            float dy = Position.Y - bullet.Position.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            return distance < Texture.Width * 0.3f; // Adjust collision radius as needed
        }
    }
}
