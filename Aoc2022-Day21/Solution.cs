using System.Diagnostics;

namespace Aoc2022_Day21;

internal class Solution
{
    const string RootName = "root";
    const string HumanName = "humn";

    public string Title => "Day 21: Monkey Math";

    public object PartOne()
    {
        var monkeys = ReadMonkeys();
        return Yell(RootName, monkeys);
    }

    public object PartTwo()
    {
        var monkeys = ReadMonkeys();
        var path = Follow(RootName, HumanName, monkeys);

        var target = 0L;
        for (var i = 1; i < path.Length; i++)
        {
            var evaluating = monkeys[path[i - 1]];
            var nextName = path[i];
            long? leftYell = evaluating.DeferredYell!.LeftMonkeyName == nextName ? null : Yell(evaluating.DeferredYell.LeftMonkeyName, monkeys);
            long? rightYell = evaluating.DeferredYell!.RightMonkeyName == nextName ? null : Yell(evaluating.DeferredYell.RightMonkeyName, monkeys);
            var @operator = i == 1 ? '=' : evaluating.DeferredYell.Operator;

            target = CalculateMissingOperand(target, leftYell, rightYell, @operator);
        }

        return target;
    }

    private static long Yell(string monkeyName, Dictionary<string, Monkey> monkeys)
    {
        var monkey = monkeys[monkeyName];
        if (monkey.ImmediateYell is not null)
            return monkey.ImmediateYell.Number;

        var yell = monkey.DeferredYell!;
        var left = Yell(yell.LeftMonkeyName, monkeys);
        var right = Yell(yell.RightMonkeyName, monkeys);
        return yell.Operator switch
               {
                   '+' => left + right,
                   '-' => left - right,
                   '*' => left * right,
                   '/' => left / right,
                   _   => throw new UnreachableException($"Unrecognised operator: {yell.Operator}")
               };
    }

    private static string[] Follow(string fromName, string toName, Dictionary<string, Monkey> monkeys)
    {
        if (fromName == toName)
            return new[] { toName };

        var from = monkeys[fromName];
        if (from.DeferredYell is not null)
        {
            var left = Follow(from.DeferredYell.LeftMonkeyName, toName, monkeys);
            if (left.Length > 0)
                return left.Prepend(fromName).ToArray();

            var right = Follow(from.DeferredYell.RightMonkeyName, toName, monkeys);
            if (right.Length > 0)
                return right.Prepend(fromName).ToArray();
        }
        return Array.Empty<string>();
    }

    private static long CalculateMissingOperand(long requiredResult, long? leftOperand, long? rightOperand, char @operator)
    {
        return @operator switch
               {
                   '=' => leftOperand ?? rightOperand!.Value,
                   '+' => requiredResult - (leftOperand ?? rightOperand!.Value),
                   '*' => requiredResult / (leftOperand ?? rightOperand!.Value),
                   '-' => leftOperand.HasValue
                              ? leftOperand.Value - requiredResult
                              : rightOperand!.Value + requiredResult,
                   '/' => leftOperand.HasValue
                              ? leftOperand.Value / requiredResult
                              : rightOperand!.Value * requiredResult,
                   _ => throw new UnreachableException($"Unexpected operator: {@operator}")
               };
    }

    private static Dictionary<string, Monkey> ReadMonkeys(string? fileName = null)
        => InputFile.ReadAllLines(fileName)
                    .Select(Monkey.Read)
                    .ToDictionary(m => m.Name);
}
