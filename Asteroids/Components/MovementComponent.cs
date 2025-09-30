/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System.Numerics;

namespace Asteroids.Components
{
    public class MovementComponent
    {
        public Vector2 Velocity;
        public float MaxSpeed;

        public MovementComponent(float maxSpeed = 300f)
        {
            Velocity = Vector2.Zero;
            MaxSpeed = maxSpeed;
        }

        // Apply an acceleration-like force (units: pixels/s^2) scaled by dt
        public void ApplyForce(Vector2 force, float dt)
        {
            Velocity += force * dt;
            if (Velocity.Length() > MaxSpeed)
            {
                Velocity = Vector2.Normalize(Velocity) * MaxSpeed;
            }
        }

        // Integrate into transform using frame time
        public void Integrate(TransformComponent t, float dt)
        {
            t.Position += Velocity * dt;
        }

        public float Speed => Velocity.Length();
    }
}