using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devfire.ConverterCNC.Models
{
    public class Drill
    {
        public string? face { get; set; } = "i";
        public int corner { get; set; } = 0;
        public string? direction { get; set; } = "XP";
        public int x { get; set; } = 0;
        public int y { get; set; } = 0;
        public float z { get; set; } = 0;
        public float depth { get; set; } = 0;
        public int diameter { get; set; } = 0;
        public bool bolthole { get; set; } = false;
    }
}
