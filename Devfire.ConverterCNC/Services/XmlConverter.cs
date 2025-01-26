using System.Xml.Linq;
using CncConverter.Models;

namespace CncConverter.Services
{
    public class XmlConverter
    {
        public string ConvertToXml(PanelInput panel)
        {
            if (panel == null)
                throw new Exception("O objeto 'PanelInput' não pode ser nulo.");

            if (string.IsNullOrEmpty(panel.id))
                throw new Exception("O campo 'Id' é obrigatório.");

            if (panel.machining_Compiled == null)
                throw new Exception("O campo 'MachiningCompiled' é obrigatório.");

            if (panel.machining_Compiled.furrowMachining == null)
                throw new Exception("O campo 'furrowMachining' é obrigatório.");

            var root = new XElement("Root",
                new XAttribute("Cad", "BuiltInCad"),
                new XAttribute("version", "2.0"),
                new XElement("Project",
                    new XElement("Panels",
                        new XElement("Panel",
                            new XAttribute("IsProduce", true),
                            new XAttribute("ID", panel.id ?? string.Empty),
                            new XAttribute("Length", panel.c ?? 0),
                            new XAttribute("Width", panel.l ?? 0),
                            new XAttribute("Thickness", panel.machining_Compiled.z ?? 0),
                            new XElement("Machines",
                                // Processar HorizontalDrills
                                panel.machining_Compiled.horizontalDrills?.Select(m => GenerateMachiningElement(m, "horizontalDrill")) ?? Enumerable.Empty<XElement>(),

                                // Processar VerticalDrills
                                panel.machining_Compiled.verticalDrills?.Select(m => GenerateMachiningElement(m, "verticalDrill")) ?? Enumerable.Empty<XElement>(),

                                // Processar FurrowMachining
                                GenerateFurrowMachiningElement(panel.machining_Compiled.furrowMachining) ?? null
                            )
                        )
                    )
                )
            );

            return root.ToString();
        }

        private static XElement GenerateMachiningElement(CncConverter.Models.Machining machining, string type)
        {
            // Verificar se o objeto 'machining' é nulo antes de acessar as propriedades
            if (machining == null)
                return null;

            return new XElement("Machining",
                new XAttribute("Type", type),
                new XAttribute("X", machining.x),
                new XAttribute("Y", machining.y),
                new XAttribute("Diameter", machining.diameter),
                new XAttribute("Depth", machining.depth),
                new XAttribute("Face", machining.face)
            );
        }

        private static XElement GenerateFurrowMachiningElement(CncConverter.Models.FurrowMachining furrowMachining)
        {
            // Verificar se o objeto 'furrowMachining' é nulo antes de acessar as propriedades
            if (furrowMachining == null)
                return null;

            return new XElement("FurrowMachining",
                new XAttribute("Face", furrowMachining.face ?? "Undefined"),
                new XAttribute("Depth", furrowMachining.depth),
                new XAttribute("Width", furrowMachining.width),
                new XAttribute("Distance", furrowMachining.distance)
            );
        }
    }

}
