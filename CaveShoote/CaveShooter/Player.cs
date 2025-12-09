using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    /// <summary>
    /// Represents a player with a ship and camera for split-screen gameplay.
    /// </summary>
    public class Player
    {
        #region Properties

        public Ship Ship { get; private set; }
        public Camera Camera { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new player with a ship and camera.
        /// </summary>
        /// <param name="startPosition">Initial spawn position for the ship.</param>
        /// <param name="inputConfig">Input configuration for controlling the ship.</param>
        /// <param name="viewWidth">Width of the player's viewport.</param>
        /// <param name="viewHeight">Height of the player's viewport.</param>
        public Player(Vector2 startPosition, InputConfig inputConfig, float viewWidth, float viewHeight)
        {
            Ship = new Ship(startPosition, inputConfig);
            Camera = new Camera(viewWidth, viewHeight);
            // Set the initial camera target to the player's starting position.
            Camera.SetTarget(startPosition);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the ship and camera each frame.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        /// <param name="map">Map for collision detection.</param>
        /// <param name="bulletManager">Manager for bullet interactions.</param>
        public void Update(float deltaTime, Map map, BulletManager bulletManager)
        {
            Ship.Update(deltaTime, bulletManager, map);
            Camera.Update(Ship.Position);
        }

        /// <summary>
        /// Renders the player's ship.
        /// </summary>
        public void Draw()
        {
            Ship.Draw();
        }

        #endregion
    }
}
