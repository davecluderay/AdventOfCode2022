namespace Aoc2022_Day24;

internal static class Calculate
{
    private static int GreatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static int LeastCommonMultiple(int a, int b)
    {
        return a / GreatestCommonFactor(a, b) * b;
    }
}
