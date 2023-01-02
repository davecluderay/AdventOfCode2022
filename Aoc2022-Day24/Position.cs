namespace Aoc2022_Day24;

internal readonly record struct Position(int X, int Y)
{
    public static implicit operator Position((int X, int Y) position)
        => new (position.X, position.Y);

    public override string ToString()
        => $"{X},{Y}";
}
