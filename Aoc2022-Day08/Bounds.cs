namespace Aoc2022_Day08;

internal record Bounds(int Width, int Height)
{
    public static implicit operator Bounds((int Width, int Height) position)
        => new (position.Width, position.Height);
}
