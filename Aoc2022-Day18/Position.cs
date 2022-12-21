namespace Aoc2022_Day18;

internal readonly record struct Position(int X, int Y, int Z)
{
    public IEnumerable<Position> GetAdjacentPositions()
    {
        yield return this with { X = X - 1 };
        yield return this with { X = X + 1 };
        yield return this with { Y = Y - 1 };
        yield return this with { Y = Y + 1 };
        yield return this with { Z = Z - 1 };
        yield return this with { Z = Z + 1 };
    }

    public static Position Read(string input)
    {
        var comma1Index = input.IndexOf(',');
        var comma2Index = input.IndexOf(',', comma1Index + 1);
        return new Position(int.Parse(input.AsSpan(0, comma1Index)),
                            int.Parse(input.AsSpan(comma1Index + 1, comma2Index - comma1Index - 1)),
                            int.Parse(input.AsSpan(comma2Index + 1)));
    }

    public static implicit operator Position((int X, int Y, int Z) position)
        => new (position.X, position.Y, position.Z);
}
