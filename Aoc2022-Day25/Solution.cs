namespace Aoc2022_Day25;

internal class Solution
{
    public string Title => "Day 25: Full of Hot Air";

    public object PartOne()
    {
        return Snafu.FromDecimal(InputFile.ReadAllLines()
                                          .Select(Snafu.ToDecimal)
                                          .Sum());
    }

    public object PartTwo()
    {
        return "MERRY CHRISTMAS!";
    }
}
