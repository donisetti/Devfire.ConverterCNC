using System.Collections.Generic;

namespace CncConverter.Models
{
    public class PanelInput
    {
        public string id { get; set; } = string.Empty;
        public float? c { get; set; } = 0;
        public float? l { get; set; } = 0;
        public string Function { get; set; } = string.Empty;
        public MachiningCompiled machining_Compiled { get; set; } = new MachiningCompiled();
    }

    public class MachiningCompiled
    {
       
        public float? x { get; set; } = 0;
        public float? y { get; set; } = 0;
        public float? z { get; set; } = 0;
        public int? StartSide { get; set; } = 0;
        public List<Machining> horizontalDrills { get; set; } = new List<Machining>();
        public List<Machining> verticalDrills { get; set; } = new List<Machining>();
        public FurrowMachining furrowMachining { get; set; } = new FurrowMachining();
       
        //public List<Machining> MillingCubes { get; set; } = new List<Machining>();

       
    }

    public class Machining
    {
        public double? x { get; set; }
        public double? y { get; set; }
        public double? z { get; set; }
        public string? face { get; set; }
        public double? depth { get; set; }
        public int? corner { get; set; }
        public double? diameter { get; set; }
        public string? direction { get; set; }
        public bool? bolthole { get; set; }
    }
}
