namespace Aoc2022_Day03;

internal class Solution
{
    public string Title => "Day 3: Rucksack Reorganization";

    public object? PartOne()
    {
        var rucksacks = InputFile.ReadAllLines()
                                 .Select(Rucksack.Read)
                                 .ToArray();
        var result = rucksacks.Select(FindIncorrectlyPackedItemType)
                              .Sum(CalculatePriority);
        return result;
    }

    public object? PartTwo()
    {
        var groups = InputFile.ReadAllLines()
                              .Select(Rucksack.Read)
                              .Chunk(3)
                              .ToArray();
        var result = groups.Select(FindBadgeItemType)
                           .Sum(CalculatePriority);
        return result;
    }

    private char FindIncorrectlyPackedItemType(Rucksack rucksack)
        => rucksack.Compartment1.Keys.First(rucksack.Compartment2.ContainsKey);

    private char FindBadgeItemType(Rucksack[] rucksacks)
        => rucksacks.First().Compartment1.Keys.Concat(rucksacks.First().Compartment2.Keys)
                    .ToHashSet()
                    .First(t => rucksacks.All(r => r.Contains(t)));

    private int CalculatePriority(char itemType)
        => char.IsLower(itemType)
               ? itemType - 'a' + 1
               : itemType - 'A' + 27;

    private record Rucksack(IDictionary<char, int> Compartment1,
                            IDictionary<char, int> Compartment2)
    {
        public bool Contains(char itemType)
            => Compartment1.ContainsKey(itemType) || Compartment2.ContainsKey(itemType);
        public static Rucksack Read(string line)
            => new(line.Take(line.Length / 2)
                       .GroupBy(t => t)
                       .ToDictionary(g => g.Key, g => g.Count()),
                   line.Skip(line.Length / 2)
                       .GroupBy(t => t)
                       .ToDictionary(g => g.Key, g => g.Count()));
    }
}
