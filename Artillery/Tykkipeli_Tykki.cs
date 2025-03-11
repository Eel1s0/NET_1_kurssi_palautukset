using System.Drawing;

public class Tykki
{
    public Point Sijainti { get; set; }
    public float Suunta { get; set; } // In degrees
    public int Osumapisteet { get; set; }

    public Tykki(Point sijainti, float suunta, int osumapisteet)
    {
        Sijainti = sijainti;
        Suunta = suunta;
        Osumapisteet = osumapisteet;
    }

    // Method to rotate the cannon
    public void Käännä(float kulma)
    {
        Suunta += kulma;
        if (Suunta < 0) Suunta += 360;
        if (Suunta >= 360) Suunta -= 360;
    }
}