using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Aoc2022_Day22;

internal abstract partial record Instruction
{
    public static Instruction[] ReadAll(string input)
        => Pattern().Matches(input)
                    .Select<Match, Instruction>(m =>
                                                {
                                                    if (m.Groups["RotateLeft"].Success) return new RotateLeft();
                                                    if (m.Groups["RotateRight"].Success) return new RotateRight();
                                                    if (m.Groups["MoveForward"].Success) return new MoveForward(int.Parse(m.Groups["MoveForward"].Value));
                                                    throw new UnreachableException();
                                                })
                    .ToArray();

    [GeneratedRegex("((?<RotateLeft>L)|(?<RotateRight>R)|(?<MoveForward>\\d+))", RegexOptions.ExplicitCapture)]
    private static partial Regex Pattern();
}

internal sealed record RotateLeft : Instruction;
internal sealed record RotateRight : Instruction;
internal sealed record MoveForward(int By) : Instruction;
