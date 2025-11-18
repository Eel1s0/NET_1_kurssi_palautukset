using System;
using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class Ship
    {
        public Vector2 Position { get; private set; }
        private Vector2 velocity;
        private const float Gravity = 50f; // pixels/s^2
        private const float Thrust = 250f; // pixels/s^2
        private const float Damping = 0.995f; // To simulate some air resistance

        private IWeapon weapon;
        private Rectangle collisionRect;

        // Cooldown fields for rapid fire
        private float fireRate = 5f; // 5 bullets per second
        private float fireCooldown = 0f;

        private InputConfig inputConfig;
        private float rotation = 0f; // Ship's rotation in degrees

        public Ship(Vector2 startPosition, InputConfig config)
        {
            Position = startPosition;
            inputConfig = config;
            velocity = Vector2.Zero;
            weapon = new Basic();
            collisionRect = new Rectangle(Position.X - 15, Position.Y - 15, 30, 30);
        }

        public void Update(float deltaTime, BulletManager bulletManager, Map map)
        {
            // Update fire cooldown
            if (fireCooldown > 0)
            {
                fireCooldown -= deltaTime;
            }

            // Rotation logic
            if (Raylib.IsKeyDown(inputConfig.Left)) rotation -= 180f * deltaTime;
            if (Raylib.IsKeyDown(inputConfig.Right)) rotation += 180f * deltaTime;

            // Physics-based movement
            Vector2 acceleration = new Vector2(0, Gravity); // Apply gravity
            float rotationRad = (float)(Math.PI / 180f) * (rotation - 90); // Adjust for triangle orientation

            if (Raylib.IsKeyDown(inputConfig.Up))
            {
                // Apply thrust in the direction the ship is facing
                acceleration.X += (float)Math.Cos(rotationRad) * Thrust;
                acceleration.Y += (float)Math.Sin(rotationRad) * Thrust;
            }

            // Update velocity and apply damping
            velocity += acceleration * deltaTime;
            velocity *= Damping;

            // --- Collision Detection and Response ---
            // We'll check X and Y movement separately to allow sliding.
            Vector2 newPosition = Position + velocity * deltaTime;
            Vector2 finalPosition = Position;

            // Check X-axis collision
            Rectangle futureRectX = new Rectangle(newPosition.X - 15, Position.Y - 15, 30, 30);
            if (!map.CheckCollision(futureRectX))
            {
                finalPosition.X = newPosition.X;
            }
            else
            {
                // Collided horizontally, stop horizontal movement
                velocity.X = 0;
            }

            // Check Y-axis collision
            Rectangle futureRectY = new Rectangle(finalPosition.X - 15, newPosition.Y - 15, 30, 30);
            if (!map.CheckCollision(futureRectY))
            {
                finalPosition.Y = newPosition.Y;
            }
            else
            {
                // Collided vertically, stop vertical movement
                velocity.Y = 0;
            }

            Position = finalPosition;
            collisionRect.X = Position.X - 15;
            collisionRect.Y = Position.Y - 15;

            // Shooting logic
            if (Raylib.IsKeyDown(inputConfig.Shoot) && fireCooldown <= 0)
            {
                // Reset cooldown
                fireCooldown = 1f / fireRate;

                Vector2 direction = new Vector2((float)Math.Cos(rotationRad), (float)Math.Sin(rotationRad));
                weapon.Shoot(Position, direction, bulletManager);
            }
        }

        public void Draw()
        {
            // Define the triangle points relative to the origin
            Vector2 v1 = new Vector2(0, -15); // Top point
            Vector2 v2 = new Vector2(-15, 15); // Bottom-left
            Vector2 v3 = new Vector2(15, 15); // Bottom-right

            // Convert rotation to radians
            float rotationRad = (float)(Math.PI / 180f) * rotation;
            float cosR = (float)Math.Cos(rotationRad);
            float sinR = (float)Math.Sin(rotationRad);

            // Rotate points around the origin
            Vector2 rv1 = new Vector2(v1.X * cosR - v1.Y * sinR, v1.X * sinR + v1.Y * cosR);
            Vector2 rv2 = new Vector2(v2.X * cosR - v2.Y * sinR, v2.X * sinR + v2.Y * cosR);
            Vector2 rv3 = new Vector2(v3.X * cosR - v3.Y * sinR, v3.X * sinR + v3.Y * cosR);

            // Translate points to the ship's position and draw
            Raylib.DrawTriangle(
                Position + rv1,
                Position + rv2,
                Position + rv3,
                Color.White);
        }
    }
}