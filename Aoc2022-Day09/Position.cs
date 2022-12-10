using System.Numerics;

namespace Aoc2022_Day09;

internal record struct Position(int X, int Y)
{
    public Position Move(Vector vector)
        => (X + vector.X, Y + vector.Y);

    public static implicit operator Position((int X, int Y) position)
        => new (position.X, position.Y);

    public Vector Delta(Position other)
        => new (X - other.X, Y - other.Y);
}

internal record struct Vector(int X, int Y)
{
    public static implicit operator Vector((int X, int Y) position)
        => new (position.X, position.Y);
}
