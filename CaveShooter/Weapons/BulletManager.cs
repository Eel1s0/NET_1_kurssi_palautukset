using System.Collections.Generic;

namespace CaveShooter.Weapons
{
    /// <summary>
    /// Manages the lifecycle of all projectiles in the game.
    /// </summary>
    public class BulletManager
    {
        private List<StandardBullet> bullets;
        private int screenWidth;
        private int screenHeight;

        public BulletManager(int screenWidth, int screenHeight)
        {
            bullets = new List<StandardBullet>();
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        /// <summary>
        /// Adds a new bullet to the manager.
        /// </summary>
        public void AddBullet(StandardBullet bullet)
        {
            bullets.Add(bullet);
        }

        /// <summary>
        /// Updates all bullets and removes inactive or out-of-bounds bullets.
        /// </summary>
        public void Update(float deltaTime)
        {
            foreach (var bullet in bullets)
            {
                bullet.Update(deltaTime);

                // Deactivate bullets that are out of bounds
                if (bullet.IsOutOfBounds(screenWidth, screenHeight))
                {
                    bullet.IsActive = false;
                }
            }

            // Remove inactive bullets
            bullets.RemoveAll(b => !b.IsActive);
        }

        /// <summary>
        /// Draws all active bullets.
        /// </summary>
        public void Draw()
        {
            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }
        }

        /// <summary>
        /// Gets all active bullets.
        /// </summary>
        public List<StandardBullet> GetBullets()
        {
            return bullets;
        }

        /// <summary>
        /// Clears all bullets.
        /// </summary>
        public void Clear()
        {
            bullets.Clear();
        }
    }
}
