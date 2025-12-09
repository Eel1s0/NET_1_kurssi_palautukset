using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    /// <summary>
    /// Manages all active bullets in the game, including creation, updates, and rendering.
    /// </summary>
    public class BulletManager
    {
        #region Private Fields

        private List<StandardBullet> bullets = new List<StandardBullet>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new bullet at the specified position traveling in the given direction.
        /// </summary>
        /// <param name="position">Starting position of the bullet.</param>
        /// <param name="direction">Direction vector for bullet travel.</param>
        public void CreateBullet(Vector2 position, Vector2 direction)
        {
            var bullet = new StandardBullet(position, direction, 500f);
            bullets.Add(bullet);
        }

        /// <summary>
        /// Updates all bullets and removes inactive ones.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        /// <param name="map">Map for collision detection.</param>
        public void Update(float deltaTime, Map map)
        {
            foreach (var bullet in bullets)
            {
                bullet.Update(deltaTime, map);
            }

            bullets.RemoveAll(b => !b.IsActive);
        }

        /// <summary>
        /// Renders all active bullets.
        /// </summary>
        public void Draw()
        {
            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }
        }

        #endregion
    }
}