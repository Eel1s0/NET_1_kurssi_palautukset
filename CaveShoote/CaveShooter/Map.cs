using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    public class Map
    {
        private int tileSize = 8;
        private List<Rectangle> wallRects = new List<Rectangle>();
        private Texture2D mapTexture;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(string mapImagePath)
        {
            // Load the PNG as a texture
            mapTexture = Raylib.LoadTexture(mapImagePath);

            Width = mapTexture.Width;
            Height = mapTexture.Height;

            // Load collision data from the image (black pixels = walls)
            Image mapImage = Raylib.LoadImage(mapImagePath);
            LoadCollisionFromImage(mapImage);
            Raylib.UnloadImage(mapImage);
        }

        private void LoadCollisionFromImage(Image image)
        {
            // Sample the image at tile intervals to build collision rectangles
            int tilesX = image.Width / tileSize;
            int tilesY = image.Height / tileSize;

            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    // Sample the center of each tile
                    int sampleX = x * tileSize + tileSize / 2;
                    int sampleY = y * tileSize + tileSize / 2;

                    Color pixelColor = Raylib.GetImageColor(image, sampleX, sampleY);

                    // INVERTED: Treat NON-dark pixels as walls (the gray/colored cave walls)
                    // Black areas (R < 50, G < 50, B < 50) are now passable
                    if (!(pixelColor.R < 50 && pixelColor.G < 50 && pixelColor.B < 50))
                    {
                        wallRects.Add(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                }
            }
        }

        public void Unload()
        {
            Raylib.UnloadTexture(mapTexture);
        }

        public void Draw(bool showCollision = false)
        {
            Raylib.DrawTexture(mapTexture, 0, 0, Color.White);

            if (showCollision)
            {
                foreach (var wall in wallRects)
                {
                    Raylib.DrawRectangleRec(wall, new Color(255, 0, 0, 100));
                }
            }
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

        public bool CheckCollision(Rectangle rect)
        {
            return CheckCollision(rect, out _);
        }

        public void DestroyWalls(List<Rectangle> wallsToDestroy)
        {
            wallRects.RemoveAll(wall => wallsToDestroy.Contains(wall));
        }
    }
}