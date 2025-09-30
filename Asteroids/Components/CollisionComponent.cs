/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using System.Numerics;
using Raylib_cs;

namespace Asteroids.Components
{
    public class CollisionComponent
    {
        public float Radius;

        public CollisionComponent(float radius)
        {
            Radius = radius;
        }

        public float GetRadius() => Radius;

        // Updated to accept components directly, avoiding new object creation
        public bool CollidesWith(TransformComponent ownTransform, CollisionComponent ownCollider, TransformComponent otherTransform, CollisionComponent otherCollider)
        {
            return Raylib.CheckCollisionCircles(ownTransform.Position, ownCollider.GetRadius(), otherTransform.Position, otherCollider.GetRadius());
        }
    }
}