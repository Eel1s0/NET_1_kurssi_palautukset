using System;
using System.Numerics;
using Raylib_cs;

namespace Asteroids
{
    enum AsteroidSize
    {
        Large,
        Medium,
        Small
    }

    class Asteroid
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public AsteroidSize Size;
        private float Rotation;
        private float Scale;

        public Asteroid(Texture2D texture, Vector2 position, Vector2 velocity, AsteroidSize size)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Size = size;
            Rotation = 0;

            // Set scale based on size
            Scale = size switch
            {
                AsteroidSize.Large => 1.0f,
                AsteroidSize.Medium => 0.6f,
                AsteroidSize.Small => 0.3f,
                _ => 1.0f
            };
        }

        public void Update()
        {
            Position += Velocity;
            Rotation += 1.0f; // Rotate asteroid
            WrapPosition();
        }

        public void Draw()
        {
            Vector2 origin = new Vector2(Texture.Width / 4, Texture.Height / 4);
            Raylib.DrawTexturePro(Texture,
                new Rectangle(0, 0, Texture.Width, Texture.Height),
                new Rectangle(Position.X, Position.Y, Texture.Width * Scale, Texture.Height * Scale),
                origin,
                Rotation,
                Color.White);
        }

        private void WrapPosition()
        {
            if (Position.X < 0) Position.X = Program.screenWidth;
            else if (Position.X > Program.screenWidth) Position.X = 0;

            if (Position.Y < 0) Position.Y = Program.screenHeight;
            else if (Position.Y > Program.screenHeight) Position.Y = 0;
        }

        public float GetRadius()
        {
            return Texture.Width * Scale / 2;
        }
    }
}
