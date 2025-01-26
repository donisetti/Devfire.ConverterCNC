using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using CncConverter.Models;
using Devfire.ConverterCNC.Models;

namespace XmlConverter.Tests
{
    public class XmlConverterTests
    {
        private readonly CncConverter.Services.XmlConverter _xmlConverter;

        public XmlConverterTests()
        {
            _xmlConverter = new CncConverter.Services.XmlConverter();
        }

        [Fact]
        public void ConvertToXml_ShouldThrowException_WhenPanelInputIsNull()
        {
            Action action = () => _xmlConverter.ConvertToXml(null);
            action.Should().Throw<Exception>().WithMessage("O objeto 'PanelInput' não pode ser nulo.");
        }

        [Fact]
        public void ConvertToXml_ShouldThrowException_WhenIdIsMissing()
        {
            var panel = new PanelInput
            {
                c = 500,
                l = 300,
                machining_Compiled = new MachiningCompiled { z = 15 }
            };

            Action action = () => _xmlConverter.ConvertToXml(panel);
            action.Should().Throw<Exception>().WithMessage("O campo 'Id' é obrigatório.");
        }

        [Fact]
        public void ConvertToXml_ShouldThrowException_WhenMachiningCompiledIsMissing()
        {
            var panel = new PanelInput
            {
                id = "88040",
                c = 500,
                l = 300
            };

            Action action = () => _xmlConverter.ConvertToXml(panel);
            action.Should().Throw<Exception>().WithMessage("O campo 'MachiningCompiled' é obrigatório.");
        }

        [Theory(DisplayName = $"Validar os atributos do json")]
        [InlineData("88040", 513.3, 135.25, 15, true)]
        [InlineData("88498", 400, 300, 18, true)]
        [InlineData("88499", 690, 582, 15, true)]
        public void ConvertToXml_ShouldValidateAttributes(string id, double length, double width, double thickness, bool isProduce)
        {
            // Arrange
            var panel = new PanelInput
            {
                id = id,
                c = (float?)length,
                l = (float?)width,
                machining_Compiled = new MachiningCompiled { z = thickness }
            };

            // Act
            var resultXml = _xmlConverter.ConvertToXml(panel);

            // Assert
            var resultDocument = XDocument.Parse(resultXml);
            var panelElement = resultDocument.Root.Element("Project")?.Element("Panels")?.Element("Panel");

            panelElement.Should().NotBeNull();
            panelElement!.Attribute("ID")?.Value.Should().Be(id);
            panelElement.Attribute("Length")?.Value.Should().Be(length.ToString());
            panelElement.Attribute("Width")?.Value.Should().Be(width.ToString());
            panelElement.Attribute("Thickness")?.Value.Should().Be(thickness.ToString());
            panelElement.Attribute("IsProduce")?.Value.Should().Be(isProduce.ToString().ToLower());
        }

        [Fact]
        public void ConvertToXml_ShouldGenerateExpectedXmlStructure()
        {
            // Arrange
            var panel = new PanelInput
            {
                id = "88040",
                c = 513.3f,
                l = 135.25f,
                machining_Compiled = new MachiningCompiled
                {
                    z = 15,
                    //verticalDrills = new List<Machining>
                    //        {
                    //            new Machining { x = 63, y = 67, diameter = 3,  depth = 1.5f, face = "e" },
                    //            new Machining { x = 287, y = 67, diameter = 3, depth = 1.5f, face = "e" },
                    //            new Machining { x = 479, y = 67, diameter = 3, depth = 1.5f, face = "e" },
                    //            new Machining { x = 479, y = 67, diameter = 3, depth = 1.5f, face = "e" }
                    //        },
                    //verticalDrills = new List<Drill>
                    //        {
                    //            new Machining { x = 63, y = 67, diameter = 3,  depth = 1.5f, face = "e" },
                    //            new Machining { x = 287, y = 67, diameter = 3, depth = 1.5f, face = "e" },
                    //            new Machining { x = 479, y = 67, diameter = 3, depth = 1.5f, face = "e" },
                    //            new Machining { x = 479, y = 67, diameter = 3, depth = 1.5f, face = "e" }
                    //        },
                    //furrowMachining = new Machining
                    //{
                    //    face = "i",
                    //    depth = 8,
                    //    diameter = 6.7,
                    //    x = 0
                    //}
                }
            };

            var expectedXmlPath = Path.Combine("TestData", "88040.xml");
            var expectedXml = File.ReadAllText(expectedXmlPath);

            // Act
            var resultXml = _xmlConverter.ConvertToXml(panel);

            // Assert
            var expectedDocument = XDocument.Parse(expectedXml);
            var resultDocument = XDocument.Parse(resultXml);

            XNode.DeepEquals(resultDocument, expectedDocument).Should().BeTrue("A estrutura XML gerada deve corresponder à esperada.");
        }
    }
}
