namespace Aoc2022_Day08;

internal class Solution
{
    public string Title => "Day 8: Treetop Tree House";

    public object PartOne()
    {
        var grid = Grid.Read();
        var visible = new HashSet<Position>();

        var left = grid.LeftEdgePositions()
                       .SelectMany(p => FindVisibleTrees(grid, grid.PositionsRightFrom(p)));
        var right = grid.RightEdgePositions()
                        .SelectMany(p => FindVisibleTrees(grid, grid.PositionsLeftFrom(p)));
        var top = grid.TopEdgePositions()
                      .SelectMany(p => FindVisibleTrees(grid, grid.PositionsDownFrom(p)));
        var bottom = grid.BottomEdgePositions()
                         .SelectMany(p => FindVisibleTrees(grid, grid.PositionsUpFrom(p)));

        foreach (var position in left.Concat(right).Concat(top).Concat(bottom))
            visible.Add(position);

        return visible.Count;
    }

    public object PartTwo()
    {
        var grid = Grid.Read();
        var highestScore = 0;

        for (var y = 1; y < grid.Bounds.Height - 2; y++)
        for (var x = 1; x < grid.Bounds.Width - 2; x++)
        {
            var right = GetViewingDistance(grid, grid.PositionsRightFrom((x, y)));
            var left = GetViewingDistance(grid, grid.PositionsLeftFrom((x, y)));
            var down = GetViewingDistance(grid, grid.PositionsDownFrom((x, y)));
            var up = GetViewingDistance(grid, grid.PositionsUpFrom((x, y)));
            var scenicScore = left * right * up * down;
            highestScore = Math.Max(scenicScore, highestScore);
        }

        return highestScore;
    }

    private static IEnumerable<Position> FindVisibleTrees(Grid grid, IEnumerable<Position> lineOfSight)
    {
        var tallest = -1;
        foreach (var position in lineOfSight)
        {
            var height = grid.HeightOfTreeAt(position);
            if (height <= tallest) continue;

            tallest = height;
            yield return position;

            if (height == 9) break;
        }
    }

    private static int GetViewingDistance(Grid grid, IEnumerable<Position> lineOfSight)
    {
        using var enumerator = lineOfSight.GetEnumerator();
        enumerator.MoveNext();
        var height = grid.HeightOfTreeAt(enumerator.Current);

        var i = 1;
        while (enumerator.MoveNext())
        {
            if (grid.HeightOfTreeAt(enumerator.Current) >= height)
                return i;
            i++;
        }

        return i - 1;
    }
}
