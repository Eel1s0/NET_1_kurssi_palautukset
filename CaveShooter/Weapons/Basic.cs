using System.Numerics;
using Raylib_cs;

namespace CaveShooter.Weapons
{
    /// <summary>
    /// Basic weapon implementation with standard fire rate and projectile speed.
    /// </summary>
    public class Basic : IWeapon
    {
        private float cooldownTimer;
        private const float COOLDOWN_TIME = 0.2f; // 5 shots per second
        private const float BULLET_SPEED = 500f;
        private const float BULLET_RADIUS = 4f;

        public float Cooldown => COOLDOWN_TIME;
        public bool CanFire => cooldownTimer <= 0;

        public void Fire(Vector2 position, Vector2 direction, BulletManager bulletManager, int ownerId)
        {
            if (!CanFire) return;

            Vector2 velocity = Vector2.Normalize(direction) * BULLET_SPEED;
            StandardBullet bullet = new StandardBullet(position, velocity, BULLET_RADIUS, Color.Yellow, ownerId);
            bulletManager.AddBullet(bullet);

            cooldownTimer = COOLDOWN_TIME;
        }

        public void Update(float deltaTime)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= deltaTime;
            }
        }
    }
}
