using System.Numerics;

namespace CaveShooter
{
    public interface IWeapon
    {
        void Shoot(Vector2 startPosition, Vector2 direction, BulletManager bulletManager);
    }
}