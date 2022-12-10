using System.Diagnostics;

namespace Aoc2022_Day09;

internal class Solution
{
    public string Title => "Day 9: Rope Bridge";

    public object PartOne()
        => SimulateRope(ReadSteps(), 2);

    public object PartTwo()
        => SimulateRope(ReadSteps(), 10);

    private static int SimulateRope(Vector[] steps, int knotCount)
    {
        var knots = Enumerable.Repeat(new Position(0, 0), knotCount).ToArray();
        var visited = new HashSet<Position> { (0, 0) };
        foreach (var step in steps)
        {
            MoveKnots(knots, step);
            visited.Add(knots.Last());
        }
        return visited.Count;
    }

    private static void MoveKnots(Position[] knots, Vector step)
    {
        knots[0] = knots[0].Move(step);
        for (var i = 1; i < knots.Length; i++)
        {
            knots[i] = MoveKnot(knots[i], towards: knots[i - 1]);
        }
    }

    private static Position MoveKnot(Position knot, Position towards)
    {
        var delta = towards.Delta(knot);
        if (Math.Abs(delta.X) > 1 || Math.Abs(delta.Y) > 1)
            return knot.Move((Math.Sign(delta.X), Math.Sign(delta.Y)));
        return knot;
    }

    private static Vector[] ReadSteps(string? fileName = null)
    {
        var results = from line in InputFile.ReadAllLines(fileName)
                      let split = line.Split(' ', 2)
                      let direction = split[0].Single()
                      let steps = Convert.ToInt32(split[1])
                      select direction switch
                             {
                                 'L' => Enumerable.Repeat(new Vector(-1, 0), steps),
                                 'R' => Enumerable.Repeat(new Vector(1, 0), steps),
                                 'U' => Enumerable.Repeat(new Vector(0, -1), steps),
                                 'D' => Enumerable.Repeat(new Vector(0, 1), steps),
                                 _   => throw new UnreachableException($"Not a movement: {line}")
                             };
        return results.SelectMany(p => p).ToArray();
    }
}
