namespace Aoc2022_Day23;

internal class Simulation
{
    private readonly HashSet<Position> _map;

    private Simulation(HashSet<Position> map)
    {
        _map = map;
    }

    public RoundOutcome SimulateRound(int round)
    {
        var moves = _map.Select(p => ProposeNewPosition(p, round))
                        .Where(p => p.From != p.To)
                        .GroupBy(p => p.To)
                        .Where(g => g.Count() == 1)
                        .SelectMany(g => g);
        var numberOfMovements = 0;
        foreach (var move in moves)
        {
            _map.Remove(move.From);
            _map.Add(move.To);
            numberOfMovements++;
        }
        return new RoundOutcome(numberOfMovements);
    }

    private (Position From, Position To) ProposeNewPosition(Position from, int round)
    {
        var (x, y) = from;
        var (nw, n, ne, w, e, sw, s, se) = (_map.Contains((x - 1, y - 1)),
                                            _map.Contains((x, y - 1)),
                                            _map.Contains((x + 1, y - 1)),
                                            _map.Contains((x - 1, y)),
                                            _map.Contains((x + 1, y)),
                                            _map.Contains((x - 1, y + 1)),
                                            _map.Contains((x, y + 1)),
                                            _map.Contains((x + 1, y + 1)));
        if (nw || n || ne || w || e || sw || s || se)
        {
            var directions = new[] { "N", "S", "W", "E" };
            for (var i = 0; i < 4; i++)
            {
                var direction = directions[(round - 1 + i) % 4];
                switch (direction)
                {
                    case "N" when !nw && !n && !ne:
                        return (from, (x, y - 1));
                    case "S" when !sw && !s && !se:
                        return (from, (x, y + 1));
                    case "W" when !nw && !w && !sw:
                        return (from, (x - 1, y));
                    case "E" when !ne && !e && !se:
                        return (from, (x + 1, y));
                }
            }
        }
        return (from, from);
    }

    public int CountEmptyTiles()
    {
        var (minY, maxY, minX, maxX) = (int.MaxValue, 0, int.MaxValue, 0);
        foreach (var position in _map)
        {
            minX = Math.Min(minX, position.X);
            maxX = Math.Max(maxX, position.X);
            minY = Math.Min(minY, position.Y);
            maxY = Math.Max(maxY, position.Y);
        }
        var tileCount = (maxX - minX + 1) * (maxY - minY + 1);
        return tileCount - _map.Count;
    }

    public static Simulation Read(string? fileName = null)
    {
        var data = new HashSet<Position>();
        var lines = InputFile.ReadAllLines(fileName);
        for (var y = 0; y < lines.Length; y++)
        for (var x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x] == '#')
                data.Add((x, y));
        }
        return new Simulation(data);
    }
}

internal record RoundOutcome(int NumberOfMovements);
