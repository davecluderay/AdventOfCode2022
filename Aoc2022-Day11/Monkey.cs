using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Aoc2022_Day11; 

internal class Monkey
{
    private readonly Func<long, long> _inspectOperation;
    private readonly int _nextMonkeyIfTestTrue;
    private readonly int _nextMonkeyIfTestFalse;

    public int Id { get; }
    public int TestDivisor { get; }
    public int NumberOfItemsInspected { get; private set; }
    public Queue<long> Items { get; } = new();

    private Monkey(int id,
                   IEnumerable<long> startingItems,
                   Func<long, long> inspectOperation,
                   int testDivisor,
                   int nextMonkeyIfTestTrue,
                   int nextMonkeyIfTestFalse)
    {
        _inspectOperation = inspectOperation;
        _nextMonkeyIfTestTrue = nextMonkeyIfTestTrue;
        _nextMonkeyIfTestFalse = nextMonkeyIfTestFalse;
        Id = id;
        TestDivisor = testDivisor;
        foreach (var item in startingItems)
            Items.Enqueue(item);
    }

    public long InspectItem(long worryLevel)
    {
        NumberOfItemsInspected++;
        return _inspectOperation(worryLevel);
    }

    public int SelectNextMonkey(long worryLevel)
    {
        var testResult = worryLevel % TestDivisor == 0;
        return testResult ? _nextMonkeyIfTestTrue : _nextMonkeyIfTestFalse;
    }

    public static Monkey Read(string input)
    {
        var match = MonkeyInput.Pattern().Match(input);
        if (!match.Success) throw new FormatException($"Not a monkey: {input}");

        var id = Convert.ToInt32(match.Groups["Id"].Value);
        var items = match.Groups["Item"].Captures.Select(c => Convert.ToInt64(c.Value));
        var inspectOperator = match.Groups["InspectOperator"].Value.Single();
        var inspectOperand = match.Groups["InspectOperand"].Value;
        var testDivisor = Convert.ToInt32(match.Groups["TestDivisor"].Value);
        var nextMonkeyIfTestTrue = Convert.ToInt32(match.Groups["NextMonkeyIfTestTrue"].Value);
        var nextMonkeyIfTestFalse = Convert.ToInt32(match.Groups["NextMonkeyIfTestFalse"].Value);

        Func<long, long> inspectOperation =
            (inspectOperator, inspectOperand) switch
            {
                ('*', "old") => n => n * n,
                ('+', "old") => n => n + n,
                ('*', _)     => n => n * Convert.ToInt32(inspectOperand),
                ('+', _)     => n => n + Convert.ToInt32(inspectOperand),
                _            => throw new UnreachableException("Unsupported operator.")
            };

        return new Monkey(id, items, inspectOperation, testDivisor, nextMonkeyIfTestTrue, nextMonkeyIfTestFalse);
    }
}

internal static partial class MonkeyInput
{
    [GeneratedRegex(
        """
        Monkey (?<Id>\d+):
          Starting items: ((, )?(?<Item>\d+))+
          Operation: new = old (?<InspectOperator>[+*]) (?<InspectOperand>\d+|old)
          Test: divisible by (?<TestDivisor>\d+)
            If true: throw to monkey (?<NextMonkeyIfTestTrue>\d+)
            If false: throw to monkey (?<NextMonkeyIfTestFalse>\d+)
        """,
        RegexOptions.ExplicitCapture | RegexOptions.Compiled
    )]
    public static partial Regex Pattern();
}
