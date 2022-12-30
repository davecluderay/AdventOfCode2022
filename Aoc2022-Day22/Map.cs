namespace Aoc2022_Day22;

internal class Map
{
    private readonly string[] _data;
    private IMovementStrategy _movementStrategy;

    public Map(string[] data)
    {
        _data = data;
        _movementStrategy = new WrappingMovementStrategy(data);
    }

    public MapPosition SelectStartingPosition()
        => new(0, _data[0].IndexOf('.'), Direction.Right);

    public void FoldIntoCube()
        => _movementStrategy = new FoldedCubeMovementStrategy(_data);

    public MapPosition CalculateMovementTarget(MapPosition startAt, int moveBy)
    {
        if (moveBy == 0)
            return startAt;

        var current = startAt;
        for (var n = 0; n < moveBy; n++)
        {
            var next = _movementStrategy.MoveForward(current);
            if (_data[next.Row][next.Column] == '#')
                return current;
            current = next;
        }

        return current;
    }
}
