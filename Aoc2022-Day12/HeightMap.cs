namespace Aoc2022_Day12;

internal class HeightMap
{
    private readonly int _width;
    private readonly int _height;
    private readonly Dictionary<Position, int> _heights = new();

    public Position StartPosition { get; } = (0, 0);
    public Position GoalPosition { get; } = (0, 0);

    private HeightMap(string[] lines)
    {
        (_width, _height) = (lines[0].Length, lines.Length);
        
        for (var y = 0; y < lines.Length; y++)
        for (var x = 0; x < lines[y].Length; x++)
        {
            var position = new Position(x, y);
            var @char = lines[y][x];
            switch (@char)
            {
                case 'S':
                    StartPosition = position;
                    _heights[position] = 0;
                    break;
                case 'E':
                    GoalPosition = position;
                    _heights[position] = 25;
                    break;
                default:
                    _heights[position] = @char - 'a';
                    break;
            }
        }
    }

    public IEnumerable<Position> AllPositions()
    {
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
        {
            yield return (x, y);
        }
    }

    public IEnumerable<Position> AllNeighbouringPositions(Position to)
    {
        var (x, y) = to;
        if (y > 0) yield return (x, y - 1);
        if (x < _width - 1) yield return (x + 1, y);
        if (y < _height - 1) yield return (x, y + 1);
        if (x > 0) yield return (x - 1, y);
    }

    public int HeightAt(Position position)
        => _heights[position];

    public static HeightMap Read(string? fileName = null)
        => new(InputFile.ReadAllLines(fileName));
}
