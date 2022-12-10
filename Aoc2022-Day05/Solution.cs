using System.Text.RegularExpressions;

namespace Aoc2022_Day05;

internal class Solution
{
    public string Title => "Day 5: Supply Stacks";

    public object PartOne()
    {
        var (stacks, movements) = ReadInput();

        foreach (var movement in movements)
        {
            for (var i = 0; i < movement.Quantity; i++)
            {
                stacks[movement.To].Push(stacks[movement.From].Pop());
            }
        }

        return new string(stacks.Select(s => s.Pop()).ToArray());
    }

    public object PartTwo()
    {
        var (stacks, movements) = ReadInput();

        var temp = new Stack<char>();
        foreach (var movement in movements)
        {
            for (var i = 0; i < movement.Quantity; i++)
            {
                temp.Push(stacks[movement.From].Pop());
            }

            while (temp.Any())
            {
                stacks[movement.To].Push(temp.Pop());
            }
        }

        return new string(stacks.Select(s => s.Pop()).ToArray());
    }

    private static (Stack<char>[] stacks, Movement[] movements) ReadInput()
    {
        var sections = InputFile.ReadInSections().ToArray();
        return (ReadStacks(sections[0]), ReadMovements(sections[1]));
    }

    private static Stack<char>[] ReadStacks(IReadOnlyList<string> section)
    {
        var count = (section.Last().Length + 1) / 4;
        var stacks = Enumerable.Range(0, count).Select(_ => new Stack<char>()).ToArray();

        for (var n = section.Count - 2; n >= 0; n--)
        for (var i = 0; i < count; i++)
        {
            var ch = section[n][1 + i * 4];
            if (!char.IsWhiteSpace(ch))
                stacks[i].Push(ch);
        }

        return stacks;
    }

    private static Movement[] ReadMovements(string[] section)
    {
        return section.Select(Movement.Read)
                      .ToArray();
    }

    private record Movement(int From, int To, int Quantity)
    {
        private static readonly Regex Pattern = new(@"^move (?<Quantity>\d+) from (?<From>\d) to (?<To>\d)$",
                                                    RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking);

        public static Movement Read(string input)
        {
            var match = Pattern.Match(input);
            if (!match.Success) throw new FormatException($"Not a movement: {input}");
            return new Movement(Convert.ToInt32(match.Groups["From"].Value) - 1,
                                Convert.ToInt32(match.Groups["To"].Value) - 1,
                                Convert.ToInt32(match.Groups["Quantity"].Value));
        }
    }
}
