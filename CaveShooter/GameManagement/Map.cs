using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace CaveShooter.GameManagement
{
    /// <summary>
    /// Represents a wall segment in the cave.
    /// </summary>
    public struct Wall
    {
        public Vector2 Start;
        public Vector2 End;
        public float Thickness;

        public Wall(Vector2 start, Vector2 end, float thickness = 10f)
        {
            Start = start;
            End = end;
            Thickness = thickness;
        }
    }

    /// <summary>
    /// Manages the game world map and collision detection.
    /// </summary>
    public class Map
    {
        private List<Wall> walls;
        private int width;
        private int height;
        private Color wallColor;

        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            walls = new List<Wall>();
            wallColor = Color.Gray;
        }

        /// <summary>
        /// Loads a basic cave layout with boundary walls.
        /// </summary>
        public void LoadBasicCave()
        {
            walls.Clear();

            // Create boundary walls
            // Top wall
            walls.Add(new Wall(new Vector2(0, 0), new Vector2(width, 0), 20f));
            // Bottom wall
            walls.Add(new Wall(new Vector2(0, height), new Vector2(width, height), 20f));
            // Left wall
            walls.Add(new Wall(new Vector2(0, 0), new Vector2(0, height), 20f));
            // Right wall
            walls.Add(new Wall(new Vector2(width, 0), new Vector2(width, height), 20f));

            // Add some interior obstacles for cave-like environment
            walls.Add(new Wall(new Vector2(200, 100), new Vector2(400, 150), 15f));
            walls.Add(new Wall(new Vector2(600, 200), new Vector2(700, 400), 15f));
            walls.Add(new Wall(new Vector2(300, 400), new Vector2(500, 450), 15f));
        }

        /// <summary>
        /// Adds a custom wall to the map.
        /// </summary>
        public void AddWall(Wall wall)
        {
            walls.Add(wall);
        }

        /// <summary>
        /// Checks if a circle (ship/bullet) collides with any wall.
        /// </summary>
        public bool CheckCircleCollision(Vector2 position, float radius)
        {
            foreach (var wall in walls)
            {
                if (CircleLineCollision(position, radius, wall.Start, wall.End))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks collision between a circle and a line segment.
        /// </summary>
        private bool CircleLineCollision(Vector2 circlePos, float radius, Vector2 lineStart, Vector2 lineEnd)
        {
            // Find the closest point on the line segment to the circle
            Vector2 lineVec = lineEnd - lineStart;
            Vector2 circleVec = circlePos - lineStart;

            float lineLengthSquared = lineVec.LengthSquared();
            if (lineLengthSquared == 0) return Vector2.Distance(circlePos, lineStart) <= radius;

            float t = MathF.Max(0, MathF.Min(1, Vector2.Dot(circleVec, lineVec) / lineLengthSquared));
            Vector2 closestPoint = lineStart + t * lineVec;

            return Vector2.Distance(circlePos, closestPoint) <= radius;
        }

        /// <summary>
        /// Draws all walls in the map.
        /// </summary>
        public void Draw()
        {
            foreach (var wall in walls)
            {
                Raylib.DrawLineEx(wall.Start, wall.End, wall.Thickness, wallColor);
            }
        }

        /// <summary>
        /// Gets all walls in the map.
        /// </summary>
        public List<Wall> GetWalls()
        {
            return walls;
        }

        /// <summary>
        /// Clears all walls from the map.
        /// </summary>
        public void Clear()
        {
            walls.Clear();
        }
    }
}
