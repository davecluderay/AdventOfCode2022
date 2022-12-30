namespace Aoc2022_Day22;

internal record MapPosition(int Row, int Column, Direction Facing);

internal enum Direction
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}
