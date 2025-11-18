using System.Numerics;

namespace CaveShooter
{
    public class Basic : IWeapon
    {
        public void Shoot(Vector2 startPosition, Vector2 direction, BulletManager bulletManager)
        {
            bulletManager.CreateBullet(startPosition, direction);
        }
    }
}