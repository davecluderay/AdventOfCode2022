using System.Text.RegularExpressions;

namespace Aoc2022_Day16;

internal partial record Room(string ValveId, int ValveFlowRate, string[] HasTunnelsLeadingToValveIds)
{
    public static Room Read(string input)
    {
        var match = Pattern().Match(input);
        if (!match.Success) throw new FormatException($"Not a room: {input}");
        return new Room(match.Groups["Valve"].Value,
                        Convert.ToInt32(match.Groups["FlowRate"].Value),
                        match.Groups["ToValve"].Captures.Select(c => c.Value).ToArray());
    }

    [GeneratedRegex(@"^Valve (?<Valve>[A-Z]{2}) has flow rate=(?<FlowRate>\d+); tunnels? leads? to valves? ((?<ToValve>[A-Z]{2})(,\s*)?)+$",
                    RegexOptions.ExplicitCapture)]
    private static partial Regex Pattern();
}
