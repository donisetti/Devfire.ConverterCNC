using CncConverter.Models;

namespace Devfire.ConverterCNC.Models;

public class Machining_Compiled
{
    public int x { get; set; } = 0;
    public int y { get; set; } = 0;
    public int z { get; set; } = 0;
    public string alias { get; set; } = string.Empty;
    public int startSide { get; set; } = 0;
    public Horizontaldrill[] horizontalDrills { get; set; } = new Horizontaldrill[0];
    public Verticaldrill[] verticalDrills { get; set; } = new Verticaldrill[0];
    public FurrowMachining? furrowMachining { get; set; } = null;
    public FurrowMachining? furrowMachiningPair { get; set; } = null;
    public object[] millingCubes { get; set; } = new object[0]; 

}
