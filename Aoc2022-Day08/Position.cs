namespace Aoc2022_Day08;

internal record Position(int X, int Y)
{
    public static implicit operator Position((int X, int Y) position)
        => new (position.X, position.Y);
}
