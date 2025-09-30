/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using Asteroids.Components;

namespace Asteroids
{
    class Enemy
    {
        // components
        private TransformComponent transform;
        private MovementComponent movement;
        private RenderComponent renderer;
        private CollisionComponent collider;
        public TransformComponent Transform => transform;
        public CollisionComponent Collider => collider;

        private Texture2D bulletTexture;
        private float shootCooldown = 2.0f;
        private float timeUntilNextShot = 0f;

        public bool IsDestroyed { get; private set; } = false;
        private Random random = new Random();
        public Vector2 Position => transform.Position;

        /// <summary>
        /// Initializes a new instance of the Enemy class.
        /// </summary>
        /// <param name="texture">The texture for the enemy ship.</param>
        /// <param name="bulletTexture">The texture for the bullets fired by the enemy.</param>
        /// <param name="startPosition">The initial position of the enemy.</param>
        public Enemy(Texture2D texture, Texture2D bulletTexture, Vector2 startPosition)
        {
            transform = new TransformComponent(startPosition.X, startPosition.Y);
            movement = new MovementComponent(maxSpeed: 150f)
            {
                Velocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1))
            };
            renderer = new RenderComponent(texture, drawScale: 0.6f);
            collider = new CollisionComponent((texture.Width * 0.6f) / 2f);
            this.bulletTexture = bulletTexture;
        }

        /// <summary>
        /// Updates the enemy's state, including movement and shooting logic.
        /// </summary>
        public void Update(List<Bullet> enemyBullets)
        {
            if (IsDestroyed) return;

            float dt = Raylib.GetFrameTime();
            movement.Integrate(transform, dt);

            WrapComponent.Wrap(ref transform.Position);

            if (random.NextDouble() < 0.01)
            {
                movement.Velocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1)) * 100f;
            }

            if (timeUntilNextShot <= 0f)
            {
                Shoot(enemyBullets);
                timeUntilNextShot = shootCooldown;
            }
            else timeUntilNextShot -= dt;
        }

        /// <summary>
        /// Creates a new bullet and adds it to the list of enemy bullets.
        /// </summary>
        public void Shoot(List<Bullet> enemyBullets)
        {
            Vector2 dir = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
            if (dir.Length() == 0f) dir = new Vector2(1f, 0f);
            enemyBullets.Add(new Bullet(transform.Position, dir, bulletTexture));
        }

        /// <summary>
        /// Draws the enemy ship and its collider if debug mode is active.
        /// </summary>
        public void Draw()
        {
            if (IsDestroyed) return;

            renderer.Draw(transform, rotationOffset: 0f);

            // Draw collider if debug mode is active
            if (Program.IsDebugMode)
            {
                Raylib.DrawCircleLines((int)transform.Position.X, (int)transform.Position.Y, GetRadius(), Color.Purple);
            }
        }

        /// <Summary>
        /// Gets the radius of the enemy's collider.
        /// The collider radius.
        public float GetRadius() => collider.GetRadius();

        
        /// Checks for a collision with the specified bullet.
        /// The bullet to check for collision against.
        /// True if a collision occurs, otherwise false.
        public bool CollidesWith(Bullet bullet)
        {
            return collider.CollidesWith(this.Transform, this.Collider, bullet.Transform, bullet.Collider);
        }

        
        /// Marks the enemy as destroyed.
        public void Destroy() => IsDestroyed = true;
    }
}