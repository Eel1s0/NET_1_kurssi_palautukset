using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class Player
    {
        public Ship Ship { get; private set; }
        public Camera Camera { get; private set; }

        public Player(Vector2 startPosition, InputConfig inputConfig, float viewWidth, float viewHeight)
        {
            Ship = new Ship(startPosition, inputConfig);
            Camera = new Camera(viewWidth, viewHeight);
            // Set the initial camera target to the player's starting position.
            Camera.SetTarget(startPosition);
        }

        public void Update(float deltaTime, Map map, BulletManager bulletManager)
        {
            Ship.Update(deltaTime, bulletManager, map);
            Camera.Update(Ship.Position);
        }

        public void Draw()
        {
            Ship.Draw();
        }
    }
}
