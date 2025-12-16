using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace CaveShooter
{
    /// <summary>
    /// Represents the game map with collision detection and destructible walls.
    /// </summary>
    public class Map
    {
        #region Constants

        private const int TileSize = 8;

        #endregion

        #region Properties

        public int Width { get; private set; }
        public int Height { get; private set; }

        #endregion

        #region Private Fields

        private List<Rectangle> wallRects = new List<Rectangle>();
        private Texture2D mapTexture;
        private Image mapImage;

        #endregion

        #region Constructor

        /// <summary>
        /// Loads a map from an image file and generates collision data.
        /// </summary>
        /// <param name="mapImagePath">Path to the map image file.</param>
        public Map(string mapImagePath)
        {
            mapImage = Raylib.LoadImage(mapImagePath);
            mapTexture = Raylib.LoadTextureFromImage(mapImage);
            Width = mapTexture.Width;
            Height = mapTexture.Height;

            LoadCollisionFromImage(mapImage);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Unloads the map texture and image from memory.
        /// </summary>
        public void Unload()
        {
            Raylib.UnloadTexture(mapTexture);
            Raylib.UnloadImage(mapImage);
        }

        /// <summary>
        /// Renders the map texture and optionally displays collision rectangles.
        /// </summary>
        /// <param name="showCollision">If true, draws red overlays on collision tiles.</param>
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

        /// <summary>
        /// Checks if a rectangle collides with any wall and returns the collided walls.
        /// </summary>
        /// <param name="rect">Rectangle to test for collision.</param>
        /// <param name="collidedWalls">List of walls that were hit.</param>
        /// <returns>True if collision occurred.</returns>
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

        /// <summary>
        /// Checks if a rectangle collides with any wall.
        /// </summary>
        /// <param name="rect">Rectangle to test for collision.</param>
        /// <returns>True if collision occurred.</returns>
        public bool CheckCollision(Rectangle rect)
        {
            return CheckCollision(rect, out _);
        }

        /// <summary>
        /// Removes specified walls from the collision map and updates the texture.
        /// </summary>
        /// <param name="wallsToDestroy">Walls to remove.</param>
        public void DestroyWalls(List<Rectangle> wallsToDestroy)
        {
            foreach (var wall in wallsToDestroy)
            {
                // Draw black (passable) color over destroyed wall area in the image
                Raylib.ImageDrawRectangle(ref mapImage, (int)wall.X, (int)wall.Y,
                    (int)wall.Width, (int)wall.Height, Color.Black);
            }

            wallRects.RemoveAll(wall => wallsToDestroy.Contains(wall));

            // Update the texture with the modified image
            unsafe
            {
                Raylib.UpdateTexture(mapTexture, mapImage.Data);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Samples the image to build collision rectangles.
        /// Non-dark pixels are treated as walls (cave walls), dark pixels are passable.
        /// </summary>
        private void LoadCollisionFromImage(Image image)
        {
            int tilesX = image.Width / TileSize;
            int tilesY = image.Height / TileSize;

            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    int sampleX = x * TileSize + TileSize / 2;
                    int sampleY = y * TileSize + TileSize / 2;

                    Color pixelColor = Raylib.GetImageColor(image, sampleX, sampleY);

                    // Non-dark pixels are walls; black areas are passable
                    if (!(pixelColor.R < 50 && pixelColor.G < 50 && pixelColor.B < 50))
                    {
                        wallRects.Add(new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize));
                    }
                }
            }
        }

        #endregion
    }
}