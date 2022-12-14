namespace Aoc2022_Day13;

internal record Value : IComparable<Value>
{
    private int? Integer { get; init; }
    private IReadOnlyList<Value>? List { get; init; }

    public int CompareTo(Value? other)
    {
        var left = this;
        var right = other!;
        if (left.Integer.HasValue && right.Integer.HasValue)
            return Comparer<int>.Default.Compare(left.Integer.Value, right.Integer.Value);

        if (left.Integer.HasValue)
            left = new Value { List = new[] { left }.AsReadOnly() };
        if (right.Integer.HasValue)
            right = new Value { List = new[] { right }.AsReadOnly() };
        
        for (var i = 0; i < Math.Max(left.List!.Count, right.List!.Count); i++)
        {
            if (i == left.List.Count) return -1;
            if (i == right.List.Count) return 1;
            var el = left.List[i].CompareTo(right.List[i]);
            if (el != 0) return el;
        }

        return 0;
    }

    public static Value Read(string input)
    {
        var (value, _) = ReadNextValue(input);
        return value;
    }

    private static (Value value, int length) ReadNextValue(ReadOnlySpan<char> span)
    {
        const string digits = "1234567890";

        if (span.StartsWith("[]"))
        {
            // Empty list.
            return (new Value { List = Array.Empty<Value>().AsReadOnly() }, 2);
        }
        
        if (span.StartsWith("["))
        {
            // List with one or more values.
            var list = new List<Value>();
            var readSoFar = 1;
            while (true)
            {
                var (value, length) = ReadNextValue(span.Slice(readSoFar));
                list.Add(value);
                readSoFar += length + 1;
                if (span[readSoFar - 1] == ']')
                    return (new Value { List = list.AsReadOnly() }, readSoFar);
            }
        }

        // Simple integer value.
        var i = span.IndexOfAnyExcept(digits);
        var text = i < 0 ? span : span.Slice(0, i);
        return (new Value { Integer = int.Parse(text) }, text.Length);
    }
}
