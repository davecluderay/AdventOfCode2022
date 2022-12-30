namespace Aoc2022_Day22;

internal class WrappingMovementStrategy : IMovementStrategy
{
    private readonly string[] _data;

    public WrappingMovementStrategy(string[] data)
    {
        _data = data;
    }

    public MapPosition MoveForward(MapPosition position)
    {
        var minColumn = _data[position.Row].IndexOfAny(new[] { '.', '#' });
        var maxColumn = _data[position.Row].LastIndexOfAny(new[] { '.', '#' });
        var minRow = Array.FindIndex(_data, x => x.Length > position.Column && x[position.Column] != ' ');
        var maxRow = Array.FindLastIndex(_data, x => x.Length > position.Column && x[position.Column] != ' ');
        var isTransitioning = position.Facing switch
                              {
                                  Direction.Right => position.Column == maxColumn,
                                  Direction.Down  => position.Row == maxRow,
                                  Direction.Left  => position.Column == minColumn,
                                  Direction.Up    => position.Row == minRow,
                                  _               => throw new ArgumentOutOfRangeException(nameof(position), $"Unknown direction: {position.Facing}")
                              };
        return isTransitioning
                   ? position.Facing switch
                     {
                         Direction.Right => position with { Column = minColumn },
                         Direction.Down  => position with { Row = minRow },
                         Direction.Left  => position with { Column = maxColumn },
                         Direction.Up    => position with { Row = maxRow },
                         _               => throw new ArgumentOutOfRangeException(nameof(position), $"Unknown direction: {position.Facing}")
                     }
                   : position.Facing switch
                     {
                         Direction.Right => position with { Column = position.Column + 1 },
                         Direction.Down  => position with { Row = position.Row + 1 },
                         Direction.Left  => position with { Column = position.Column - 1 },
                         Direction.Up    => position with { Row = position.Row - 1 },
                         _               => throw new ArgumentOutOfRangeException(nameof(position), $"Unknown direction: {position.Facing}")
                     };
    }
}
