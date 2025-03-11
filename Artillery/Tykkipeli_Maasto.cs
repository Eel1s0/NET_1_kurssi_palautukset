using System.Collections.Generic;
using System.Drawing;

public class Maasto
{
    public List<Rectangle> Palat { get; set; }

    public Maasto(int leveys, int korkeus, int palaLeveys)
    {
        Palat = new List<Rectangle>();
        int x = 0;
        while (x < leveys)
        {
            Palat.Add(new Rectangle(x, korkeus - 50, palaLeveys, 50)); // Example height
            x += palaLeveys;
        }
    }

    // Method to check collision with a shot and destroy terrain
    public void TarkistaTörmäys(Ammus ammus, Point sijainti)
    {
        int indeksi = sijainti.X / Palat[0].Width;
        if (indeksi >= 0 && indeksi < Palat.Count)
        {
            // Simple destruction logic
            Palat[indeksi] = new Rectangle(Palat[indeksi].X, Palat[indeksi].Y, Palat[indeksi].Width, Palat[indeksi].Height - ammus.Räjähdysalue);
        }
    }
}