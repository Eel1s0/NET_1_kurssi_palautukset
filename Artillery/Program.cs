using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Raylib_cs;
using RayRectangle = Raylib_cs.Rectangle;

/// <summary>
/// Koko Koodissa on käytetty AI:n apua.
/// </summary>

class Program
{
    public const int screenWidth = 800;
    public const int screenHeight = 600;
    public const int terrainHeight = 100;
    public const float gravity = 0.5f;
    public const float maxCharge = 15.0f; // Adjusted max charge
    public const float chargeSpeed = 20.0f; // Adjusted charge speed
    public const float baseVelocity = 20.0f;
    public const int startingScore = 5; // Starting score
    public const float minChargeToShoot = 10.0f; // Minimum charge level to fire a shot
    public const float minVelocity = 5.0f;

    static void Main()
    {
        // Initialization
        Raylib.InitWindow(screenWidth, screenHeight, "Tykkipeli");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();

        Maasto terrain = new Maasto();
        List<Player> players = new List<Player> {
            new Player(new Vector2(100, terrain.GetHeightAt(100)), Raylib_cs.Color.Red),
            new Player(new Vector2(screenWidth - 100, terrain.GetHeightAt(screenWidth - 100)), Raylib_cs.Color.Blue)
        };
        List<Ammus> projectiles = new List<Ammus>();

        // Load Ammo Types 
        List<AmmusType> ammusTypes = LoadAmmusTypes("ammus_types.json");
        int currentPlayerIndex = 0;
        int selectedBulletIndex = 0;

        if (!File.Exists("explosion.wav"))
        {
            Console.WriteLine("Error: explosion.wav file not found!");
            Raylib.CloseWindow();
            return;
        }

        // Load sound
        Sound explosionSound = Raylib.LoadSound("explosion.wav");
        Sound cannonShootSound = Raylib.LoadSound("laserShoot.wav");

        bool gameWon = false;
        string winningPlayer = "";

        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            // Update
            Player currentPlayer = players[currentPlayerIndex];
            if (Raylib.IsKeyDown(KeyboardKey.Right)) players[currentPlayerIndex].cannon.Rotate(1.0f);
            if (Raylib.IsKeyDown(KeyboardKey.Left)) players[currentPlayerIndex].cannon.Rotate(-1.0f);

            if (Raylib.IsKeyPressed(KeyboardKey.One)) selectedBulletIndex = 0;
            if (Raylib.IsKeyPressed(KeyboardKey.Two)) selectedBulletIndex = 1;
            if (Raylib.IsKeyPressed(KeyboardKey.Three)) selectedBulletIndex = 2;

            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                currentPlayer.chargeLevel += chargeSpeed * Raylib.GetFrameTime();
                if (currentPlayer.chargeLevel > maxCharge) currentPlayer.chargeLevel = maxCharge;
                Console.WriteLine($"Charging: {currentPlayer.chargeLevel}");
            }

            if (Raylib.IsKeyReleased(KeyboardKey.Space))
            {
                if (currentPlayer.chargeLevel >= minChargeToShoot)
                {
                    Tykki cannon = currentPlayer.cannon;
                    float charge = currentPlayer.chargeLevel / maxCharge;
                    float adjustedVelocity = minVelocity + (baseVelocity - minVelocity) * charge;
                    Vector2 velocity = new Vector2((float)Math.Cos(cannon.angle * Raylib.DEG2RAD) * adjustedVelocity, (float)Math.Sin(cannon.angle * Raylib.DEG2RAD) * adjustedVelocity);
                    AmmusType selectedAmmusType = ammusTypes[selectedBulletIndex];
                    Vector2 startPosition = new Vector2(cannon.position.X + (float)Math.Cos(cannon.angle * Raylib.DEG2RAD) * 40, cannon.position.Y + (float)Math.Sin(cannon.angle * Raylib.DEG2RAD) * 40); // Start position to avoid self-collision
                    projectiles.Add(new Ammus(startPosition, velocity, selectedAmmusType.radius, new Raylib_cs.Color(selectedAmmusType.color.r, selectedAmmusType.color.g, selectedAmmusType.color.b, 255)));

                    // Play cannon shoot sound
                    Raylib.PlaySound(cannonShootSound);

                    currentPlayer.chargeLevel = 0;
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

                    // Debug output
                    Console.WriteLine($"Shot fired with velocity: {velocity}, charge: {charge}, angle: {cannon.angle}, startPosition: {startPosition}");
                }
                else
                {
                    // Debug output
                    Console.WriteLine($"Shot not fired, charge level too low: {currentPlayer.chargeLevel}");
                }
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                bool projectileRemoved = false;
                foreach (Player player in players)
                {
                    if (Raylib.CheckCollisionCircleRec(projectiles[i].position, 5, new RayRectangle(player.cannon.position.X - 20, player.cannon.position.Y - 10, 40, 20)))
                    {
                        player.score--;
                        projectiles.RemoveAt(i);
                        projectileRemoved = true;
                        break;
                    }
                }

                // Ensure projectile removal does not cause index out of range issues
                if (projectileRemoved) break;

                if (projectiles[i].position.Y >= screenHeight - terrainHeight)
                {
                    terrain.DestroyTerrain(projectiles[i].position, projectiles[i].explosionRadius);
                    Raylib.PlaySound(explosionSound);
                    projectiles.RemoveAt(i);
                }
            }

            foreach (Player player in players)
            {
                if (player.score <= 0)
                {
                    gameWon = true;
                    Raylib_cs.Color playerColor = player.cannon.color; // Ensure this is Raylib_cs.Color

                    if (playerColor.Equals(Raylib_cs.Color.Blue))
                    {
                        winningPlayer = "Red";
                    }
                    else if (playerColor.Equals(Raylib_cs.Color.Red))
                    {
                        winningPlayer = "Blue";
                    }
                    else
                    {
                        winningPlayer = "Unknown"; // Fallback in case of an unexpected color
                    }
                }
            }

            // Draw
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib_cs.Color.SkyBlue);

            if (gameWon)
            {
                Raylib.DrawText($"{winningPlayer} player wins!", screenWidth / 2 - 100, screenHeight / 2 - 50, 20, Raylib_cs.Color.Black);
                Raylib.DrawText("Press [R] to Restart or [ESC] to Exit", screenWidth / 2 - 150, screenHeight / 2, 20, Raylib_cs.Color.Black);

                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    // Restart the game
                    gameWon = false;
                    foreach (Player player in players)
                    {
                        player.score = startingScore;
                    }
                    currentPlayerIndex = 0;
                    projectiles.Clear();
                }
            }
            else
            {
                terrain.Draw();
                foreach (Player player in players)
                {
                    player.Draw();
                    DrawScore(player);
                }
                foreach (Ammus projectile in projectiles)
                {
                    projectile.Draw();
                }
                DrawChargeBar(currentPlayer);

                DrawSelectedBullet(ammusTypes[selectedBulletIndex]);
            }

            Raylib.EndDrawing();
        }

        Raylib.UnloadSound(explosionSound);
        Raylib.UnloadSound(cannonShootSound);

        // De-Initialization
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }

    public static void DrawChargeBar(Player player)
    {
        int barWidth = 200;
        int barHeight = 20;
        int posX = (int)player.cannon.position.X - barWidth / 4;
        int posY = (int)player.cannon.position.Y - 100;
        Raylib.DrawRectangle(posX, posY, barWidth, barHeight, Raylib_cs.Color.Gray);
        Raylib.DrawRectangle(posX, posY, (int)(barWidth * (player.chargeLevel / maxCharge)), barHeight, Raylib_cs.Color.Green);
        Raylib.DrawRectangleLines(posX, posY, barWidth, barHeight, Raylib_cs.Color.Black);
    }

    static void DrawScore(Player player)
    {
        int posX = (int)player.cannon.position.X - 20;
        int posY = (int)player.cannon.position.Y - 200;
        Raylib.DrawText($"{player.score}", posX, posY, 20, Raylib_cs.Color.Black);
    }

    static void DrawSelectedBullet(AmmusType selectedBullet)
    {
        Raylib.DrawText($"Selected Bullet: {selectedBullet.name}", 10, 10, 20, Raylib_cs.Color.Black);
    }

    // Load Ammo types from JSON
    static List<AmmusType> LoadAmmusTypes(string filename)
    {
        string jsonString = File.ReadAllText(filename);
        return JsonConvert.DeserializeObject<List<AmmusType>>(jsonString);
    }
}

class Ammus
{
    public Vector2 position;
    public Vector2 velocity;
    public float explosionRadius;
    public Raylib_cs.Color color;

    public Ammus(Vector2 pos, Vector2 vel, float radius, Raylib_cs.Color col)
    {
        position = pos;
        velocity = vel;
        explosionRadius = radius;
        color = col;
    }

    public void Update()
    {
        velocity.Y += Program.gravity;
        position.X += velocity.X;
        position.Y += velocity.Y;
    }

    public void Draw()
    {
        Raylib.DrawCircleV(position, 5, Raylib_cs.Color.Black);
    }
}

class Tykki
{
    public Vector2 position;
    public float angle;
    public Raylib_cs.Color color;

    public Tykki(Vector2 pos, Raylib_cs.Color col)
    {
        position = pos;
        angle = 0;
        color = col;
    }

    public void Rotate(float delta)
    {
        angle += delta;
    }

    public void Draw()
    {
        Raylib.DrawRectanglePro(new RayRectangle(position.X, position.Y, 40, 10), new Vector2(0, 5), angle, color);
    }
}

class Maasto
{
    public List<RayRectangle> blocks;

    public Maasto()
    {
        blocks = new List<RayRectangle>();
        // Generate varied terrain
        Random rand = new Random();
        for (int i = 0; i < Program.screenWidth; i += 40)
        {
            int heightVariation = rand.Next(-20, 20); // Random variation in height
            blocks.Add(new RayRectangle(i, Program.screenHeight - Program.terrainHeight + heightVariation, 40, Program.terrainHeight - heightVariation));
        }
    }

    public void Draw()
    {
        foreach (RayRectangle block in blocks)
        {
            Raylib.DrawRectangleRec(block, Raylib_cs.Color.DarkGreen);
        }
    }

    public float GetHeightAt(float x)
    {
        foreach (RayRectangle block in blocks)
        {
            if (x >= block.X && x <= block.X + block.Width)
            {
                return block.Y;
            }
        }
        return Program.screenHeight - Program.terrainHeight; // Default to ground level if not found
    }

    public void DestroyTerrain(Vector2 position, float radius)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (Raylib.CheckCollisionCircleRec(position, radius, blocks[i]))
            {
                // Create a temporary variable for the block
                RayRectangle block = blocks[i];

                // Modify the temporary variable
                block.Height -= radius;
                if (block.Height < 0) block.Height = 0;
                block.Y = Program.screenHeight - block.Height;

                // Assign the modified block back to the list
                blocks[i] = block;
            }
        }
    }
}

class Player
{
    public Tykki cannon;
    public float chargeLevel = 0;
    public int score;
    public Raylib_cs.Color col { get { return cannon.color; } }

    public Player(Vector2 pos, Raylib_cs.Color col)
    {
        cannon = new Tykki(pos, col);
        chargeLevel = 0;
        score = Program.startingScore; // Initialize score to startingScore
    }

    public void Draw()
    {
        cannon.Draw();
    }
}

class AmmusType
{
    public string name;
    public float radius;
    public Color color;
}

struct Color
{
    public int r;
    public int g;
    public int b;
}