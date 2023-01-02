namespace Aoc2022_Day24;

internal class Basin
{
    private Basin(Position start, Position end, int width, int height, Blizzards blizzards)
    {
        _blizzards = blizzards;
        _width = width;
        _height = height;
        Start = start;
        End = end;
    }

    private readonly Blizzards _blizzards;
    private readonly int _width;
    private readonly int _height;

    public Position Start { get; }
    public Position End { get; }

    public int FindShortestPath(Position from, Position to, int fromMinute)
    {
        var repeatFrequency = Calculate.LeastCommonMultiple(_width - 2, _height - 2);
        var repeats = new HashSet<(Position Position, int Minute)>();

        var queue = new Queue<(Position Position, int Minute)>();
        queue.Enqueue((from, fromMinute));
        var shortest = int.MaxValue;
        while (queue.Count > 0)
        {
            var entry = queue.Dequeue();

            if (entry.Minute >= shortest) continue;
            if (repeats.Contains((entry.Position, entry.Minute % repeatFrequency))) continue;

            repeats.Add((entry.Position, entry.Minute % repeatFrequency));
            if (entry.Position == to)
            {
                shortest = Math.Min(shortest, entry.Minute);
                continue;
            }

            var possibles = GetPossibleNextPositions(entry.Position, to, entry.Minute + 1);
            foreach (var possible in possibles)
            {
                queue.Enqueue((possible, entry.Minute + 1));
            }
        }

        return shortest;
    }

    private IEnumerable<Position> GetPossibleNextPositions(Position position, Position target, int minute)
    {
        var (x, y) = position;
        var list = new List<Position>(5);
        if (CanOccupyPosition((x + 1, y), minute))
            list.Add((x + 1, y));
        if (CanOccupyPosition((x, y + 1), minute))
            list.Add((x, y + 1));
        if (CanOccupyPosition(position, minute))
            list.Add(position);
        if (CanOccupyPosition((x, y - 1), minute))
            list.Add((x, y - 1));
        if (CanOccupyPosition((x - 1, y), minute))
            list.Add((x - 1, y));
        return list.OrderBy(p => Math.Abs(p.X - target.X) + Math.Abs(p.Y - target.Y))
                   .ToArray();
    }

    private bool CanOccupyPosition(Position position, int minute)
    {
        if (position == Start) return true;
        if (position == End) return true;
        if (position.X < 1) return false;
        if (position.X > _width - 2) return false;
        if (position.Y < 1) return false;
        if (position.Y > _height - 2) return false;
        return !HasBlizzardAt(position, minute);
    }

    private bool HasBlizzardAt(Position position, int minute)
    {
        var (x, y) = position;
        var basinWidth = _width - 2;
        var basinHeight = _height - 2;

        var rightBlizzardStartingAt = ((x - 1 - minute + (minute / basinWidth + 1) * basinWidth) % basinWidth + 1, y);
        if (_blizzards.Right.Contains(rightBlizzardStartingAt))
            return true;

        var downBlizzardStartingAt = (x, (y - 1 - minute + (minute / basinHeight + 1) * basinHeight) % basinHeight + 1);
        if (_blizzards.Down.Contains(downBlizzardStartingAt))
            return true;

        var leftBlizzardStartingAt = ((x - 1 + minute) % basinWidth + 1, y);
        if (_blizzards.Left.Contains(leftBlizzardStartingAt))
            return true;

        var upBlizzardStartingAt = (x, (y - 1 + minute) % basinHeight + 1);
        if (_blizzards.Up.Contains(upBlizzardStartingAt))
            return true;

        return false;
    }

    public static Basin Read(string? fileName = null)
    {
        var lines = InputFile.ReadAllLines(fileName);
        var (width, height) = (lines[0].Length, lines.Length);
        var startPosition = new Position(lines[0].IndexOf('.'), 0);
        var endPosition = new Position(lines[^1].IndexOf('.'), lines.Length - 1);

        var blizzards = new Blizzards();
        for (var y = 1; y < lines.Length - 1; y++)
        for (var x = 1; x < lines[y].Length - 1; x++)
        {
            var addTo = lines[y][x] switch
                        {
                            '<' => blizzards.Left,
                            '>' => blizzards.Right,
                            '^' => blizzards.Up,
                            'v' => blizzards.Down,
                            _   => null
                        };
            addTo?.Add((x, y));
        }

        return new Basin(startPosition, endPosition, width, height, blizzards);
    }

    private record Blizzards
    {
        public HashSet<Position> Left { get; } = new();
        public HashSet<Position> Right { get; } = new();
        public HashSet<Position> Up { get; } = new();
        public HashSet<Position> Down { get; } = new();
    }
}
