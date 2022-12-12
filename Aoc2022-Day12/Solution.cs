using System.Collections.Immutable;

namespace Aoc2022_Day12;

internal class Solution
{
    public string Title => "Day 12: Hill Climbing Algorithm";

    public object PartOne()
    {
        var heightMap = HeightMap.Read();
        var fewestStepsByPosition = heightMap.AllPositions().ToDictionary(p => p, _ => int.MaxValue);

        int FewestSteps(HeightMap map, Position from, Position to, ImmutableHashSet<Position> visited)
        {
            fewestStepsByPosition[from] = visited.Count;
            if (from == to) return 0;

            var options = map.AllNeighbouringPositions(from)
                             .Where(t => !visited.Contains(t))
                             .Where(p => map.HeightAt(p) <= map.HeightAt(from) + 1)
                             .Where(t => fewestStepsByPosition[t] > visited.Count + 1);

            var fewest = int.MaxValue;
            foreach (var next in options)
            {
                fewest = Math.Min(fewest, FewestSteps(map, next, to, visited.Add(from)));
            }

            return fewest == int.MaxValue ? int.MaxValue : fewest + 1;
        }

        var result = FewestSteps(heightMap,
                                 from: heightMap.StartPosition,
                                 to: heightMap.GoalPosition,
                                 visited: Enumerable.Empty<Position>().ToImmutableHashSet());
        return result;
    }
        
    public object PartTwo()
    {
        var heightMap = HeightMap.Read();
        var fewestStepsByPosition = heightMap.AllPositions().ToDictionary(p => p, _ => int.MaxValue);

        int FewestSteps(HeightMap map, Position from, int toElevation, ImmutableHashSet<Position> visited)
        {
            fewestStepsByPosition[from] = visited.Count;
            if (map.HeightAt(from) == toElevation) return 0;

            var options = map.AllNeighbouringPositions(from)
                             .Where(t => !visited.Contains(t))
                             .Where(p => map.HeightAt(from) <= map.HeightAt(p) + 1)
                             .Where(t => fewestStepsByPosition[t] > visited.Count + 1);

            var fewest = int.MaxValue;
            foreach (var next in options)
            {
                fewest = Math.Min(fewest, FewestSteps(map, next, toElevation, visited.Add(from)));
            }

            return fewest == int.MaxValue ? int.MaxValue : fewest + 1;
        }

        var result = FewestSteps(heightMap,
                                 from: heightMap.GoalPosition,
                                 toElevation: 0,
                                 visited: Enumerable.Empty<Position>().ToImmutableHashSet());
        return result;
    }
}
