/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System;
using System.Numerics;
using Raylib_cs;
using Asteroids.Components;

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
        private TransformComponent transform;
        private MovementComponent movement;
        private RenderComponent renderer;
        private CollisionComponent collider;

        public Texture2D Texture => renderer.Texture;
        public Vector2 Position => transform.Position;
        public Vector2 Velocity => movement.Velocity;
        public AsteroidSize Size { get; private set; }
        public TransformComponent Transform => transform;
        public CollisionComponent Collider => collider;

        private float rotationSpeed = 30f;
        

        public Asteroid(Texture2D texture, Vector2 position, Vector2 velocity, AsteroidSize size)
        {
            transform = new TransformComponent(position.X, position.Y);
            movement = new MovementComponent(maxSpeed: 500f) { Velocity = velocity };
            Size = size;

            float scale = size switch
            {
                AsteroidSize.Large => 1.0f,
                AsteroidSize.Medium => 0.6f,
                AsteroidSize.Small => 0.3f,
                _ => 1.0f
            };

            renderer = new RenderComponent(texture, drawScale: scale);
            // Correctly calculate the radius based on the scale for this size
            collider = new CollisionComponent(renderer.GetDrawRadius());
        }

        public void Update()
        {
            float dt = Raylib.GetFrameTime();
            movement.Integrate(transform, dt);

            transform.Rotation += rotationSpeed * dt;

            WrapComponent.Wrap(ref transform.Position);
        }

        public void Draw()
        {
            renderer.Draw(transform, rotationOffset: 0f);
            if (Program.IsDebugMode)
            {
                Raylib.DrawCircleLines((int)transform.Position.X, (int)transform.Position.Y, GetRadius(), Color.Green);
            }
        }

        public float GetRadius() => collider.GetRadius();
    }
}