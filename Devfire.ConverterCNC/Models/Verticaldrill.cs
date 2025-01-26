namespace Devfire.ConverterCNC.Models;

public class Verticaldrill
{
    public float x { get; set; } = 0;
    public int y { get; set; } = 0;
    public string? face { get; set; } = string.Empty;
    public float depth { get; set; } = 0;
    public int corner { get; set; } = 0;
    public int diameter { get; set; } = 0;
    public bool bolthole { get; set; } = false;
}
