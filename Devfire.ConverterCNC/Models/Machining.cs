namespace Devfire.ConverterCNC.Models;

public class Machining
{
    public string? type { get; set; } = "2";
    public string? isGenCode { get; set; } = "2";
    public int face { get; set; } = 0;
    public float x { get; set; } = 0;
    public int y { get; set; } = 0;
    public int diameter { get; set; } = 0;
    public float depth { get; set; } = 0;



}


//    <Machining Type="2" IsGenCode="2" Face="5" X="23.5" Y="22" Diameter="15" Depth="12.5"/>
