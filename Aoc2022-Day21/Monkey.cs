using System.Text.RegularExpressions;

namespace Aoc2022_Day21;

internal partial record Monkey(string Name, ImmediateYell? ImmediateYell, DeferredYell? DeferredYell)
{
    public static Monkey Read(string input)
    {
        var match = Pattern().Match(input);
        if (!match.Success) throw new FormatException($"Not a monkey: {input}");

        var name = match.Groups["Name"].Value;
        if (match.Groups["Number"].Success)
        {
            return new Monkey(name, new ImmediateYell(int.Parse(match.Groups["Number"].Value)), null);
        }

        return new Monkey(name, null, new DeferredYell(match.Groups["LeftMonkeyName"].Value,
                                                       match.Groups["RightMonkeyName"].Value,
                                                       match.Groups["Operator"].Value.Single()));
    }

    [GeneratedRegex(@"^(?<Name>\w+):\s*((?<LeftMonkeyName>\w+)\s*(?<Operator>[-+*/])\s*(?<RightMonkeyName>\w+)|(?<Number>-?\d+))$", RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking)]
    private static partial Regex Pattern();
}

internal record ImmediateYell(int Number);

internal record DeferredYell(string LeftMonkeyName, string RightMonkeyName, char Operator);
