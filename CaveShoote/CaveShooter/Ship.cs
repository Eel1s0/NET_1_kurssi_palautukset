using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    /// <summary>
    /// Represents the player's ship with physics, collision, weapons, and rendering.
    /// </summary>
    public class Ship
    {
        #region Constants

        private const float Gravity = 80f;
        private const float Thrust = 180f;
        private const float Damping = 0.98f;
        private const float CollisionBounce = -0.3f;
        private const float ShipSize = 4f;

        #endregion

        #region Properties

        public Vector2 Position { get; private set; }
        public int Health { get; private set; } = 100;

        #endregion

        #region Private Fields

        private Vector2 velocity;
        private IWeapon weapon;
        private Rectangle collisionRect;
        private float fireRate = 5f;
        private float fireCooldown = 0f;
        private InputConfig inputConfig;
        private float rotation = 0f;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ship at the specified position with the given input configuration.
        /// </summary>
        /// <param name="startPosition">Initial spawn position.</param>
        /// <param name="config">Input configuration for controlling the ship.</param>
        public Ship(Vector2 startPosition, InputConfig config)
        {
            Position = startPosition;
            inputConfig = config;
            velocity = Vector2.Zero;
            weapon = new Basic();
            collisionRect = new Rectangle(Position.X - ShipSize, Position.Y - ShipSize, ShipSize * 2, ShipSize * 2);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reduces the ship's health by the specified amount.
        /// </summary>
        /// <param name="amount">Damage to apply.</param>
        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        /// <summary>
        /// Updates ship physics, handles input, processes collisions, and manages shooting.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        /// <param name="bulletManager">Manager for creating bullets.</param>
        /// <param name="map">Map for collision detection.</param>
        public void Update(float deltaTime, BulletManager bulletManager, Map map)
        {
            UpdateCooldown(deltaTime);
            HandleRotationInput(deltaTime);

            Vector2 acceleration = CalculateAcceleration();
            ApplyPhysics(deltaTime, acceleration);
            HandleCollision(deltaTime, map);

            HandleShooting(bulletManager);
        }

        /// <summary>
        /// Renders the ship as a rotated triangle.
        /// </summary>
        public void Draw()
        {
            Vector2 v1 = new Vector2(0, -ShipSize);
            Vector2 v2 = new Vector2(-ShipSize, ShipSize);
            Vector2 v3 = new Vector2(ShipSize, ShipSize);

            float rotationRad = MathF.PI / 180f * rotation;
            float cosR = MathF.Cos(rotationRad);
            float sinR = MathF.Sin(rotationRad);

            Vector2 rv1 = new Vector2(v1.X * cosR - v1.Y * sinR, v1.X * sinR + v1.Y * cosR);
            Vector2 rv2 = new Vector2(v2.X * cosR - v2.Y * sinR, v2.X * sinR + v2.Y * cosR);
            Vector2 rv3 = new Vector2(v3.X * cosR - v3.Y * sinR, v3.X * sinR + v3.Y * cosR);

            Raylib.DrawTriangle(
                Position + rv1,
                Position + rv2,
                Position + rv3,
                Color.White);
        }

        #endregion

        #region Private Methods

        private void UpdateCooldown(float deltaTime)
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= deltaTime;
            }
        }

        private void HandleRotationInput(float deltaTime)
        {
            if (Raylib.IsKeyDown(inputConfig.Left)) rotation -= 120f * deltaTime;
            if (Raylib.IsKeyDown(inputConfig.Right)) rotation += 120f * deltaTime;
        }

        private Vector2 CalculateAcceleration()
        {
            Vector2 acceleration = new Vector2(0, Gravity);
            float rotationRad = MathF.PI / 180f * (rotation - 90);

            if (Raylib.IsKeyDown(inputConfig.Up))
            {
                acceleration.X += MathF.Cos(rotationRad) * Thrust;
                acceleration.Y += MathF.Sin(rotationRad) * Thrust;
            }

            return acceleration;
        }

        private void ApplyPhysics(float deltaTime, Vector2 acceleration)
        {
            velocity += acceleration * deltaTime;
            velocity *= Damping;
        }

        private void HandleCollision(float deltaTime, Map map)
        {
            Vector2 newPosition = Position;

            // Check X movement
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

            // Check Y movement
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
        }

        private void HandleShooting(BulletManager bulletManager)
        {
            if (Raylib.IsKeyDown(inputConfig.Shoot) && fireCooldown <= 0)
            {
                fireCooldown = 1f / fireRate;
                float rotationRad = MathF.PI / 180f * (rotation - 90);
                Vector2 direction = new Vector2(MathF.Cos(rotationRad), MathF.Sin(rotationRad));
                weapon.Shoot(Position, direction, bulletManager);
            }
        }

        #endregion
    }
}