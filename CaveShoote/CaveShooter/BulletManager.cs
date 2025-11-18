using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class BulletManager
    {
        private List<StandardBullet> bullets = new List<StandardBullet>();

        public void CreateBullet(Vector2 position, Vector2 direction)
        {
            var bullet = new StandardBullet(position, direction, 500f);
            bullets.Add(bullet);
        }

        public void Update(float deltaTime, Map map)
        {
            foreach (var bullet in bullets)
            {
                bullet.Update(deltaTime, map);
            }
            // Remove bullets that are no longer active
            bullets.RemoveAll(b => !b.IsActive);
        }

        public void Draw()
        {
            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }
        }
    }
}