using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class Ship
    {
        public Vector2 Position { get; private set; }
        private Vector2 velocity;
        private const float Gravity = 80f;
        private const float Thrust = 180f;
        private const float Damping = 0.98f;
        private const float CollisionBounce = -0.3f;

        // Ship size - reduced from 15 to 4 for pixel art scale
        private const float ShipSize = 4f;

        public int Health { get; private set; } = 100;

        private IWeapon weapon;
        private Rectangle collisionRect;

        private float fireRate = 5f;
        private float fireCooldown = 0f;

        private InputConfig inputConfig;
        private float rotation = 0f;

        public Ship(Vector2 startPosition, InputConfig config)
        {
            Position = startPosition;
            inputConfig = config;
            velocity = Vector2.Zero;
            weapon = new Basic();
            collisionRect = new Rectangle(Position.X - ShipSize, Position.Y - ShipSize, ShipSize * 2, ShipSize * 2);
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        public void Update(float deltaTime, BulletManager bulletManager, Map map)
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= deltaTime;
            }

            if (Raylib.IsKeyDown(inputConfig.Left)) rotation -= 120f * deltaTime;
            if (Raylib.IsKeyDown(inputConfig.Right)) rotation += 120f * deltaTime;

            Vector2 acceleration = new Vector2(0, Gravity);
            float rotationRad = (float)(Math.PI / 180f) * (rotation - 90);

            if (Raylib.IsKeyDown(inputConfig.Up))
            {
                acceleration.X += (float)Math.Cos(rotationRad) * Thrust;
                acceleration.Y += (float)Math.Sin(rotationRad) * Thrust;
            }

            velocity += acceleration * deltaTime;
            velocity *= Damping;

            Vector2 newPosition = Position;

            // Check X movement with smaller collision box
            float newX = Position.X + velocity.X * deltaTime;
            Rectangle xRect = new Rectangle(newX - ShipSize, Position.Y - ShipSize, ShipSize * 2, ShipSize * 2);
            if (map.CheckCollision(xRect))
            {
                velocity.X *= CollisionBounce;
            }
            else
            {
                newPosition.X = newX;
            }

            // Check Y movement with smaller collision box
            float newY = Position.Y + velocity.Y * deltaTime;
            Rectangle yRect = new Rectangle(newPosition.X - ShipSize, newY - ShipSize, ShipSize * 2, ShipSize * 2);
            if (map.CheckCollision(yRect))
            {
                velocity.Y *= CollisionBounce;
            }
            else
            {
                newPosition.Y = newY;
            }

            Position = newPosition;
            collisionRect.X = Position.X - ShipSize;
            collisionRect.Y = Position.Y - ShipSize;

            if (Raylib.IsKeyDown(inputConfig.Shoot) && fireCooldown <= 0)
            {
                fireCooldown = 1f / fireRate;
                Vector2 direction = new Vector2((float)Math.Cos(rotationRad), (float)Math.Sin(rotationRad));
                weapon.Shoot(Position, direction, bulletManager);
            }
        }

        public void Draw()
        {
            // Smaller triangle for pixel art scale
            Vector2 v1 = new Vector2(0, -ShipSize);
            Vector2 v2 = new Vector2(-ShipSize, ShipSize);
            Vector2 v3 = new Vector2(ShipSize, ShipSize);

            float rotationRad = (float)(Math.PI / 180f) * rotation;
            float cosR = (float)Math.Cos(rotationRad);
            float sinR = (float)Math.Sin(rotationRad);

            Vector2 rv1 = new Vector2(v1.X * cosR - v1.Y * sinR, v1.X * sinR + v1.Y * cosR);
            Vector2 rv2 = new Vector2(v2.X * cosR - v2.Y * sinR, v2.X * sinR + v2.Y * cosR);
            Vector2 rv3 = new Vector2(v3.X * cosR - v3.Y * sinR, v3.X * sinR + v3.Y * cosR);

            Raylib.DrawTriangle(
                Position + rv1,
                Position + rv2,
                Position + rv3,
                Color.White);
        }
    }
}