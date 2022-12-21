namespace Aoc2022_Day17;

internal class HeightPredictor
{
    private readonly long _afterFallCount;
    private readonly List<TrackedData> _trackingData = new();

    private int _currentFallCount;
    private RepetitionPattern? _detectedPattern;
    public long? PredictedY { get; private set; }

    public HeightPredictor(long afterFallCount)
    {
        _afterFallCount = afterFallCount;
    }

    public void Record(int rockIndex, int jetIndex, Position position)
    {
        _currentFallCount++;
        if (_detectedPattern is null) DetectRepetition(rockIndex, jetIndex, position);
        if (_detectedPattern is not null) PredictY(position);
    }

    private void DetectRepetition(int rockIndex, int jetIndex, Position position)
    {
        if (rockIndex != 0) return; // Monitor a single rock type.

        // Record details for this rock's fall.
        var differenceInY = position.Y - (_trackingData.Any() ? _trackingData[^1].Position.Y : 0L);
        _trackingData.Add(new TrackedData(position, jetIndex, differenceInY, _currentFallCount));

        // Look for repetition with the previously recorded details.
        var possibleRepeats = _trackingData.FindAll(r => r.Position.X == position.X &&
                                                         r.JetIndex == jetIndex &&
                                                         r.DifferenceInY == differenceInY);
        if (possibleRepeats.Count <= 12) return;
        
        for (var n = 1; n <= possibleRepeats.Count / 3; n++)
        {
            // Look backwards for repeating groups of n where the fall count delta and y delta match.
            var list = new List<(long DeltaY, int DeltaFallCount)>();
            for (var i = possibleRepeats.Count - n - 1; i >= 0; i -= n)
            {
                var start = possibleRepeats[i];
                var end = possibleRepeats[i + n];
                list.Add((end.Position.Y - start.Position.Y, end.FallCount - start.FallCount));
            }

            if (list.Distinct().Count() != 1) continue;

            var repeatBase = possibleRepeats[possibleRepeats.Count - list.Count * n - 1];
            _detectedPattern = new RepetitionPattern(BaseFallCount: repeatBase.FallCount,
                                                     BaseY: repeatBase.Position.Y + 1,
                                                     RepeatsAfterFallCount: list.First().DeltaFallCount,
                                                     IncreaseInY: list.First().DeltaY);
            return;
        }
    }

    private void PredictY(Position position)
    {
        var pattern = _detectedPattern!;
        var targetOffset = (_afterFallCount - pattern.BaseFallCount) % pattern.RepeatsAfterFallCount;
        var offset = (_currentFallCount - pattern.BaseFallCount) % pattern.RepeatsAfterFallCount;
        if (offset != targetOffset) return;

        var deltaY = position.Y - _trackingData[^1].Position.Y;
        PredictedY = pattern.BaseY + (_afterFallCount - pattern.BaseFallCount) / pattern.RepeatsAfterFallCount * pattern.IncreaseInY + deltaY;
    }

    private record TrackedData(Position Position, int JetIndex, long DifferenceInY, int FallCount);
    private record RepetitionPattern(int BaseFallCount, long BaseY, int RepeatsAfterFallCount, long IncreaseInY);
}
