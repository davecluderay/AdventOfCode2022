using System.Text.RegularExpressions;

namespace Aoc2022_Day04;

internal class Solution
{
    public string Title => "Day 4: Camp Cleanup";

    public object? PartOne()
    {
        var pairs = ReadElfPairs();
        var result = pairs.Count(p => p.Elf1.CompletelyContains(p.Elf2) ||
                                      p.Elf2.CompletelyContains(p.Elf1));
        return result;
    }

    public object? PartTwo()
    {
        var pairs = ReadElfPairs();
        var result = pairs.Count(p => p.Elf1.Overlaps(p.Elf2));
        return result;
    }

    private ElfPair[] ReadElfPairs()
        => InputFile.ReadAllLines()
                    .Select(ElfPair.Read)
                    .ToArray();
}

internal record ElfPair(AreaRange Elf1, AreaRange Elf2)
{
    public static ElfPair Read(string text)
    {
        var ranges = text.Split(',');
        return new ElfPair(AreaRange.Read(ranges[0]), AreaRange.Read(ranges[1]));
    }
}

internal partial record AreaRange(int Start, int End)
{
    [GeneratedRegex(@"^(?<Start>\d+)-(?<End>\d+)$", RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking)]
    private static partial Regex Pattern();

    public bool CompletelyContains(AreaRange other)
        => Start <= other.Start && End >= other.End;

    public bool Overlaps(AreaRange other)
        => Start <= other.End && End >= other.Start;

    public static AreaRange Read(string input)
    {
        var match = Pattern().Match(input);
        if (!match.Success) throw new FormatException($"Not a range: {input}");
        return new AreaRange(Convert.ToInt32(match.Groups["Start"].Value),
                             Convert.ToInt32(match.Groups["End"].Value));
    }
}
