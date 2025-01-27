O objetivo é criar um conversor que gere um XML similar para os arquivos JSON fornecidos pelo CAD, tratando valores nulos e inválidos, além de garantir que a saída seja compatível com o CNC Router.

Vou detalhar as etapas e fornecer o código necessário para implementar e testar o programa no .NET Core 8 (C#)

#Plano de Implementação
R##equisitos Principais:

Ler arquivos JSON com as propriedades das peças.
Validar os dados (exemplo: valores mínimos e máximos, campos obrigatórios, etc.).
Ignorar valores nulos ou inválidos, mas continuar processando os dados válidos.
Gerar XML no formato esperado para o CNC Router.
Pontos de Validação:

Tamanhos mínimos e máximos: Exemplo, Length, Width, Thickness.
Propriedades obrigatórias: ID, IsProduce, Machining.
Tratamento de valores nulos ou inválidos: Substituir valores inválidos por padrões aceitáveis (ex.: 0 para dimensões ausentes).
Arquitetura do Projeto:

```


#Implementação do Conversor

##1. Modelos de Dados (JSON)

###Arquivo: Models/PanelInput.cs

´´´

#Implementação do Conversor

##1. Modelos de Dados (JSON)

###Arquivo: Models/PanelInput.cs

´´´
using System.Collections.Generic;

namespace CncConverter.Models
{
    public class PanelInput
    {
        public string Id { get; set; }
        public int? MachiningPoint { get; set; }
        public bool? IsProduce { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Thickness { get; set; }
        public List<Machining> Machining { get; set; }
    }

    public class Machining
    {
        public string Type { get; set; }
        public string IsGenCode { get; set; }
        public string Face { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public double? Diameter { get; set; }
        public double? Depth { get; set; }
        public MillPreDefinedRectangle MillPreDefinedRectangle { get; set; }
        public List<Line> Lines { get; set; }
    }

    public class MillPreDefinedRectangle
    {
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
    }

    public class Line
    {
        public int? LineID { get; set; }
        public double? EndX { get; set; }
        public double? EndY { get; set; }
        public double? Angle { get; set; }
    }
}
2. Conversor XML
Arquivo: Services/XmlConverter.cs

csharp
Copiar
Editar
using System.Xml.Linq;
using CncConverter.Models;

namespace CncConverter.Services
{
    public class XmlConverter
    {
        public string ConvertToXml(PanelInput panel)
        {
            // Validação básica
            if (string.IsNullOrEmpty(panel.Id))
                throw new Exception("O campo 'Id' é obrigatório.");
            if (panel.Machining == null || panel.Machining.Count == 0)
                throw new Exception("O campo 'Machining' é obrigatório e deve conter pelo menos uma operação.");

            var root = new XElement("Root",
                new XAttribute("Cad", "BuiltInCad"),
                new XAttribute("version", "2.0"),
                new XElement("Project",
                    new XElement("Panels",
                        new XElement("Panel",
                            new XAttribute("IsProduce", panel.IsProduce ?? false),
                            new XAttribute("ID", panel.Id),
                            new XAttribute("Length", panel.Length ?? 0),
                            new XAttribute("Width", panel.Width ?? 0),
                            new XAttribute("Thickness", panel.Thickness ?? 0),
                            new XAttribute("MachiningPoint", panel.MachiningPoint ?? 0),
                            new XElement("Machines",
                                panel.Machining.Select(m =>
                                    new XElement("Machining",
                                        new XAttribute("Type", m.Type ?? "Undefined"),
                                        new XAttribute("IsGenCode", m.IsGenCode ?? "Undefined"),
                                        new XAttribute("Face", m.Face ?? "Undefined"),
                                        new XAttribute("X", m.X ?? 0),
                                        new XAttribute("Y", m.Y ?? 0),
                                        new XAttribute("Z", m.Z ?? 0),
                                        new XAttribute("Diameter", m.Diameter ?? 0),
                                        new XAttribute("Depth", m.Depth ?? 0)
                                    )
                                )
                            )
                        )
                    )
                )
            );

            return root.ToString();
        }
    }
}
3. Programa Principal
Arquivo: Program.cs

csharp
Copiar
Editar
using System;
using System.IO;
using System.Text.Json;
using CncConverter.Models;
using CncConverter.Services;

class Program
{
    static void Main(string[] args)
    {
        var inputPath = Path.Combine(Directory.GetCurrentDirectory(), "Fixtures");
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Output");

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var converter = new XmlConverter();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        foreach (var file in Directory.GetFiles(inputPath, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var panel = JsonSerializer.Deserialize<PanelInput>(json, options);

                if (panel == null)
                    throw new Exception("O JSON está vazio ou malformado.");

                var xml = converter.ConvertToXml(panel);
                var outputFile = Path.Combine(outputPath, $"{panel.Id}.xml");
                File.WriteAllText(outputFile, xml);

                Console.WriteLine($"Arquivo XML gerado: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {file}: {ex.Message}");
            }
        }
    }
}
4. Testes Automatizados
Arquivo: Test/XmlConverterTests.cs Os testes validarão cenários válidos e inválidos, como:

JSONs com valores nulos.
Dimensões fora dos limites esperados.
using System.Collections.Generic;

namespace CncConverter.Models
{
    public class PanelInput
    {
        public string Id { get; set; }
        public int? MachiningPoint { get; set; }
        public bool? IsProduce { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Thickness { get; set; }
        public List<Machining> Machining { get; set; }
    }

    public class Machining
    {
        public string Type { get; set; }
        public string IsGenCode { get; set; }
        public string Face { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
        public double? Diameter { get; set; }
        public double? Depth { get; set; }
        public MillPreDefinedRectangle MillPreDefinedRectangle { get; set; }
        public List<Line> Lines { get; set; }
    }

    public class MillPreDefinedRectangle
    {
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
    }

    public class Line
    {
        public int? LineID { get; set; }
        public double? EndX { get; set; }
        public double? EndY { get; set; }
        public double? Angle { get; set; }
    }
}
2. Conversor XML
Arquivo: Services/XmlConverter.cs

csharp
Copiar
Editar
using System.Xml.Linq;
using CncConverter.Models;

namespace CncConverter.Services
{
    public class XmlConverter
    {
        public string ConvertToXml(PanelInput panel)
        {
            // Validação básica
            if (string.IsNullOrEmpty(panel.Id))
                throw new Exception("O campo 'Id' é obrigatório.");
            if (panel.Machining == null || panel.Machining.Count == 0)
                throw new Exception("O campo 'Machining' é obrigatório e deve conter pelo menos uma operação.");

            var root = new XElement("Root",
                new XAttribute("Cad", "BuiltInCad"),
                new XAttribute("version", "2.0"),
                new XElement("Project",
                    new XElement("Panels",
                        new XElement("Panel",
                            new XAttribute("IsProduce", panel.IsProduce ?? false),
                            new XAttribute("ID", panel.Id),
                            new XAttribute("Length", panel.Length ?? 0),
                            new XAttribute("Width", panel.Width ?? 0),
                            new XAttribute("Thickness", panel.Thickness ?? 0),
                            new XAttribute("MachiningPoint", panel.MachiningPoint ?? 0),
                            new XElement("Machines",
                                panel.Machining.Select(m =>
                                    new XElement("Machining",
                                        new XAttribute("Type", m.Type ?? "Undefined"),
                                        new XAttribute("IsGenCode", m.IsGenCode ?? "Undefined"),
                                        new XAttribute("Face", m.Face ?? "Undefined"),
                                        new XAttribute("X", m.X ?? 0),
                                        new XAttribute("Y", m.Y ?? 0),
                                        new XAttribute("Z", m.Z ?? 0),
                                        new XAttribute("Diameter", m.Diameter ?? 0),
                                        new XAttribute("Depth", m.Depth ?? 0)
                                    )
                                )
                            )
                        )
                    )
                )
            );

            return root.ToString();
        }
    }
}
3. Programa Principal
Arquivo: Program.cs

csharp
Copiar
Editar
using System;
using System.IO;
using System.Text.Json;
using CncConverter.Models;
using CncConverter.Services;

class Program
{
    static void Main(string[] args)
    {
        var inputPath = Path.Combine(Directory.GetCurrentDirectory(), "Fixtures");
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Output");

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var converter = new XmlConverter();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        foreach (var file in Directory.GetFiles(inputPath, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var panel = JsonSerializer.Deserialize<PanelInput>(json, options);

                if (panel == null)
                    throw new Exception("O JSON está vazio ou malformado.");

                var xml = converter.ConvertToXml(panel);
                var outputFile = Path.Combine(outputPath, $"{panel.Id}.xml");
                File.WriteAllText(outputFile, xml);

                Console.WriteLine($"Arquivo XML gerado: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {file}: {ex.Message}");
            }
        }
    }
}

´´´
#4. Testes Automatizados

###Arquivo: Test/XmlConverterTests.cs Os testes validarão cenários válidos e inválidos, como:

JSONs com valores nulos.
Dimensões fora dos limites esperados.

´´´
CncConverter/
├── Program.cs                 # Ponto de entrada do programa
├── Models/
│   ├── PanelInput.cs          # Modelo de entrada JSON
│   ├── Machining.cs           # Submodelos (Drill, Furrow, etc.)
├── Services/
│   ├── XmlConverter.cs        # Conversor JSON para XML
│   ├── Validator.cs           # Validação de dados
├── Fixtures/
│   ├── 90001.json             # Arquivo JSON de entrada
│   ├── 90002.json             # Arquivo JSON de entrada
├── Output/                    # XMLs gerados
│   ├── (gerado durante a execução)
├── Test/
│   ├── XmlConverterTests.cs   # Testes automatizados com xUnit


#Plano Geral da Solução
##Estrutura do Projeto:

#Projeto em .NET 8 Console Application.

Classes e métodos organizados para leitura de JSON, conversão e geração do XML.
Testes automatizados usando xUnit.
``
Componentes Principais:

Classe PanelInput: Representa a entrada JSON.
Classe XmlConverter: Contém a lógica para converter os dados JSON em XML.
Ponto de entrada (Program.cs): Lê os arquivos JSON e escreve os XMLs gerados.
Ferramentas e Tecnologias:

System.Text.Json: Para desserializar os arquivos JSON.
System.Xml.Linq: Para gerar os XMLs.
xUnit: Para testes unitários.
Estrutura do Projeto
plaintext
Copiar
Editar
cnc-converter/
├── Program.cs                 # Ponto de entrada do programa
├── Models/
│   ├── PanelInput.cs          # Modelo para os dados do JSON
│   ├── Machining.cs           # Modelo para operações de usinagem
├── Services/
│   ├── XmlConverter.cs        # Lógica de conversão JSON para XML
├── Test/
│   ├── XmlConverterTests.cs   # Testes unitários para a conversão
├── Fixtures/
│   ├── 90001.json             # Arquivo de exemplo JSON
│   ├── 90002.json             # Outro exemplo JSON
│   ├── 90003.json             # Outro exemplo JSON
Passo a Passo da Implementação
1. Criar o Projeto
bash
Copiar
Editar
dotnet new console -n CncConverter
cd CncConverter
2. Adicionar a Classe PanelInput
No diretório Models/PanelInput.cs:

csharp
Copiar
Editar
using System.Collections.Generic;

namespace CncConverter.Models
{
    public class PanelInput
    {
        public string Id { get; set; }
        public int C { get; set; }
        public int L { get; set; }
        public MachiningData MachiningCompiled { get; set; }
    }

    public class MachiningData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public List<Drill> HorizontalDrills { get; set; }
        public List<Drill> VerticalDrills { get; set; }
        public List<Furrow> FurrowMachining { get; set; }
    }

    public class Drill
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Depth { get; set; }
        public int Diameter { get; set; }
        public int Face { get; set; }
    }

    public class Furrow
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }
    }
}
3. Criar a Classe XmlConverter
No diretório Services/XmlConverter.cs:

csharp
Copiar
Editar
using System.Xml.Linq;
using CncConverter.Models;

namespace CncConverter.Services
{
    public class XmlConverter
    {
        public string ConvertToXml(PanelInput panel)
        {
            var root = new XElement("Root",
                new XElement("Project",
                    new XElement("Panel",
                        new XAttribute("ID", panel.Id),
                        new XAttribute("Length", panel.MachiningCompiled.X),
                        new XAttribute("Width", panel.MachiningCompiled.Y),
                        new XAttribute("Thickness", panel.MachiningCompiled.Z),
                        new XElement("Machines",
                            panel.MachiningCompiled.HorizontalDrills.Select(drill =>
                                new XElement("Machining",
                                    new XAttribute("Type", "1"),
                                    new XAttribute("X", drill.X),
                                    new XAttribute("Y", drill.Y),
                                    new XAttribute("Z", drill.Z),
                                    new XAttribute("Diameter", drill.Diameter),
                                    new XAttribute("Depth", drill.Depth),
                                    new XAttribute("Face", drill.Face))),
                            panel.MachiningCompiled.VerticalDrills.Select(drill =>
                                new XElement("Machining",
                                    new XAttribute("Type", "2"),
                                    new XAttribute("X", drill.X),
                                    new XAttribute("Y", drill.Y),
                                    new XAttribute("Diameter", drill.Diameter),
                                    new XAttribute("Depth", drill.Depth),
                                    new XAttribute("Face", drill.Face))),
                            panel.MachiningCompiled.FurrowMachining.Select(furrow =>
                                new XElement("Machining",
                                    new XAttribute("Type", "3"),
                                    new XAttribute("X", furrow.X),
                                    new XAttribute("Y", furrow.Y),
                                    new XAttribute("Z", furrow.Z),
                                    new XAttribute("Length", furrow.Length),
                                    new XAttribute("Width", furrow.Width),
                                    new XAttribute("Depth", furrow.Depth)))
                        )
                    )
                )
            );

            return root.ToString();
        }
    }
}
4. Criar o Program.cs
csharp
Copiar
Editar
using System;
using System.IO;
using System.Text.Json;
using CncConverter.Models;
using CncConverter.Services;

class Program
{
    static void Main(string[] args)
    {
        var inputPath = "Fixtures";
        var outputPath = "Output";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var converter = new XmlConverter();

        foreach (var file in Directory.GetFiles(inputPath, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var panelInput = JsonSerializer.Deserialize<PanelInput>(json);

                if (panelInput == null)
                    throw new Exception("Arquivo JSON inválido.");

                var xml = converter.ConvertToXml(panelInput);

                var outputFileName = Path.Combine(outputPath, $"{panelInput.Id}.xml");
                File.WriteAllText(outputFileName, xml);

                Console.WriteLine($"Arquivo XML gerado: {outputFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {file}: {ex.Message}");
            }
        }
    }
}
5. Criar Testes com xUnit
Adicione o projeto de testes:

bash
Copiar
Editar
dotnet new xunit -n CncConverter.Tests
dotnet add CncConverter.Tests reference CncConverter
No arquivo Test/XmlConverterTests.cs:

csharp
Copiar
Editar
using Xunit;
using CncConverter.Models;
using CncConverter.Services;

public class XmlConverterTests
{
    [Fact]
    public void ConvertToXml_ShouldGenerateValidXml()
    {
        var panel = new PanelInput
        {
            Id = "90001",
            MachiningCompiled = new MachiningData
            {
                X = 1000,
                Y = 2000,
                Z = 18,
                HorizontalDrills = new List<Drill>
                {
                    new Drill { X = 50, Y = 100, Z = 0, Diameter = 5, Depth = 10, Face = 1 }
                },
                VerticalDrills = new List<Drill>
                {
                    new Drill { X = 500, Y = 1000, Diameter = 8, Depth = 12, Face = 6 }
                },
                FurrowMachining = new List<Furrow>
                {
                    new Furrow { X = 100, Y = 300, Z = 0, Length = 500, Width = 50, Depth = 10 }
                }
            }
        };

        var converter = new XmlConverter();
        var xml = converter.ConvertToXml(panel);

        Assert.Contains("<Panel", xml);
        Assert.Contains("90001", xml);
        Assert.Contains("<Machining Type=\"1\"", xml);
    }
}

6. Executar o Programa
Para gerar os XMLs:

dotnet run


Para rodar os testes:


dotnet test

```
