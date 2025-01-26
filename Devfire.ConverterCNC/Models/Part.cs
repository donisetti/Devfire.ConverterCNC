namespace Devfire.ConverterCNC.Models;


public class Part
{
    public int c { get; set; } = 0;
    public int l { get; set; } = 0;
    public string? function { get; set; } = string.Empty;
    public string? id { get; set; } = string.Empty;
    public required Machining_Compiled machining_compiled { get; set; } = new Machining_Compiled();
}
