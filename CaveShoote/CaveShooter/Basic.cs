using System.Numerics;

namespace CaveShooter
{
    /// <summary>
    /// Basic weapon that fires a single standard bullet.
    /// </summary>
    public class Basic : IWeapon
    {
        /// <summary>
        /// Fires a single bullet in the specified direction.
        /// </summary>
        /// <param name="startPosition">Position to spawn the bullet.</param>
        /// <param name="direction">Direction vector for bullet travel.</param>
        /// <param name="bulletManager">Manager to register the created bullet.</param>
        public void Shoot(Vector2 startPosition, Vector2 direction, BulletManager bulletManager)
        {
            bulletManager.CreateBullet(startPosition, direction);
        }
    }
}