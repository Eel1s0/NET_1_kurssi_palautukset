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

        // Movement parameters (units are per second where appropriate)
        private float Acceleration = 200.0f; // pixels/s^2
        private float MaxSpeed = 300.0f;     // pixels/s

        // Visual / collision scale (kept as a named field instead of a magic number)
        private float DrawScale = 0.5f;

        private float Rotation = 0.0f;    // Rotation angle in degrees
        private float ShootCooldown = 0.1f; // Time between shots
        private float TimeUntilNextShot = 0.0f; // Countdown until next shot

        public Player(float x, float y, Texture2D texture)
        {
            Position = new Vector2(x, y);
            Velocity = Vector2.Zero;
            Texture = texture;
        }

        public void Update()
        {
            // Make movement framerate independent
            float dt = Raylib.GetFrameTime();

            // Input -> direction vector
            Vector2 input = Vector2.Zero;
            if (Raylib.IsKeyDown(KeyboardKey.W)) input.Y -= 1f;
            if (Raylib.IsKeyDown(KeyboardKey.S)) input.Y += 1f;
            if (Raylib.IsKeyDown(KeyboardKey.A)) input.X -= 1f;
            if (Raylib.IsKeyDown(KeyboardKey.D)) input.X += 1f;

            if (input.Length() > 0f)
            {
                input = Vector2.Normalize(input);
                // Acceleration applied per second -> scale by dt
                Velocity += input * Acceleration * dt;
            }

            // Clamp speed (vel is in pixels/s)
            if (Velocity.Length() > MaxSpeed)
            {
                Velocity = Vector2.Normalize(Velocity) * MaxSpeed;
            }

            // Integrate position
            Position += Velocity * dt;

            // Update facing direction from velocity (degrees)
            if (Velocity.Length() > 0.001f)
            {
                Rotation = MathF.Atan2(Velocity.Y, Velocity.X) * Raylib.RAD2DEG;
            }

            // Shooting cooldown
            if (TimeUntilNextShot > 0f) TimeUntilNextShot -= dt;

            WrapPosition();
        }

        public void Shoot(List<Bullet> bullets, Texture2D bulletTexture)
        {
            if (TimeUntilNextShot <= 0f)
            {
                // Use Raylib's Raymath helper to compute direction from rotation
                float rad = Rotation * Raylib.DEG2RAD;
                Vector2 direction = Raymath.Vector2Rotate(new Vector2(1f, 0f), rad);
                direction = Vector2.Normalize(direction);

                bullets.Add(new Bullet(Position, direction, bulletTexture));
                TimeUntilNextShot = ShootCooldown;
            }
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
                Rotation + 90f, // texture orientation adjustment
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

        // Expose collision radius so callers don't need to use magic numbers
        public float GetRadius()
        {
            // radius = half of the drawn width
            return (Texture.Width * DrawScale) / 2f;
        }

        public bool CollidesWith(Asteroid asteroid)
        {
            // Use Raylib's built-in circle collision check
            return Raylib.CheckCollisionCircles(Position, GetRadius(), asteroid.Position, asteroid.GetRadius());
        }

        public bool CollidesWith(Bullet bullet)
        {
            // Use Raylib's built-in circle collision check and explicit radii (no magic numbers)
            return Raylib.CheckCollisionCircles(Position, GetRadius(), bullet.Position, bullet.GetRadius());
        }
    }
}