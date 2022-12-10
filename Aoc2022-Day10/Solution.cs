using System.Text;

namespace Aoc2022_Day10;

internal class Solution
{
    public string Title => "Day 10: Cathode-Ray Tube";

    public object PartOne()
    {
        var cpu = new Cpu();
        cpu.LoadProgram();

        var nextSampleAtCycle = 20;
        var signalStrength = 0;
        while (true)
        {
            cpu.ExecuteCycle();
            if (cpu.CurrentCycle == nextSampleAtCycle)
            {
                signalStrength += cpu.CurrentCycle * cpu.Register;
                nextSampleAtCycle += 40;
                if (nextSampleAtCycle > 220) break;
            }
        }

        return signalStrength;
    }
        
    public object PartTwo()
    {
        const char solidChar = '\u2588';
        const char clearChar = ' ';

        var cpu = new Cpu();
        cpu.LoadProgram();

        var crtOutput = new StringBuilder();
        for (var scanPosition = 0; scanPosition < 40 * 6; scanPosition++)
        {
            if (scanPosition % 40 == 0)
                crtOutput.Append('\n');

            cpu.ExecuteCycle();

            crtOutput.Append(Math.Abs(scanPosition % 40 - cpu.Register) < 2
                                 ? solidChar
                                 : clearChar);
        }

        return crtOutput.ToString();
    }
}
