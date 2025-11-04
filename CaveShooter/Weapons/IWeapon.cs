using System.Numerics;

namespace CaveShooter.Weapons
{
    /// <summary>
    /// Interface for all weapon types in the game.
    /// </summary>
    public interface IWeapon
    {
        /// <summary>
        /// Fires the weapon from the specified position and direction.
        /// </summary>
        /// <param name="position">The position to fire from.</param>
        /// <param name="direction">The direction to fire in.</param>
        /// <param name="bulletManager">The bullet manager to add projectiles to.</param>
        void Fire(Vector2 position, Vector2 direction, BulletManager bulletManager);

        /// <summary>
        /// Gets the cooldown time between shots in seconds.
        /// </summary>
        float Cooldown { get; }

        /// <summary>
        /// Gets whether the weapon can currently fire.
        /// </summary>
        bool CanFire { get; }

        /// <summary>
        /// Updates the weapon state (cooldown, etc.).
        /// </summary>
        /// <param name="deltaTime">Time since last frame in seconds.</param>
        void Update(float deltaTime);
    }
}
