using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    /// <summary>
    /// Manages a 2D camera that smoothly follows a target position.
    /// </summary>
    public class Camera
    {
        #region Properties

        public Camera2D Instance { get; private set; }

        #endregion

        #region Constants

        private const float DefaultZoom = 2.0f;
        private const float SmoothingFactor = 0.1f;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new camera centered on the specified viewport dimensions.
        /// </summary>
        /// <param name="viewWidth">Width of the viewport.</param>
        /// <param name="viewHeight">Height of the viewport.</param>
        public Camera(float viewWidth, float viewHeight)
        {
            Instance = new Camera2D
            {
                Offset = new Vector2(viewWidth / 2f, viewHeight / 2f),
                Rotation = 0f,
                Zoom = DefaultZoom
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the camera target position directly.
        /// </summary>
        /// <param name="target">New target position.</param>
        public void SetTarget(Vector2 target)
        {
            Instance = Instance with { Target = target };
        }

        /// <summary>
        /// Smoothly interpolates the camera toward the target position.
        /// </summary>
        /// <param name="targetPosition">Position to follow.</param>
        public void Update(Vector2 targetPosition)
        {
            // Smoothly follow the target
            Vector2 newTarget = Instance.Target + (targetPosition - Instance.Target) * SmoothingFactor;
            Instance = Instance with { Target = newTarget };
        }

        #endregion
    }
}