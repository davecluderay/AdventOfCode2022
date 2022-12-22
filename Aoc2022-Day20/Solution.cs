namespace Aoc2022_Day20;

internal class Solution
{
    public string Title => "Day 20: Grove Positioning System";

    public object PartOne()
    {
        var encrypted = InputFile.ReadAllLines()
                                 .Select(long.Parse)
                                 .ToArray();
        var mixed = Mix(encrypted);
        var zeroIndex = Array.IndexOf(mixed, 0);
        var (x, y, z) = (mixed[(zeroIndex + 1000) % mixed.Length],
                         mixed[(zeroIndex + 2000) % mixed.Length],
                         mixed[(zeroIndex + 3000) % mixed.Length]);
        return x + y + z;
    }

    public object PartTwo()
    {
        var encrypted = InputFile.ReadAllLines()
                                 .Select(long.Parse)
                                 .Select(n => n * 811589153)
                                 .ToArray();
        var mixed = Mix(encrypted, rounds: 10);
        var zeroIndex = Array.IndexOf(mixed, 0);
        var (x, y, z) = (mixed[(zeroIndex + 1000) % mixed.Length],
                         mixed[(zeroIndex + 2000) % mixed.Length],
                         mixed[(zeroIndex + 3000) % mixed.Length]);
        return x + y + z;
    }

    private static long[] Mix(long[] data, int rounds = 1)
    {
        var list = new LinkedList<long>();
        var nodes = data.Select(e => list.AddLast(e)).ToArray();
        for (var round = 1; round <= rounds; round++)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                var moveBy = Math.Abs(data[i]) % (data.Length - 1);
                var direction = Math.Sign(data[i]);

                if (moveBy == 0) continue;

                var moveNode = nodes[i];
                var newNeighbour = moveNode;
                for (var n = 0; n < moveBy; n++)
                {
                    newNeighbour = direction > 0
                                       ? newNeighbour.Next ?? list.First!
                                       : newNeighbour.Previous ?? list.Last!;
                }

                list.Remove(moveNode);
                if (direction > 0)
                {
                    list.AddAfter(newNeighbour, moveNode);
                }
                else
                {
                    list.AddBefore(newNeighbour, moveNode);
                }
            }
        }

        return list.ToArray();
    }
}
