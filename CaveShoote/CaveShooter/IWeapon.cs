using System.Numerics;

namespace CaveShooter
{
    /// <summary>
    /// Interface for weapon implementations that can fire projectiles.
    /// </summary>
    public interface IWeapon
    {
        /// <summary>
        /// Fires the weapon from the specified position in the given direction.
        /// </summary>
        /// <param name="startPosition">Position to spawn the projectile.</param>
        /// <param name="direction">Direction vector for projectile travel.</param>
        /// <param name="bulletManager">Manager to register the created bullet.</param>
        void Shoot(Vector2 startPosition, Vector2 direction, BulletManager bulletManager);
    }
}