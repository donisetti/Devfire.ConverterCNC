namespace Devfire.ConverterCNC.Models;

public class Horizontaldrill
{
    public int x { get; set; } = 0;
    public int y { get; set; } = 0;
    public float z { get; set; } = 0;
    public string? face { get; set; } = "i";
    public int depth { get; set; } = 0;
    public int corner { get; set; } = 0;
    public int diameter { get; set; } = 0;
    public string? direction { get; set; } = "XP";
}
