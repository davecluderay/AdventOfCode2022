namespace Aoc2022_Day18;

internal class Solution
{
    public string Title => "Day 18: Boiling Boulders";

    public object PartOne()
    {
        var cubes = InputFile.ReadAllLines()
                             .Select(Position.Read)
                             .ToHashSet();
        return cubes.Sum(c => c.GetAdjacentPositions()
                               .Count(a => !cubes.Contains(a)));
    }

    public object PartTwo()
    {
        var cubes = InputFile.ReadAllLines()
                             .Select(Position.Read)
                             .ToHashSet();
        var bounds = Bounds.Calculate(cubes);

        var outside = new HashSet<Position>(bounds.GetAllSurfacePositions()
                                                  .Where(p => !cubes.Contains(p)));
        var queue = new Queue<Position>(outside);
        while (queue.Any())
        {
            var p = queue.Dequeue();
            foreach (var adjacent in p.GetAdjacentPositions().Where(a => !outside.Contains(a) && !cubes.Contains(a) && bounds.Contains(a)))
            {
                outside.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }

        return cubes.Sum(c => c.GetAdjacentPositions().Count(a => outside.Contains(a) || !bounds.Contains(a)));
    }
}
