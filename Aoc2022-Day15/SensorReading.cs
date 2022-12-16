using System.Text.RegularExpressions;

namespace Aoc2022_Day15;

internal partial record SensorReading(Position At, Position ClosestBeaconAt)
{
    public int BeaconDistance => Math.Abs(At.X - ClosestBeaconAt.X) + Math.Abs(At.Y - ClosestBeaconAt.Y);

    public static SensorReading Read(string input)
    {
        var match = Pattern().Match(input);
        if (!match.Success) throw new FormatException($"Not a sensor: {input}");
        return new SensorReading(new Position(Convert.ToInt32(match.Groups["SX"].Value),
                                              Convert.ToInt32(match.Groups["SY"].Value)),
                                 new Position(Convert.ToInt32(match.Groups["BX"].Value),
                                              Convert.ToInt32(match.Groups["BY"].Value)));
    }

    [GeneratedRegex(@"^Sensor at x=(?<SX>-?\d+), y=(?<SY>-?\d+): closest beacon is at x=(?<BX>-?\d+), y=(?<BY>-?\d+)",
                    RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking)]
    private static partial Regex Pattern();
}
