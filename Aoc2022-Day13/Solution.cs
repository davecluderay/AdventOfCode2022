namespace Aoc2022_Day13;

internal class Solution
{
    public string Title => "Day 13: Distress Signal";

    public object PartOne()
    {
        var result = ReadInSections()
                     .Select((p, i) => (Index: i + 1,
                                        IsCorrect: p[0].CompareTo(p[1]) <= 0))
                     .Where(p => p.IsCorrect)
                     .Sum(p => p.Index);
        return result;
    }

    public object PartTwo()
    {
        var dividers = new[] { Value.Read("[[2]]"), Value.Read("[[6]]") };
        var result = ReadInSections()
                     .SelectMany(p => p)
                     .Concat(dividers)
                     .Order()
                     .Select((p, i) => (Index: i + 1,
                                        IsDivider: dividers.Contains(p)))
                     .Where(p => p.IsDivider)
                     .Aggregate(1, (a, v) => a * v.Index);
        return result;
    }

    private static IEnumerable<Value[]> ReadInSections(string? fileName = null)
        => InputFile.ReadInSections(fileName)
                    .Select(s => s.Select(Value.Read)
                                  .ToArray());
}
