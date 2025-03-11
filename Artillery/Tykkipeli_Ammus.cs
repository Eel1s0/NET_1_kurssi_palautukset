using System.Drawing;

public class Ammus
{
    public int Räjähdysalue { get; set; }
    public Color Väri { get; set; }
    public float Paino { get; set; }
    public float Räjähdysvoima { get; set; }

    public Ammus(int räjähdysalue, Color väri, float paino, float räjähdysvoima)
    {
        Räjähdysalue = räjähdysalue;
        Väri = väri;
        Paino = paino;
        Räjähdysvoima = räjähdysvoima;
    }
}