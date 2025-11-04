using System.Numerics;
using Raylib_cs;

namespace CaveShooter.Weapons
{
    /// <summary>
    /// Mine weapon that drops stationary explosive projectiles.
    /// </summary>
    public class Mine : IWeapon
    {
        private float cooldownTimer;
        private const float COOLDOWN_TIME = 1.0f; // 1 mine per second
        private const float BULLET_RADIUS = 8f;

        public float Cooldown => COOLDOWN_TIME;
        public bool CanFire => cooldownTimer <= 0;

        public void Fire(Vector2 position, Vector2 direction, BulletManager bulletManager, int ownerId)
        {
            if (!CanFire) return;

            // Mines are stationary - velocity is zero
            Vector2 velocity = Vector2.Zero;
            StandardBullet mine = new StandardBullet(position, velocity, BULLET_RADIUS, Color.Orange, ownerId);
            bulletManager.AddBullet(mine);

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
