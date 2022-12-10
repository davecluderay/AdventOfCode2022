namespace Aoc2022_Day08;

internal class Grid
{
    private static readonly Dictionary<Position, int> Data = new();
    public Bounds Bounds { get; }

    private Grid(string[] data)
    {
        Bounds = (data.Length, data.First().Length);
        for (var y = 0; y < Bounds.Height; y++)
        for (var x = 0; x < Bounds.Width; x++)
        {
            Data[(x, y)] = data[y][x] - '0';
        }
    }

    public int HeightOfTreeAt(Position position)
        => Data[position];

    public IEnumerable<Position> LeftEdgePositions()
        => EnumerateRange(0, Bounds.Height - 1).Select(y => new Position(0, y));

    public IEnumerable<Position> RightEdgePositions()
        => EnumerateRange(0, Bounds.Height - 1).Select(y => new Position(Bounds.Width - 1, y));

    public IEnumerable<Position> TopEdgePositions()
        => EnumerateRange(0, Bounds.Height - 1).Select(x => new Position(x, 0));

    public IEnumerable<Position> BottomEdgePositions()
        => EnumerateRange(0, Bounds.Height - 1).Select(x => new Position(x, Bounds.Height - 1));

    public IEnumerable<Position> PositionsRightFrom(Position position)
        => EnumerateRange(position.X, Bounds.Width - 1).Select(x => position with { X = x });

    public IEnumerable<Position> PositionsLeftFrom(Position position)
        => EnumerateRange(position.X, 0).Select(x => position with { X = x });

    public IEnumerable<Position> PositionsDownFrom(Position position)
        => EnumerateRange(position.Y, Bounds.Height - 1).Select(y => position with { Y = y });

    public IEnumerable<Position> PositionsUpFrom(Position position)
        => EnumerateRange(position.Y, 0).Select(y => position with { Y = y });

    private IEnumerable<int> EnumerateRange(int from, int to)
    {
        return to < from
                   ? Enumerable.Range(0, from - to + 1).Select(n => from - n)
                   : Enumerable.Range(from, to - from + 1);
    }

    public static Grid Read(string? fileName = null)
    {
        return new Grid(InputFile.ReadAllLines(fileName));
    }
}
