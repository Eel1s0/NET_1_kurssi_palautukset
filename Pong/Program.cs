using Raylib_cs;
using System.Numerics;

namespace Pong
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 600;

            Raylib.InitWindow(screenWidth, screenHeight, "Pong Game");
            Raylib.SetTargetFPS(60);

            Vector2 player1Pos = new Vector2(50, screenHeight / 2 - 50);
            Vector2 player2Pos = new Vector2(screenWidth - 70, screenHeight / 2 - 50);

            int player1Score = 0;
            int player2Score = 0;

            const float paddleSpeed = 400.0f;
            const int paddleWidth = 20;
            const int paddleHeight = 100;

            Vector2 ballPos = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 ballDir = new Vector2(1, 1);
            const float ballSpeed = 300.0f;

            while (!Raylib.WindowShouldClose())
            {
                ballPos += ballDir * ballSpeed * Raylib.GetFrameTime();

                if (ballPos.Y <= 0 || ballPos.Y >= screenHeight)
                {
                    ballDir.Y *= -1;
                }

                Rectangle player1Rect = new Rectangle(player1Pos.X, player1Pos.Y, paddleWidth, paddleHeight);
                Rectangle player2Rect = new Rectangle(player2Pos.X, player2Pos.Y, paddleWidth, paddleHeight);
                
                if (Raylib.CheckCollisionCircleRec(ballPos, 10, player1Rect))
                {
                    ballDir.X = 1;
                }

                if (Raylib.CheckCollisionCircleRec(ballPos, 10, player2Rect))
                {
                    ballDir.X = -1;
                }

                if (Raylib.IsKeyDown(KeyboardKey.W) && player1Pos.Y > 0)
                {
                    player1Pos.Y -= paddleSpeed * Raylib.GetFrameTime();
                }

                if (Raylib.IsKeyDown(KeyboardKey.S) && player1Pos.Y + paddleHeight < screenHeight)
                {
                    player1Pos.Y += paddleSpeed * Raylib.GetFrameTime();
                }

                // Player 2 (Up & Down keys)
                if (Raylib.IsKeyDown(KeyboardKey.Up) && player2Pos.Y > 0)
                {
                    player2Pos.Y -= paddleSpeed * Raylib.GetFrameTime();
                }

                if (Raylib.IsKeyDown(KeyboardKey.Down) && player2Pos.Y + paddleHeight < screenHeight)
                {
                    player2Pos.Y += paddleSpeed * Raylib.GetFrameTime();
                }

                if (ballPos.X <= 0)
                {
                    player2Score++;
                    ballPos = new Vector2(screenWidth / 2, screenHeight / 2);
                    ballDir = new Vector2(1, 1);
                }

                if (ballPos.X >= screenWidth)
                {
                    player1Score++;
                    ballPos = new Vector2(screenWidth / 2, screenHeight / 2);
                    ballDir = new Vector2(-1, -1);
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.DrawRectangle((int)player1Pos.X, (int)player1Pos.Y, paddleWidth, paddleHeight, Color.Blue);
                Raylib.DrawRectangle((int)player2Pos.X, (int)player2Pos.Y, paddleWidth, paddleHeight, Color.Red);

                // Draw ball
                Raylib.DrawCircle((int)ballPos.X, (int)ballPos.Y, 10, Color.White);

                // Draw scores
                Raylib.DrawText(player1Score.ToString(), screenWidth / 4, 20, 40, Color.Blue);
                Raylib.DrawText(player2Score.ToString(), screenWidth * 3 / 4, 20, 40, Color.Red);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
