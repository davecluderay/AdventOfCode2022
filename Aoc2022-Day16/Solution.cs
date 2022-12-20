using System.Collections.Immutable;

namespace Aoc2022_Day16;

internal class Solution
{
    public string Title => "Day 16: Proboscidea Volcanium";

    public object PartOne()
    {
        var map = ValveMap.Read();
        var possibleRoutes = FindAllPossibleRoutes(map,
                                                   ValveMap.StartFromValveId,
                                                   stopAfterMinutes: 30,
                                                   visited: ImmutableHashSet.Create<string>());
        return possibleRoutes.Max(route => route.ReleasesTotalPressure);
    }

    public object PartTwo()
    {
        var map = ValveMap.Read();
        var possibleRoutes = FindAllPossibleRoutes(map,
                                                   ValveMap.StartFromValveId,
                                                   stopAfterMinutes: 26,
                                                   visited: ImmutableHashSet.Create<string>());

        long CalculateMask(Route route)
            => route.Stops.Skip(1).Aggregate(0L, (a, v) => a | (1L << map.MaskBitFor(v.ValveId)));

        var routesWithMasks = possibleRoutes.Select(r => (Route: r, Mask: CalculateMask(r)))
                                            .ToList();

        var maximumPressureReleased = 0;

        for (var i = 0; i < routesWithMasks.Count; i++)
        for (var j = i + 1; j < routesWithMasks.Count; j++)
        {
            var (route1, mask1) = routesWithMasks[i];
            var (route2, mask2) = routesWithMasks[j];
            if ((mask1 & mask2) != 0L) continue;

            var totalPressureReleased = route1.ReleasesTotalPressure + route2.ReleasesTotalPressure;
            maximumPressureReleased = Math.Max(maximumPressureReleased, totalPressureReleased);
        }

        return maximumPressureReleased;
    }

    private static IEnumerable<Route> FindAllPossibleRoutes(ValveMap map, string from, int stopAfterMinutes, ImmutableHashSet<string> visited)
    {
        var fromStep = new RouteStop(ValveId: from, ReleasesTotalPressure: stopAfterMinutes * map.FlowRateOfValve(from));

        foreach (var connection in map.ConnectionsFrom(from))
        {
            var valveId = connection.ValveId;
            var minutesToReachAndOpenValve = connection.MinutesAway + 1;
            if (visited.Contains(connection.ValveId)) continue;
            if (minutesToReachAndOpenValve >= stopAfterMinutes) continue;

            foreach (var onward in FindAllPossibleRoutes(map, valveId, stopAfterMinutes - minutesToReachAndOpenValve, visited.Add(from)))
            {
                yield return new Route(Stops: onward.Stops.Prepend(fromStep));
            }
        }

        yield return new Route(fromStep);
    }

    private readonly record struct Route(IEnumerable<RouteStop> Stops)
    {
        public Route(RouteStop stop) : this(new[] { stop }) {}
        public int ReleasesTotalPressure { get; } = Stops.Sum(s => s.ReleasesTotalPressure);
    }

    private readonly record struct RouteStop(string ValveId, int ReleasesTotalPressure);
}
