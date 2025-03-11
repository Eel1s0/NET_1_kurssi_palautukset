using System.Collections.Generic;

public class Pelaaja
{
    public List<Tykki> Tykit { get; set; }

    public Pelaaja(List<Tykki> tykit)
    {
        Tykit = tykit;
    }

    // Method to move player left or right
    public void Liiku(int suunta)
    {
        foreach (var tykki in Tykit)
        {
            tykki.Sijainti = new System.Drawing.Point(tykki.Sijainti.X + suunta, tykki.Sijainti.Y);
        }
    }
}