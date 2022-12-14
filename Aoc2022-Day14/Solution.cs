using System.Diagnostics;

namespace Aoc2022_Day14;

internal class Solution
{
    public string Title => "Day 14: Regolith Reservoir";

    public object PartOne()
    {
        var cave = Cave.Read();
        var count = 0;
        while (cave.PositionNextGrainOfSand(simulateFloor: false) is not null)
            count++;
        return count; // 779
    }

    public object PartTwo()
    {
        var cave = Cave.Read();
        var count = 1;
        while (cave.PositionNextGrainOfSand(simulateFloor: true) != cave.Source)
            count++;
        return count; // 27426
    }
}

internal class Cave
{
    private readonly Dictionary<Position, char> _data;

    public Position Source { get; }
    public Bounds Bounds { get; }

    private Cave(Dictionary<Position, char> data, Position source, Bounds bounds)
    {
        _data = data;
        Source = source;
        Bounds = bounds;
    }

    public Position? PositionNextGrainOfSand(bool simulateFloor)
    {
        var (x, y) = Source;
        while (true)
        {
            if (y >= Bounds.MaxY + 2) return null;
            if (y == Bounds.MaxY + 1 && simulateFloor)
            {
                _data[(x, y)] = 'O';
                return (x, y);
            }

            Position down = (x, y + 1);
            if (!_data.ContainsKey(down))
            {
                (x, y) = down;
                continue;
            }

            Position downLeft = (x - 1, y + 1);
            if (!_data.ContainsKey(downLeft))
            {
                (x, y) = downLeft;
                continue;
            }

            Position downRight = (x + 1, y + 1);
            if (!_data.ContainsKey(downRight))
            {
                (x, y) = downRight;
                continue;
            }

            _data[(x, y)] = 'O';
            return (x, y);
        }
    }

    public static Cave Read(string? fileName = null)
    {
        var source = new Position(500, 0);
        var bounds = new Bounds(source.X, source.X, source.Y, source.Y);
        var data = new Dictionary<Position, char>();
        foreach (var line in InputFile.ReadAllLines())
        {
            var positions = line.Split(" -> ")
                                .Select(Position.Read)
                                .ToArray();

            bounds = bounds.Extend(positions[0]);
            for (var i = 0; i < positions.Length - 1; i++)
            {
                bounds = bounds.Extend(positions[i + 1]);
                foreach (var position in TracePath(positions[i], positions[i + 1]))
                {
                    data[position] = '#';
                }
            }
        }

        return new Cave(data, source, bounds);
    }

    private static IEnumerable<Position> TracePath(Position from, Position to)
    {
        var horizontal = (from.X == to.X, from.Y == to.Y) switch
                         {
                             (_, true) => true,
                             (true, _) => false,
                             _         => throw new UnreachableException($"Line is diagonal: {from.X},{from.Y} -> {to.X},{to.Y}")
                         };

        var distance = Math.Abs(horizontal ? to.X - from.X : to.Y - from.Y);
        var delta = Math.Sign(horizontal ? to.X - from.X : to.Y - from.Y);
        for (var i = 0; i <= distance; i++)
        {
            yield return horizontal
                ? (from.X + delta * i, from.Y)
                : (from.X, from.Y + delta * i);
        }
    }
}

internal readonly record struct Position(int X, int Y)
{
    public static implicit operator Position((int X, int Y) position)
        => new (position.X, position.Y);

    public static Position Read(string input)
    {
        var commaIndex = input.IndexOf(',');
        return new Position(int.Parse(input.AsSpan(0, commaIndex)),
                            int.Parse(input.AsSpan(commaIndex + 1)));
    }
}

internal readonly record struct Bounds(int MinX, int MaxX, int MinY, int MaxY)
{
    public Bounds Extend(Position position)
        => (Math.Min(MinX, position.X),
            Math.Max(MaxX, position.X),
            Math.Min(MinY, position.Y),
            Math.Max(MaxY, position.Y));

    public static implicit operator Bounds((int MinX, int MaxX, int MinY, int MaxY) bounds)
        => new (bounds.MinX, bounds.MaxX, bounds.MinY, bounds.MaxY);
}
