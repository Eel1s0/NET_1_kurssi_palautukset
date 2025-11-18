using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class Map
    {
        private string[] layout = new string[]
        {
            "11111111111111111111111111111111",
            "10000000000000000000000000000001",
            "10000000000000000000000000000001",
            "10000011111000000000011111000001",
            "10000000000000000000000000000001",
            "10000000000000000000000000000001",
            "11111000001111111111100000111111",
            "10000000000000000000000000000001",
            "10000000000000000000000000000001",
            "10000011111000000000011111000001",
            "10000000000000000000000000000001",
            "10000000000000000000000000000001",
            "11111111111111111111111111111111"
        };

        private int tileSize = 40;
        private List<Rectangle> wallRects = new List<Rectangle>();
        private RenderTexture2D mapTexture;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map()
        {
            Width = layout[0].Length * tileSize;
            Height = layout.Length * tileSize;

            for (int y = 0; y < layout.Length; y++)
            {
                for (int x = 0; x < layout[y].Length; x++)
                {
                    if (layout[y][x] == '1')
                    {
                        wallRects.Add(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                }
            }

            // Create a render texture to draw the map on once
            mapTexture = Raylib.LoadRenderTexture(Width, Height);
            RedrawMapTexture();
        }

        private void RedrawMapTexture()
        {
            Raylib.BeginTextureMode(mapTexture);
            Raylib.ClearBackground(Color.Black); // Ensure texture background is clear
            foreach (var wall in wallRects)
            {
                Raylib.DrawRectangleRec(wall, Color.Gray);
            }
            Raylib.EndTextureMode();
        }

        public void Unload()
        {
            Raylib.UnloadRenderTexture(mapTexture);
        }

        public void Draw()
        {
            // Draw the pre-rendered map texture. Note that the texture is flipped vertically.
            Raylib.DrawTextureRec(
                mapTexture.Texture,
                new Rectangle(0, 0, Width, -Height), // Negative height to flip the texture
                Vector2.Zero,
                Color.White
            );
        }

        public bool CheckCollision(Rectangle rect, out List<Rectangle> collidedWalls)
        {
            collidedWalls = new List<Rectangle>();
            bool hasCollided = false;
            foreach (var wall in wallRects)
            {
                if (Raylib.CheckCollisionRecs(rect, wall))
                {
                    collidedWalls.Add(wall);
                    hasCollided = true;
                }
            }
            return hasCollided;
        }

        // Overload for checks where we don't care which walls were hit
        public bool CheckCollision(Rectangle rect)
        {
            return CheckCollision(rect, out _);
        }

        public void DestroyWalls(List<Rectangle> wallsToDestroy)
        {
            if (wallsToDestroy.Count == 0) return;

            wallRects.RemoveAll(wall => wallsToDestroy.Contains(wall));
            RedrawMapTexture();
        }
    }
}