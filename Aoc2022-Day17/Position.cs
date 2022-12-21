namespace Aoc2022_Day17;

internal readonly record struct Position(int X, long Y)
{
    public Position Left() => this with { X = X - 1 };
    public Position Right() => this with { X = X + 1 };
    public Position Down() => this with { Y = Y - 1 };

    public static implicit operator Position((int X, long Y) position)
        => new (position.X, position.Y);
}
