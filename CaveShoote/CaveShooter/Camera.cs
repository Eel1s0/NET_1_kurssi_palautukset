using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class Camera
    {
        public Camera2D Instance { get; private set; }

        public Camera(float viewWidth, float viewHeight)
        {
            Instance = new Camera2D
            {
                Offset = new Vector2(viewWidth / 2f, viewHeight / 2f),
                Rotation = 0f,
                Zoom = 1.0f
            };
        }

        public void SetTarget(Vector2 target)
        {
            Instance = Instance with { Target = target };
        }

        public void Update(Vector2 targetPosition)
        {
            // Smoothly follow the target
            float smoothing = 0.1f;
            Vector2 newTarget = Instance.Target + (targetPosition - Instance.Target) * smoothing;
            Instance = Instance with { Target = newTarget };
        }
    }
}