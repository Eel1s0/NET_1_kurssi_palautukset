using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Raylib_cs;

public class Peli
{
    public List<Pelaaja> Pelaajat { get; set; }
    public Maasto Maasto { get; set; }
    public List<Ammus> Ammukset { get; set; }

    private Sound shootSound;
    private Sound explosionSound;
    private Sound victorySound;

    public Peli()
    {
        Raylib.InitWindow(800, 600, "Tykkipeli");
        Raylib.InitAudioDevice();

        shootSound = Raylib.LoadSound("Sounds/shoot.wav");
        explosionSound = Raylib.LoadSound("Sounds/explosion.wav");
        victorySound = Raylib.LoadSound("Sounds/victory.wav");

        Pelaajat = new List<Pelaaja>();
        Maasto = new Maasto(800, 600, 80); // Example values
        Ammukset = new List<Ammus>();
        LataaAmmukset();
    }

    // Method to load ammunition types from JSON files
    private void LataaAmmukset()
    {
        string[] files = Directory.GetFiles("Ammukset", "*.json");
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            Ammus ammus = JsonConvert.DeserializeObject<Ammus>(json);
            Ammukset.Add(ammus);
        }
    }

    // Method to start the game
    public void Aloita()
    {
        // Initialize players, cannons, etc.
        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib_cs.Color.RayWhite);

            // Draw game objects
            foreach (var pala in Maasto.Palat)
            {
                Raylib.DrawRectangleRec(pala, Raylib_cs.Color.DarkGreen);
            }

            foreach (var pelaaja in Pelaajat)
            {
                foreach (var tykki in pelaaja.Tykit)
                {
                    // Draw cannon
                    Raylib.DrawRectangle(tykki.Sijainti.X, tykki.Sijainti.Y, 10, 20, Raylib_cs.Color.Black);
                }
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }

    // Method to handle shooting
    public void Ammu(Pelaaja pelaaja, Tykki tykki, Ammus ammus)
    {
        // Calculate shot trajectory, apply gravity, check collisions, etc.
        // Play shoot sound
        Raylib.PlaySound(shootSound);
        // Play explosion sound if collision detected
    }

    // Method to handle victory
    public void Voitto(Pelaaja voittaja)
    {
        // Play victory sound
        Raylib.PlaySound(victorySound);
    }
}