using System.Text.RegularExpressions;

namespace Aoc2022_Day19;

internal partial record Blueprint(int Id, OreRobotCost OreRobotCost, ClayRobotCost ClayRobotCost, ObsidianRobotCost ObsidianRobotCost, GeodeRobotCost GeodeRobotCost)
{
    public static Blueprint[] ReadBlueprints(string input)
    {
        var matches = Pattern().Matches(input);
        var blueprints = new List<Blueprint>(matches.Count);
        foreach (Match match in matches)
        {
            blueprints.Add(
                new Blueprint(Convert.ToInt32(match.Groups["Id"].Value),
                              new OreRobotCost(Convert.ToInt32(match.Groups["OreRobotOreCost"].Value)),
                              new ClayRobotCost(Convert.ToInt32(match.Groups["ClayRobotOreCost"].Value)),
                              new ObsidianRobotCost(Convert.ToInt32(match.Groups["ObsidianRobotOreCost"].Value),
                                                    Convert.ToInt32(match.Groups["ObsidianRobotClayCost"].Value)),
                              new GeodeRobotCost(Convert.ToInt32(match.Groups["GeodeRobotOreCost"].Value),
                                                 Convert.ToInt32(match.Groups["GeodeRobotObsidianCost"].Value))));
        }
        return blueprints.ToArray();
    }

    [GeneratedRegex(@"Blueprint (?<Id>\d+):\W*" +
                    @"Each ore robot costs (?<OreRobotOreCost>\d+) ore\.\W*" +
                    @"Each clay robot costs (?<ClayRobotOreCost>\d+) ore\.\W*" +
                    @"Each obsidian robot costs (?<ObsidianRobotOreCost>\d+) ore and (?<ObsidianRobotClayCost>\d+) clay\.\W*" +
                    @"Each geode robot costs (?<GeodeRobotOreCost>\d+) ore and (?<GeodeRobotObsidianCost>\d+) obsidian.",
                    RegexOptions.ExplicitCapture)]
    private static partial Regex Pattern();
}

internal readonly record struct OreRobotCost(int OreUnits);
internal readonly record struct ClayRobotCost(int OreUnits);
internal readonly record struct ObsidianRobotCost(int OreUnits, int ClayUnits);
internal readonly record struct GeodeRobotCost(int OreUnits, int ObsidianUnits);
