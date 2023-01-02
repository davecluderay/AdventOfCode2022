namespace Aoc2022_Day24;

internal class Solution
{
    public string Title => "Day 24: Blizzard Basin";

    public object PartOne()
    {
        var basin = Basin.Read();
        return basin.FindShortestPath(from: basin.Start,
                                      to: basin.End,
                                      fromMinute: 0);
    }

    public object PartTwo()
    {
        var basin = Basin.Read();
        var minutes = basin.FindShortestPath(from: basin.Start,
                                             to: basin.End,
                                             fromMinute: 0);
        minutes = basin.FindShortestPath(from: basin.End,
                                         to: basin.Start,
                                         fromMinute: minutes);
        minutes = basin.FindShortestPath(from: basin.Start,
                                         to: basin.End,
                                         fromMinute: minutes);
        return minutes;
    }
}
