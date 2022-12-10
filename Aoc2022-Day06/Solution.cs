using System.Diagnostics;

namespace Aoc2022_Day06;

internal class Solution
{
    public string Title => "Day 6: Tuning Trouble";

    public object PartOne()
    {
        return FindPositionAfterUniqueSequenceLengthOf(4);
    }

    public object PartTwo()
    {
        return FindPositionAfterUniqueSequenceLengthOf(14);
    }

    private static int FindPositionAfterUniqueSequenceLengthOf(int uniqueSequenceLength)
    {
        var data = InputFile.ReadAllText();
        var pos = uniqueSequenceLength;
        while (pos <= data.Length)
        {
            var span = data.AsSpan(pos - uniqueSequenceLength, uniqueSequenceLength);
            var hasDuplicate = false;
            for (var i = uniqueSequenceLength - 1; i >= 0; i--)
            {
                if (!span.Slice(i + 1).Contains(span[i]))
                    continue;

                pos += i + 1;
                hasDuplicate = true;
                break;
            }

            if (!hasDuplicate)
                return pos;
        }

        throw new UnreachableException("No start of packet sequence found.");
    }
}
