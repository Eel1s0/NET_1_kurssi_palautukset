using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroids
{
    class Bullet
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private Texture2D Texture; // The texture for the bullet
        private float Speed = 10.0f;
        private float Rotation; // Rotation angle based on direction
        private float Lifespan = 2.0f; // New: Lifespan in seconds

        public Bullet(Vector2 position, Vector2 direction, Texture2D texture)
        {
            Position = position;
            Velocity = Vector2.Normalize(direction) * Speed;
            // Load the texture for the bullet
            Texture = texture;
            // Calculate rotation angle based on direction
            Rotation = MathF.Atan2(direction.Y, direction.X) * Raylib.RAD2DEG;
        }

        public void Update()
        {
            // Update the bullet's position
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;

            // Screen wrapping logic
            if (Position.X > Program.screenWidth) Position.X = 0; // Wrap from right to left
            else if (Position.X < 0) Position.X = Program.screenWidth; // Wrap from left to right

            if (Position.Y > Program.screenHeight) Position.Y = 0; // Wrap from bottom to top
            else if (Position.Y < 0) Position.Y = Program.screenHeight; // Wrap from top to bottom

            Lifespan -= Raylib.GetFrameTime();
        }

        public bool IsExpired()
        {
            // Check if the bullet's lifespan has expired
            return Lifespan <= 0;
        }


        public void Draw()
        {
            // Scale factor for reducing bullet size
            float scale = 0.5f;

            // Draw the bullet texture with rotation and scaling
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2); // Center of the image
            Raylib.DrawTexturePro(
                Texture,
                new Rectangle(0, 0, Texture.Width, Texture.Height), // Source rectangle (full texture)
                new Rectangle(Position.X, Position.Y, Texture.Width * scale, Texture.Height * scale), // Destination rectangle (scaled size)
                origin, // Origin (rotation center)
                Rotation + 90, // Add 90 degrees to the rotation
                Color.White
            );
        }

        public bool IsOnScreen()
        {
            return Position.X >= 0 && Position.X <= Program.screenWidth && Position.Y >= 0 && Position.Y <= Program.screenHeight;
        }

        public bool CollidesWith(Enemy enemy)
        {
            float dx = Position.X - enemy.Position.X;
            float dy = Position.Y - enemy.Position.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            return distance < enemy.Texture.Width * 0.3f; // Adjust collision radius as needed
        }

        public bool CollidesWith(Asteroid asteroid)
        {
            float dx = Position.X - asteroid.Position.X;
            float dy = Position.Y - asteroid.Position.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);
            return distance < asteroid.GetRadius(); // Adjust collision radius as needed
        }
    }
}
