namespace Aoc2022_Day23;

internal class Solution
{
    public string Title => "Day 23: Unstable Diffusion";

    public object PartOne()
    {
        var simulation = Simulation.Read();
        for (var round = 1; round <= 10; round++)
        {
            simulation.SimulateRound(round);
        }
        return simulation.CountEmptyTiles();
    }

    public object PartTwo()
    {
        var simulation = Simulation.Read();
        var round = 0;
        while (true)
        {
            var outcome = simulation.SimulateRound(++round);
            if (outcome.NumberOfMovements == 0)
            {
                break;
            }
        }
        return round;
    }
}
