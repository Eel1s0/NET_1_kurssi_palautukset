using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

class Program
{
    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;
        Raylib.InitWindow(screenWidth, screenHeight, "Tank Game");
        Raylib.SetTargetFPS(60);

        Tank player1 = new Tank(new Vector2(100, screenHeight / 2), Color.Blue, KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D, KeyboardKey.Space);
        Tank player2 = new Tank(new Vector2(screenWidth - 100, screenHeight / 2), Color.Red, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right, KeyboardKey.Enter);

        List<Bullet> bullets = new List<Bullet>();
        List<Wall> walls = new List<Wall>
        {
            new Wall(new Vector2(screenWidth / 3, screenHeight / 2 - 100), new Vector2(30, 200)),
            new Wall(new Vector2(2 * screenWidth / 3, screenHeight / 2 - 100), new Vector2(30, 200))
        };

        int score1 = 0;
        int score2 = 0;
        const int winningScore = 5;

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            if (score1 < winningScore && score2 < winningScore)
            {
                player1.Update(deltaTime, bullets);
                player2.Update(deltaTime, bullets);

                foreach (var bullet in bullets)
                    bullet.Update(deltaTime);

                foreach (var bullet in bullets)
                {
                    foreach (var wall in walls)
                    {
                        if (Raylib.CheckCollisionRecs(bullet.GetRectangle(), wall.GetRectangle()))
                            bullet.IsActive = false;
                    }

                    if (Raylib.CheckCollisionRecs(bullet.GetRectangle(), player1.GetRectangle()))
                    {
                        bullet.IsActive = false;
                        score2++;  // Player 2 scores for hitting Player 1
                        player1.Respawn();
                    }

                    if (Raylib.CheckCollisionRecs(bullet.GetRectangle(), player2.GetRectangle()))
                    {
                        bullet.IsActive = false;
                        score1++;  // Player 1 scores for hitting Player 2
                        player2.Respawn();
                    }
                }

                bullets.RemoveAll(b => !b.IsActive);
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Green);

            // Draw walls
            foreach (var wall in walls)
                wall.Draw();

            // Draw tanks and bullets if no one has won yet
            if (score1 < winningScore && score2 < winningScore)
            {
                player1.Draw();
                player2.Draw();
                foreach (var bullet in bullets)
                    bullet.Draw();
            }

            // 🏆 Draw scores
            Raylib.DrawText($"Player 1: {score1}", 20, 20, 30, Color.Blue);
            Raylib.DrawText($"Player 2: {score2}", 600, 20, 30, Color.Red);

            // 🎉 Win condition
            if (score1 >= winningScore)
                Raylib.DrawText("Player 1 Wins!", 300, 250, 40, Color.Blue);
            else if (score2 >= winningScore)
                Raylib.DrawText("Player 2 Wins!", 300, 250, 40, Color.Red);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}

class Tank
{
    public Vector2 Position;
    public Vector2 Direction;
    public Color TankColor;
    private float speed = 200.0f;
    private KeyboardKey upKey, downKey, leftKey, rightKey, shootKey;
    private float lastShootTime = 0f;
    private float shootCooldown = 0.5f;
    private float gunLength = 30.0f;

    public Tank(Vector2 startPosition, Color color, KeyboardKey up, KeyboardKey down, KeyboardKey left, KeyboardKey right, KeyboardKey shoot)
    {
        Position = startPosition;
        TankColor = color;
        Direction = new Vector2(0, -1);
        upKey = up;
        downKey = down;
        leftKey = left;
        rightKey = right;
        shootKey = shoot;
    }

    public void Update(float deltaTime, List<Bullet> bullets)
    {
        Vector2 movement = Vector2.Zero;

        if (Raylib.IsKeyDown(upKey)) { movement.Y -= 1; Direction = new Vector2(0, -1); }
        if (Raylib.IsKeyDown(downKey)) { movement.Y += 1; Direction = new Vector2(0, 1); }
        if (Raylib.IsKeyDown(leftKey)) { movement.X -= 1; Direction = new Vector2(-1, 0); }
        if (Raylib.IsKeyDown(rightKey)) { movement.X += 1; Direction = new Vector2(1, 0); }

        if (movement.Length() > 0)
            movement = Vector2.Normalize(movement);

        Position += movement * speed * deltaTime;

        if (Raylib.IsKeyPressed(shootKey) && Raylib.GetTime() - lastShootTime > shootCooldown)
        {
            Vector2 bulletStart = Position + Direction * gunLength;
            bullets.Add(new Bullet(bulletStart, Direction, TankColor));
            lastShootTime = (float)Raylib.GetTime();
        }
    }

    public void Draw()
    {
        Raylib.DrawRectangleV(Position, new Vector2(40, 40), TankColor);
        Vector2 barrelEnd = Position + Direction * gunLength;
        Raylib.DrawRectangleV(barrelEnd - new Vector2(5, 5), new Vector2(10, 20), Color.Black);
    }

    public Rectangle GetRectangle() => new Rectangle(Position.X, Position.Y, 40, 40);
    public void Respawn() => Position = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
}

class Bullet
{
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed = 400f;
    public bool IsActive = true;
    private Color BulletColor;

    public Bullet(Vector2 startPosition, Vector2 direction, Color color)
    {
        Position = startPosition + direction * 20;
        Direction = direction;
        BulletColor = color;
    }

    public void Update(float deltaTime)
    {
        Position += Direction * Speed * deltaTime;

        if (Position.X < 0 || Position.X > Raylib.GetScreenWidth() ||
            Position.Y < 0 || Position.Y > Raylib.GetScreenHeight())
        {
            IsActive = false;
        }
    }

    public void Draw() => Raylib.DrawCircleV(Position, 5, BulletColor);
    public Rectangle GetRectangle() => new Rectangle(Position.X - 5, Position.Y - 5, 10, 10);
}

class Wall
{
    public Vector2 Position;
    public Vector2 Size;

    public Wall(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public void Draw() => Raylib.DrawRectangleV(Position, Size, Color.DarkGray);
    public Rectangle GetRectangle() => new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
}
