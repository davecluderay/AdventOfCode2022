namespace Aoc2022_Day15;

internal readonly record struct Range(int Start, int End)
{
    public int Length
        => End - Start + 1;

    public bool Contains(Range other)
        => Start <= other.Start && End >= other.End;

    public bool Contains(int value)
        => value >= Start && value <= End;

    public bool Overlaps(Range other)
        => Start <= other.End && End >= other.Start;

    public int OverlapLength(Range other)
        => Math.Max(0, Math.Min(End, other.End) - Math.Max(Start, other.Start) + 1);

    public static IReadOnlyList<Range> CombineAll(IEnumerable<Range> ranges)
    {
        var queue = new Queue<Range>(ranges);
        var results = new List<Range>();
        while (queue.Count > 0)
        {
            var range = queue.Dequeue();
            var index = results.FindIndex(r => range.CanCombineWith(r));
            if (index < 0)
            {
                results.Add(range);
            }
            else
            {
                var combined = results[index].CombineWith(range);
                queue.Enqueue(combined);
                results.RemoveAt(index);
            }
        }
        return results.OrderBy(r => (From: r.Start, To: r.End))
                      .ToList()
                      .AsReadOnly();
    }

    public static implicit operator Range((int From, int To) position)
        => new (position.From, position.To);

    private bool CanCombineWith(Range other)
        => (Start <= other.End && End >= other.Start) || Start == other.End + 1 || other.Start == End + 1;

    private Range CombineWith(Range other)
        => CanCombineWith(other)
               ? new Range(Math.Min(Start, other.Start), Math.Max(End, other.End))
               : throw new ArgumentException("Not an overlapping range.", nameof(other));
}
