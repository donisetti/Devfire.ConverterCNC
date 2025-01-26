using System.Text.Json;
using CncConverter.Models;

// Diretório base dependendo do ambiente
var basePath = GetBasePath();

// Diretórios relativos ao diretório base
var inputPath = Path.Combine(basePath, "Fixtures");
var outputPath = Path.Combine(basePath, "Output");

// Verificar se o diretório de entrada existe
if (!Directory.Exists(inputPath))
{
    Console.WriteLine($"Erro: O diretório de entrada '{inputPath}' não foi encontrado.");
    return;
}

// Criar o diretório de saída, se não existir
if (!Directory.Exists(outputPath))
{
    Console.WriteLine($"Criando diretório de saída: {outputPath}");
    Directory.CreateDirectory(outputPath);
}

// Corrected the instantiation of XmlConverter
var converter = new CncConverter.Services.XmlConverter(); // Initialize the converter
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };



foreach (var file in Directory.GetFiles(inputPath, "*.json"))
{
    try
    {
        Console.WriteLine($"Processando arquivo: {file}");

        var json = File.ReadAllText(file);

        var panel = JsonSerializer.Deserialize<PanelInput>(json, options);

        if (panel == null)
        {
            throw new Exception("O arquivo JSON está vazio ou malformado.");
        }

        // Convert Part to PanelInput
        var panelInput = new PanelInput
        {
            // Map properties from Part to PanelInput
            c = panel.c,
            l = panel.l,
            Function = panel.Function,
            id = panel.id,
            machining_Compiled = panel.machining_Compiled // Corrected property name
        };

        // Converter para XML
        var xml = converter.ConvertToXml(panelInput);

        Console.WriteLine(xml);

        // Gerar o caminho do arquivo de saída
        var outputFile = Path.Combine(outputPath, $"{panel.id}.xml");

        // Escrever o arquivo XML
        File.WriteAllText(outputFile, xml);

      //  MoverValido(basePath, file);

        Console.WriteLine($">>>>>>>: {outputFile}");

        // quebra de linha crlf
        Console.WriteLine();
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        var erro = ex.Message;

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("------ ERRO -----------");
        Console.WriteLine($"Erro ao processar o arquivo '{file}': {erro}");
        Console.WriteLine("------------------------");

      //  MoverInvalido(basePath, file);

        Console.WriteLine();
        Console.WriteLine();

    }
}

// ...

// quebra de linha crlf
Console.WriteLine();


Console.WriteLine("Processamento concluído.");

Console.ReadLine();

string GetBasePath()
{
    // No ambiente de desenvolvimento (Visual Studio)
#if DEBUG
    return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
#else
        // Em produção (binário executável)
        return AppDomain.CurrentDomain.BaseDirectory;
#endif
}

static void MoverInvalido(string basePath, string file)
{
    // mover o arquivo para a pasta de invalidos
    var invalidPath = Path.Combine(basePath, "Invalidos");
    if (!Directory.Exists(invalidPath))
    {
        Directory.CreateDirectory(invalidPath);
    }


    var invalidFile = Path.Combine(invalidPath, Path.GetFileName(file));
    File.Move(file, invalidFile);


    Console.WriteLine($"Arquivo movido para a pasta de inválidos: {invalidFile}");
}

static void MoverValido(string basePath, string file)
{
    // mover o arquivo para a pasta de validos
    var validPath = Path.Combine(basePath, "Validos");
    if (!Directory.Exists(validPath))
    {
        Directory.CreateDirectory(validPath);
    }


    var validFile = Path.Combine(validPath, Path.GetFileName(file));
    File.Move(file, validFile);


    Console.WriteLine($"Arquivo movido para a pasta de válidos: {validFile}");
}