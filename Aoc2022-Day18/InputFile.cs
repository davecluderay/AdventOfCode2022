using System.Reflection;

namespace Aoc2022_Day18;

internal static class InputFile
{
    public static string[] ReadAllLines(string? fileName = null)
    {
        var directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? ".";
        var filePath      = Path.Combine(directoryPath, fileName ?? "input.txt");
        if (!File.Exists(filePath)) throw new Exception($"File not found: {filePath}");

        return File.ReadAllLines(filePath);
    }

    public static IEnumerable<string[]> ReadInSections(string? fileName = null)
    {
        var section = new List<string>();
        foreach (var line in ReadAllLines(fileName))
        {
            if (string.IsNullOrEmpty(line))
            {
                if (section.Count > 0)
                {
                    yield return section.ToArray();
                    section.Clear();
                }
            }
            else
            {
                section.Add(line);
            }
        }

        if (section.Count > 0) yield return section.ToArray();
    }
        
    public static string ReadAllText(string? fileName = null)
    {
        var directoryPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? ".";
        var filePath      = Path.Combine(directoryPath, fileName ?? "input.txt");
        if (!File.Exists(filePath)) throw new Exception($"File not found: {filePath}");

        return File.ReadAllText(filePath);
    }
}