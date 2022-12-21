namespace Aoc2022_Day17;

internal class Chamber
{
    private const int Width = 7;
    public long Height { get; private set; }
    private readonly HashSet<Position> _filledArea = new();

    public void FixPosition(string[] rock, Position at)
    {
        foreach (var position in CalculateFilledPositions(rock, at))
        {
            _filledArea.Add(position);
            Height = Math.Max(Height, position.Y + 1);
        }
    }

    public bool CanOccupyPosition(string[] rock, Position at)
    {
        return CalculateFilledPositions(rock, at).All(p => !IsOutOfBounds(p) && !_filledArea.Contains(p));
    }

    public Position SelectStartingPositionFor(string[] rock)
    {
        return (2, Height + 3 + rock.Length - 1);
    }

    private bool IsOutOfBounds(Position position)
        => position.X < 0 || position.X >= Width || position.Y < 0;

    private IEnumerable<Position> CalculateFilledPositions(string[] rock, Position at)
    {
        for (var ry = 0; ry < rock.Length; ry++)
        for (var rx = 0; rx < rock[ry].Length; rx++)
        {
            if (rock[ry][rx] != '#') continue;
            yield return (at.X + rx, at.Y - ry);
        }
    }
}
