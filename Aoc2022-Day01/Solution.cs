namespace Aoc2022_Day01;

internal class Solution
{
    public string Title => "Day 1: Calorie Counting";

    public object? PartOne()
    {
        var result = InputFile.ReadInSections()
                              .Max(s => s.Sum(Convert.ToInt32));
        return result;
    }
        
    public object? PartTwo()
    {
        var result = InputFile.ReadInSections()
                              .Select(s => s.Sum(Convert.ToInt32))
                              .OrderDescending()
                              .Take(3)
                              .Sum();
        return result;
    }
}
