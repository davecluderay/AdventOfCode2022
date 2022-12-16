using System.Diagnostics;

namespace Aoc2022_Day15;

internal class Solution
{
    public string Title => "Day 15: Beacon Exclusion Zone";

    public object PartOne()
    {
        var sensors = InputFile.ReadAllLines()
                               .Select(SensorReading.Read)
                               .ToArray();
        var beaconPositions = sensors.Select(s => s.ClosestBeaconAt).Distinct();
        var sensorPositions = sensors.Select(s => s.At);

        const int y = 2_000_000;
        var (_, combinedRanges) = AnalyzeExcludedRangesAlongLine(y, sensors);
        var result = combinedRanges.Sum(r => r.Length)
                     - beaconPositions.Count(b => b.Y == y && combinedRanges.Any(r => r.Contains(b.X)))
                     - sensorPositions.Count(s => s.Y == y && combinedRanges.Any(r => r.Contains(s.X)));
        return result;
    }

    public object PartTwo()
    {
        var sensors = InputFile.ReadAllLines()
                               .Select(SensorReading.Read)
                               .ToArray();

        var clampTo = new Range(0, 4_000_000);

        // Start from the centre and work up/down. 
        var analyseQueue = new Queue<int>(new[] { clampTo.End / 2, clampTo.End / 2 - 1 });
        while (analyseQueue.Count > 0)
        {
            var lineY = analyseQueue.Dequeue();
            var analysis = AnalyzeExcludedRangesAlongLine(lineY, sensors, clampTo);
            if (analysis.CombinedRanges.Count == 2)
            {
                var x = analysis.CombinedRanges.Max(r => r.Start) - 1;
                var tuningFrequency = x * 4_000_000L + lineY;
                return tuningFrequency;
            }

            // Based on range overlaps, we can skip some lines
            // (i.e. if we know all ranges will still overlap on those lines).
            var skipSign = lineY >= clampTo.End / 2 ? 1 : -1;
            var nextY = lineY + skipSign * Math.Max(1, analysis.ShortestRangeOverlap / 2);
            if (clampTo.Contains(nextY))
                analyseQueue.Enqueue(nextY);
        }

        throw new UnreachableException("Did not find an answer.");
    }

    private static LineAnalysisResult AnalyzeExcludedRangesAlongLine(int y, SensorReading[] sensors, Range? clampTo = null)
    {
        var ranges = GetRawExcludedRangesAlongLine(y, sensors);
        ranges = ClampRanges(ranges, clampTo);
        ranges = EliminateFullyContainedRanges(ranges);
        var shortestRangeOverlap = FindShortestRangeOverlap(ranges);
        var combinedRanges = Range.CombineAll(ranges)
                                  .ToList()
                                  .AsReadOnly();
        return new LineAnalysisResult(shortestRangeOverlap, combinedRanges);
    }

    private static IReadOnlyCollection<Range> GetRawExcludedRangesAlongLine(int y, IEnumerable<SensorReading> sensors)
    {
        var results = new List<Range>();
        foreach (var sensor in sensors)
        {
            var lineDistance = Math.Abs(y - sensor.At.Y);
            if (sensor.BeaconDistance < lineDistance) continue;
            results.Add((sensor.At.X - (sensor.BeaconDistance - lineDistance),
                         sensor.At.X + (sensor.BeaconDistance - lineDistance)));
        }
        return results;
    }

    private static IReadOnlyCollection<Range> ClampRanges(IReadOnlyCollection<Range> ranges, Range? clampTo)
    {
        if (!clampTo.HasValue) return ranges;

        var result = ranges.ToList();
        for (var i = result.Count - 1; i >= 0; i--)
        {
            if (clampTo.Value.Overlaps(result[i]))
            {
                result[i] = new Range(Math.Max(result[i].Start, clampTo.Value.Start),
                                      Math.Min(result[i].End, clampTo.Value.End));
            }
            else
            {
                result.RemoveAt(i);
            }
        }

        return result;
    }

    private static IReadOnlyCollection<Range> EliminateFullyContainedRanges(IEnumerable<Range> ranges)
    {
        var results = ranges.ToList();
        for (var i = 0; i < results.Count; i++)
        for (var j = 0; j < results.Count; j++)
        {
            if (i == j) continue;
            if (!results[j].Contains(results[i])) continue;
            results.RemoveAt(i--);
            break;
        }
        return results;
    }

    private static int FindShortestRangeOverlap(IEnumerable<Range> ranges)
    {
        int? shortestRangeOverlap = null;
        var sorted = ranges.OrderBy(r => (r.Start, r.End)).ToList();
        for (var i = 0; i < sorted.Count - 1; i++)
        {
            var overlap = sorted[i].OverlapLength(sorted[i + 1]);
            shortestRangeOverlap = Math.Min(overlap, shortestRangeOverlap ?? int.MaxValue);
        }
        return shortestRangeOverlap ?? 0;
    }

    private record LineAnalysisResult(int ShortestRangeOverlap, IReadOnlyCollection<Range> CombinedRanges);
}
