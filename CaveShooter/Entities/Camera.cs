using System.Numerics;
using Raylib_cs;

namespace CaveShooter.Entities
{
    /// <summary>
    /// Camera class that follows the player's ship.
    /// </summary>
    public class Camera
    {
        private Camera2D camera;
        private Ship? targetShip;
        private const float SMOOTHING = 0.1f;

        public Camera(int screenWidth, int screenHeight)
        {
            camera = new Camera2D
            {
                Target = new Vector2(screenWidth / 2, screenHeight / 2),
                Offset = new Vector2(screenWidth / 2, screenHeight / 2),
                Rotation = 0f,
                Zoom = 1f
            };
        }

        /// <summary>
        /// Sets the ship for the camera to follow.
        /// </summary>
        public void SetTarget(Ship ship)
        {
            targetShip = ship;
        }

        /// <summary>
        /// Updates the camera to smoothly follow the target ship.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (targetShip != null && targetShip.IsAlive)
            {
                // Smoothly interpolate camera towards ship position
                Vector2 targetPosition = targetShip.Position;
                camera.Target = Vector2.Lerp(camera.Target, targetPosition, SMOOTHING);
            }
        }

        /// <summary>
        /// Begins the camera mode for rendering.
        /// </summary>
        public void BeginMode()
        {
            Raylib.BeginMode2D(camera);
        }

        /// <summary>
        /// Ends the camera mode for rendering.
        /// </summary>
        public void EndMode()
        {
            Raylib.EndMode2D();
        }

        /// <summary>
        /// Gets the current camera target position.
        /// </summary>
        public Vector2 GetTarget()
        {
            return camera.Target;
        }

        /// <summary>
        /// Sets the camera zoom level.
        /// </summary>
        public void SetZoom(float zoom)
        {
            camera.Zoom = zoom;
        }
    }
}
