/// <summary>
/// KOODI TEHTY AI AVUSTUKSELLA
/// </summary>
using Raylib_cs;
using System.Numerics;

namespace Asteroids.Components
{
    public class RenderComponent
    {
        public Texture2D Texture;
        public float DrawScale;

        public RenderComponent(Texture2D texture, float drawScale = 1.0f)
        {
            Texture = texture;
            DrawScale = drawScale;
        }

        public void Draw(TransformComponent t, float rotationOffset = 0f)
        {
            if (Texture.Id == 0) return;

            var sourceRec = new Rectangle(0, 0, Texture.Width, Texture.Height);
            var destRec = new Rectangle(t.Position.X, t.Position.Y, Texture.Width * DrawScale, Texture.Height * DrawScale);
            var origin = new Vector2(Texture.Width * DrawScale / 2, Texture.Height * DrawScale / 2);

            Raylib.DrawTexturePro(Texture, sourceRec, destRec, origin, t.Rotation + rotationOffset, Color.White);

            if (Program.IsDebugMode)
            {
                Raylib.DrawCircleLines((int)t.Position.X, (int)t.Position.Y, GetDrawRadius(), Color.Lime);
            }
        }

        public float GetDrawRadius()
        {
            return (Texture.Width > Texture.Height ? Texture.Width : Texture.Height) * DrawScale / 2;
        }
    }
}