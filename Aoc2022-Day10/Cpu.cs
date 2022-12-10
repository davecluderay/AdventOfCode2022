namespace Aoc2022_Day10;

internal class Cpu
{
    private Instruction[]? _instructions;
    private int _instructionPointer;
    private ExecutingInstruction? _currentInstruction;

    public int CurrentCycle { get; private set; }
    public int Register { get; private set; } = 1;

    public void LoadProgram(string? fileName = null)
    {
        _instructions = InputFile.ReadAllLines()
                                 .Select(Instruction.Read)
                                 .ToArray();
        _instructionPointer = 0;
    }

    public void ExecuteCycle()
    {
        if (_instructions is null) throw new InvalidOperationException("No program loaded.");

        ++CurrentCycle;

        if (_currentInstruction is not null && _currentInstruction?.CompletesAtCycle == CurrentCycle)
        {
            Register += _currentInstruction.Instruction.IncrementsRegisterBy;
        }

        if (_currentInstruction is null || _currentInstruction.CompletesAtCycle == CurrentCycle)
        {
            if (_instructionPointer == _instructions.Length) throw new InvalidOperationException("Program has completed.");
            var nextInstruction = _instructions[_instructionPointer++];
            var completesAtCycle = CurrentCycle + nextInstruction.TakesCycles;
            _currentInstruction = new ExecutingInstruction(nextInstruction, completesAtCycle);
        }
    }

    private record ExecutingInstruction(Instruction Instruction, int CompletesAtCycle);

    private record Instruction(int IncrementsRegisterBy, int TakesCycles)
    {
        public static Instruction Read(string input)
        {
            if (input == "noop")
                return new(IncrementsRegisterBy: 0, TakesCycles: 1);
            if (input.StartsWith("addx "))
                return new(IncrementsRegisterBy: Convert.ToInt32(input.Substring(5)), 2);
            throw new FormatException($"Not an instruction: {input}");
        }
    }
}
