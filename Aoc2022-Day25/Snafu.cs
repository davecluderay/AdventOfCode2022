using System.Diagnostics;
using System.Text;

namespace Aoc2022_Day25;

internal static class Snafu
{
    public static long ToDecimal(string snafu)
    {
        var len = snafu.Length;
        var result = 0L;
        for (var i = 0; i < snafu.Length; i++)
        {
            var pow = len - 1 - i;
            var val = snafu[i] switch
                      {
                          '2' => 2,
                          '1' => 1,
                          '0' => 0,
                          '-' => -1,
                          '=' => -2
                      };
            result += (long)Math.Pow(5, pow) * val;
        }

        return result;
    }

    public static string FromDecimal(long @decimal)
    {
        var result = new StringBuilder();
        while (@decimal > 0)
        {
            var v = @decimal % 5;
            result.Insert(0, v switch
                             {
                                 < 3 => v.ToString()[0],
                                 3   => '=',
                                 4   => '-',
                                 _   => throw new UnreachableException()
                             });
            @decimal = v < 3
                           ? @decimal / 5
                           : (@decimal + 5) / 5;
        }
        return result.ToString();
    }
}
