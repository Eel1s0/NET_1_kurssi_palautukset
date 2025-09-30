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
    class Player
    {
        // Components
        private TransformComponent transform;
        private MovementComponent movement;
        private RenderComponent renderer;
        private CollisionComponent collider;
        public TransformComponent Transform => transform;
        public CollisionComponent Collider => collider;

        private float Acceleration = 400f;
        private float ShootCooldown = 0.1f;
        private float timeUntilNextShot = 0f;

        public Player(float x, float y, Texture2D texture)
        {
            transform = new TransformComponent(x, y);
            movement = new MovementComponent(maxSpeed: 300f);
            renderer = new RenderComponent(texture, drawScale: 0.5f);
            // Ensure the collider uses the renderer's scale
            collider = new CollisionComponent(renderer.GetDrawRadius());
        }

        public void Update()
        {
            float dt = Raylib.GetFrameTime();

            Vector2 input = Vector2.Zero;
            if (Raylib.IsKeyDown(KeyboardKey.W)) input.Y -= 1f;
            if (Raylib.IsKeyDown(KeyboardKey.S)) input.Y += 1f;
            if (Raylib.IsKeyDown(KeyboardKey.A)) input.X -= 1f;
            if (Raylib.IsKeyDown(KeyboardKey.D)) input.X += 1f;

            if (input.Length() > 0f)
            {
                input = Vector2.Normalize(input);
                movement.ApplyForce(input * Acceleration, dt);
            }

            movement.Integrate(transform, dt);

            if (movement.Speed > 0.001f)
            {
                transform.Rotation = MathF.Atan2(movement.Velocity.Y, movement.Velocity.X) * Raylib.RAD2DEG;
            }

            if (timeUntilNextShot > 0f) timeUntilNextShot -= dt;

            WrapComponent.Wrap(ref transform.Position);
        }

        public void Shoot(List<Bullet> bullets, Texture2D bulletTexture)
        {
            if (timeUntilNextShot <= 0f)
            {
                float rad = transform.Rotation * Raylib.DEG2RAD;
                Vector2 direction = Raymath.Vector2Rotate(new Vector2(1f, 0f), rad);
                direction = Vector2.Normalize(direction);
                bullets.Add(new Bullet(transform.Position, direction, bulletTexture));
                timeUntilNextShot = ShootCooldown;
            }
        }

        public void Draw()
        {
            renderer.Draw(transform, rotationOffset: 90f);

            // Draw collider if debug mode is active
            if (Program.IsDebugMode)
            {
                Raylib.DrawCircleLines((int)transform.Position.X, (int)transform.Position.Y, collider.GetRadius(), Color.Red);
            }
        }

        public Vector2 Position => transform.Position;

        public bool CollidesWith(Asteroid asteroid)
        {
            // Use the exposed components directly
            return collider.CollidesWith(this.Transform, this.Collider, asteroid.Transform, asteroid.Collider);
        }

        public bool CollidesWith(Bullet bullet)
        {
            // Use the exposed components directly
            return collider.CollidesWith(this.Transform, this.Collider, bullet.Transform, bullet.Collider);
        }
    }
}