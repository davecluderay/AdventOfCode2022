namespace Aoc2022_Day22;

internal class Solution
{
    public string Title => "Day 22: Monkey Map";

    public object PartOne()
    {
        var (map, instructions) = ReadInput();
        var position = map.SelectStartingPosition();
        foreach (var instruction in instructions)
        {
            position = FollowInstruction(instruction, map, position);
        }
        return (position.Row + 1) * 1000 + (position.Column + 1) * 4 + (int)position.Facing;
    }

    public object PartTwo()
    {
        var (map, instructions) = ReadInput();
        map.FoldIntoCube();
        var position = map.SelectStartingPosition();
        foreach (var instruction in instructions)
        {
            position = FollowInstruction(instruction, map, position);
        }
        return (position.Row + 1) * 1000 + (position.Column + 1) * 4 + (int)position.Facing;
    }

    private static MapPosition FollowInstruction(Instruction instruction, Map map, MapPosition position)
    {
        return instruction switch
              {
                  RotateLeft    => position with { Facing = (Direction)(((int)position.Facing + 3) % 4) },
                  RotateRight   => position with { Facing = (Direction)(((int)position.Facing + 1) % 4) },
                  MoveForward m => map.CalculateMovementTarget(position, m.By),
                  _ => throw new ArgumentOutOfRangeException(nameof(instruction), $"Unknown instruction type: {instruction.GetType().Name}")
              };
    }

    private static (Map Map, Instruction[] Instructions) ReadInput(string? fileName = null)
    {
        var sections = InputFile.ReadInSections(fileName).ToArray();
        var map = new Map(sections[0]);
        var instructions = Instruction.ReadAll(sections[^1].Single());
        return (map, instructions);
    }
}
