namespace Aoc2022_Day11;

internal class Solution
{
    public string Title => "Day 11: Monkey in the Middle";

    public object PartOne()
    {
        var monkeys = ReadMonkeys();
        SimulateRounds(monkeys, 20, decreaseWorryLevelAfterInspection: true);
        var result = CalculateMonkeyBusiness(monkeys);
        return result;
    }

    public object PartTwo()
    {
        var monkeys = ReadMonkeys();
        SimulateRounds(monkeys, 10_000, decreaseWorryLevelAfterInspection: false);
        var result = CalculateMonkeyBusiness(monkeys);
        return result;
    }

    private static IReadOnlyDictionary<int, Monkey> ReadMonkeys(string? fileName = null)
        => InputFile.ReadInSections(fileName)
                    .Select(s => string.Join('\n', s))
                    .Select(Monkey.Read)
                    .ToDictionary(m => m.Id)
                    .AsReadOnly();

    private static void SimulateRounds(IReadOnlyDictionary<int, Monkey> monkeys, int numberOfRounds, bool decreaseWorryLevelAfterInspection)
    {
        for (var i = 0; i < numberOfRounds; i++)
            SimulateRound(monkeys, decreaseWorryLevelAfterInspection);
    }

    private static void SimulateRound(IReadOnlyDictionary<int, Monkey> monkeys, bool decreaseWorryLevelAfterInspection)
    {
        var worryLimiter = monkeys.Aggregate(1L, (a, v) => a * v.Value.TestDivisor);
        foreach (var id in monkeys.Keys)
        {
            var monkey = monkeys[id];
            while (monkey.Items.Any())
            {
                var oldWorryLevel = monkey.Items.Dequeue();

                var newWorryLevel = monkey.InspectItem(oldWorryLevel);
                if (decreaseWorryLevelAfterInspection)
                {
                    newWorryLevel /= 3;
                }
                newWorryLevel %= worryLimiter;

                var nextMonkey = monkey.SelectNextMonkey(newWorryLevel);
                monkeys[nextMonkey].Items.Enqueue(newWorryLevel);
            }
        }
    }

    private static long CalculateMonkeyBusiness(IReadOnlyDictionary<int, Monkey> monkeys)
        => monkeys.Select(m => (long)m.Value.NumberOfItemsInspected)
                  .OrderDescending()
                  .Take(2)
                  .Aggregate(1L, (prev, next) => prev * next);
}
