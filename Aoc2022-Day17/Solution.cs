using System.Diagnostics;

namespace Aoc2022_Day17;

internal class Solution
{
    public string Title => "Day 17: Pyroclastic Flow";

    public object PartOne()
    {
        return CalculateHeightAfterRockFalls(2022L);
    }

    public object PartTwo()
    {
        return CalculateHeightAfterRockFalls(1_000_000_000_000L);
    }

    private static long CalculateHeightAfterRockFalls(long stopAtFallCount)
    {
        var rockLayouts = new[]
                          {
                              new[] { "####" },
                              new[] { ".#.", "###", ".#." },
                              new[] { "..#", "..#", "###" },
                              new[] { "#", "#", "#", "#" },
                              new[] { "##", "##" }
                          };
        var jets = InputFile.ReadAllText()
                            .Trim();

        var chamber = new Chamber();
        var heightPredictor = new HeightPredictor(stopAtFallCount);

        var totalRocksFallen = 0L;
        var rockIndex = 0;
        var jetIndex = -1;
        var rockPosition = chamber.SelectStartingPositionFor(rockLayouts[rockIndex]);

        while (true)
        {
            jetIndex = (jetIndex + 1) % jets.Length;
            var jet = jets[jetIndex];
            var rock = rockLayouts[rockIndex];

            // Push the rock left or right.
            var pushedPosition = jet switch
                                 {
                                     '<' => rockPosition.Left(),
                                     '>' => rockPosition.Right(),
                                     _   => throw new UnreachableException($"Not a jet direction: {jet}")
                                 };
            if (chamber.CanOccupyPosition(rock, pushedPosition))
            {
                rockPosition = pushedPosition;
            }

            // Let the rock fall one position.
            var downPosition = rockPosition.Down();
            if (chamber.CanOccupyPosition(rock, downPosition))
            {
                rockPosition = downPosition;
            }
            else
            {
                // The rock can't fall any further.
                chamber.FixPosition(rock, rockPosition);
                heightPredictor.Record(rockIndex, jetIndex, rockPosition);
                if (heightPredictor.PredictedY is not null)
                    return heightPredictor.PredictedY.Value;

                rockIndex = (rockIndex + 1) % rockLayouts.Length;
                rock = rockLayouts[rockIndex];
                rockPosition = chamber.SelectStartingPositionFor(rock);
                totalRocksFallen++;
            }

            if (totalRocksFallen == stopAtFallCount) return chamber.Height;
        }
    }
}
