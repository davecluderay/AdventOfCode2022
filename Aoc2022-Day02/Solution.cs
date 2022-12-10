using System.Diagnostics;

namespace Aoc2022_Day02;

internal class Solution
{
    public string Title => "Day 2: Rock Paper Scissors";

    public object? PartOne()
    {
        var result = InputFile.ReadAllLines()
                              .Select(l => l.Split(" "))
                              .Sum(a => (a[0], a[1]) switch
                                           {
                                               ("A", "X") => 1 + 3,
                                               ("A", "Y") => 2 + 6,
                                               ("A", "Z") => 3 + 0,
                                               ("B", "X") => 1 + 0,
                                               ("B", "Y") => 2 + 3,
                                               ("B", "Z") => 3 + 6,
                                               ("C", "X") => 1 + 6,
                                               ("C", "Y") => 2 + 0,
                                               ("C", "Z") => 3 + 3,
                                               _ => throw new UnreachableException($"Unknown move combination: {a[0]} {a[1]}")
                                           });
        return result;
    }
        
    public object? PartTwo()
    {
        var result = InputFile.ReadAllLines()
                              .Select(l => l.Split(" "))
                              .Sum(a => (a[0], a[1]) switch
                                        {
                                            ("A", "X") => 3 + 0,
                                            ("A", "Y") => 1 + 3,
                                            ("A", "Z") => 2 + 6,
                                            ("B", "X") => 1 + 0,
                                            ("B", "Y") => 2 + 3,
                                            ("B", "Z") => 3 + 6,
                                            ("C", "X") => 2 + 0,
                                            ("C", "Y") => 3 + 3,
                                            ("C", "Z") => 1 + 6,
                                            _ => throw new UnreachableException($"Unknown move combination: {a[0]} {a[1]}")
                                        });
        return result;
    }
}
