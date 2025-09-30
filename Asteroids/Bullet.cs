/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System;
using System.Numerics;
using Raylib_cs;
using Asteroids.Components;

namespace Asteroids
{
    class Bullet
    {
        // components
        private TransformComponent transform;
        private MovementComponent movement;
        private RenderComponent renderer;
        private CollisionComponent collider;
        public TransformComponent Transform => transform;
        public CollisionComponent Collider => collider;

        private float lifespan = 2.0f;
        private float age = 0.0f;

        public Bullet(Vector2 position, Vector2 direction, Texture2D texture)
        {
            transform = new TransformComponent(position.X, position.Y);
            movement = new MovementComponent(maxSpeed: 1000f);
            movement.Velocity = Vector2.Normalize(direction) * 500f;
            renderer = new RenderComponent(texture, drawScale: 0.25f);
            // Ensure the collider uses the renderer's scale
            collider = new CollisionComponent(renderer.GetDrawRadius());
        }

        public void Update()
        {
            float dt = Raylib.GetFrameTime();
            movement.Integrate(transform, dt);
            age += dt;
        }

        public bool IsExpired() => age >= lifespan;

        public void Draw()
        {
            // Compute visual rotation from velocity and add 90 degrees so sprite orientation matches ship nose
            float angleDeg = movement.Velocity.Length() > 0
                ? MathF.Atan2(movement.Velocity.Y, movement.Velocity.X) * Raylib.RAD2DEG
                : 0f;

            // Add 90 deg to rotate the bullet sprite to match ship orientation
            renderer.Draw(transform, rotationOffset: angleDeg + 90f);

            // Draw collider if debug mode is active
            if (Program.IsDebugMode)
            {
                Raylib.DrawCircleLines((int)transform.Position.X, (int)transform.Position.Y, GetRadius(), Color.Blue);
            }
        }

        public bool IsOnScreen()
        {
            float r = GetRadius();
            return transform.Position.X + r >= 0 && transform.Position.X - r <= Program.screenWidth
                && transform.Position.Y + r >= 0 && transform.Position.Y - r <= Program.screenHeight;
        }

        public float GetRadius() => collider.GetRadius();

        public Vector2 Position => transform.Position;

        public bool CollidesWith(Asteroid asteroid)
        {
            return collider.CollidesWith(this.Transform, this.Collider, asteroid.Transform, asteroid.Collider);
        }

        public bool CollidesWith(Enemy enemy)
        {
            return collider.CollidesWith(this.Transform, this.Collider, enemy.Transform, enemy.Collider);
        }
    }
}