namespace Aoc2022_Day19;

internal class Solution
{
    public string Title => "Day 19: Not Enough Minerals";
    
    public object PartOne()
    {
        var result = 0;
        foreach (var blueprint in ReadBlueprints())
        {
            var geodeCount = FindBestGeodeHarvest(blueprint,
                                                  Inventory.StartingInventory,
                                                  Robots.StartingRobots,
                                                  remainingMinutes: 24,
                                                  geodesAlreadyOpened: 0,
                                                  new BestGeodeHarvestCache());
            result += blueprint.Id * geodeCount;
        }
        return result;
    }

    public object PartTwo()
    {
        var result = 1;
        foreach (var blueprint in ReadBlueprints().Take(3))
        {
            var geodeCount = FindBestGeodeHarvest(blueprint,
                                                  Inventory.StartingInventory,
                                                  Robots.StartingRobots,
                                                  remainingMinutes: 32,
                                                  geodesAlreadyOpened: 0,
                                                  new BestGeodeHarvestCache());
            result *= geodeCount;
        }
        return result;
    }

    private int FindBestGeodeHarvest(Blueprint blueprint, Inventory inventory, Robots robots, int remainingMinutes, int geodesAlreadyOpened, BestGeodeHarvestCache cache)
    {
        if (remainingMinutes <= 0)
            return 0;
    
        if (cache.HasCachedValueFor(inventory, robots, remainingMinutes))
            return cache.GetCachedValueFor(inventory, robots, remainingMinutes);

        var geodeOpeningRate = robots.GeodeRobots;
        
        // Bail out if it's already a lost cause.
        if (geodesAlreadyOpened + Enumerable.Range(0, remainingMinutes).Sum(n => robots.GeodeRobots + n) < cache.CurrentBest)
            return 0;

        // Scenario: do nothing for the remainder of the time.
        var bestOnwardHarvest = remainingMinutes * geodeOpeningRate;

        // Scenario: create a geode robot.
        if (robots.ObsidianRobots > 0)
        {
            var minutesBeforeActive = Math.Max(1, 1 + Math.Max((blueprint.GeodeRobotCost.OreUnits - inventory.Ore + robots.OreRobots - 1) / robots.OreRobots,
                                                               (blueprint.GeodeRobotCost.ObsidianUnits - inventory.Obsidian + robots.ObsidianRobots - 1) / robots.ObsidianRobots));
            if (remainingMinutes - minutesBeforeActive > 0)
            {
                bestOnwardHarvest = Math.Max(bestOnwardHarvest,
                                             minutesBeforeActive * geodeOpeningRate +
                                             FindBestGeodeHarvest(blueprint,
                                                                  inventory.Accumulate(robots, minutesBeforeActive)
                                                                           .ConsumeOre(blueprint.GeodeRobotCost.OreUnits)
                                                                           .ConsumeObsidian(blueprint.GeodeRobotCost.ObsidianUnits),
                                                                  robots.BuildGeodeRobot(),
                                                                  remainingMinutes - minutesBeforeActive,
                                                                  geodesAlreadyOpened + minutesBeforeActive * geodeOpeningRate,
                                                                  cache));
            }
        }

        // Scenario: create an obsidian robot.
        if (robots.ClayRobots > 0)
        {
            var minutesBeforeActive = Math.Max(1, 1 + Math.Max((blueprint.ObsidianRobotCost.OreUnits - inventory.Ore + robots.OreRobots - 1) / robots.OreRobots,
                                                               (blueprint.ObsidianRobotCost.ClayUnits - inventory.Clay + robots.ClayRobots - 1) / robots.ClayRobots));
            if (remainingMinutes - minutesBeforeActive > 0)
            {
                bestOnwardHarvest = Math.Max(bestOnwardHarvest,
                                             minutesBeforeActive * geodeOpeningRate +
                                             FindBestGeodeHarvest(blueprint,
                                                                  inventory.Accumulate(robots, minutesBeforeActive)
                                                                           .ConsumeOre(blueprint.ObsidianRobotCost.OreUnits)
                                                                           .ConsumeClay(blueprint.ObsidianRobotCost.ClayUnits),
                                                                  robots.BuildObsidianRobot(),
                                                                  remainingMinutes - minutesBeforeActive,
                                                                  geodesAlreadyOpened + minutesBeforeActive * geodeOpeningRate,
                                                                  cache));
            }
        }

        // Scenario: create a clay robot.
        {
            var minutesBeforeActive = Math.Max(1, 1 + (blueprint.ClayRobotCost.OreUnits - inventory.Ore + robots.OreRobots - 1) / robots.OreRobots);
            if (remainingMinutes - minutesBeforeActive > 0)
            {
                bestOnwardHarvest = Math.Max(bestOnwardHarvest,
                                             minutesBeforeActive * geodeOpeningRate +
                                             FindBestGeodeHarvest(blueprint,
                                                                  inventory.Accumulate(robots, minutesBeforeActive)
                                                                           .ConsumeOre(blueprint.ClayRobotCost.OreUnits),
                                                                  robots.BuildClayRobot(),
                                                                  remainingMinutes - minutesBeforeActive,
                                                                  geodesAlreadyOpened + minutesBeforeActive * geodeOpeningRate,
                                                                  cache));
            }
        }

        // Scenario: create an ore robot.
        {
            var minutesBeforeActive = Math.Max(1, 1 + (blueprint.OreRobotCost.OreUnits - inventory.Ore + robots.OreRobots - 1) / robots.OreRobots);
            if (remainingMinutes - minutesBeforeActive > 0)
            {
                bestOnwardHarvest = Math.Max(bestOnwardHarvest,
                                             minutesBeforeActive * geodeOpeningRate +
                                             FindBestGeodeHarvest(blueprint,
                                                                  inventory.Accumulate(robots, minutesBeforeActive)
                                                                           .ConsumeOre(blueprint.OreRobotCost.OreUnits),
                                                                  robots.BuildOreRobot(),
                                                                  remainingMinutes - minutesBeforeActive,
                                                                  geodesAlreadyOpened + minutesBeforeActive * geodeOpeningRate,
                                                                  cache));
            }
        }

        cache.CacheValueFor(inventory, robots, remainingMinutes, bestOnwardHarvest);
        cache.CurrentBest = Math.Max(cache.CurrentBest, geodesAlreadyOpened + bestOnwardHarvest);
        return bestOnwardHarvest;
    }

    private static Blueprint[] ReadBlueprints(string? fileName = null)
        => Blueprint.ReadBlueprints(InputFile.ReadAllText(fileName));

    private readonly record struct Inventory(int Ore, int Clay, int Obsidian)
    {
        public Inventory Accumulate(Robots robots, int minutes = 1)
            => new(Ore + robots.OreRobots * minutes,
                   Clay + robots.ClayRobots * minutes,
                   Obsidian + robots.ObsidianRobots * minutes);

        public Inventory ConsumeOre(int units) => this with { Ore = Ore - units };
        public Inventory ConsumeClay(int units) => this with { Clay = Clay - units };
        public Inventory ConsumeObsidian(int units) => this with { Obsidian = Obsidian - units };

        public static Inventory StartingInventory => new(0, 0, 0);
    }

    private readonly record struct Robots(int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots)
    {
        public Robots BuildOreRobot() => this with { OreRobots = OreRobots + 1 };
        public Robots BuildClayRobot() => this with { ClayRobots = ClayRobots + 1 };
        public Robots BuildObsidianRobot() => this with { ObsidianRobots = ObsidianRobots + 1 };
        public Robots BuildGeodeRobot() => this with { GeodeRobots = GeodeRobots + 1 };
        public static Robots StartingRobots => new(1, 0, 0, 0);
    }

    private class BestGeodeHarvestCache
    {
        private readonly Dictionary<(Inventory, Robots, int), int> _cache = new();

        public int CurrentBest { get; set; }

        public void CacheValueFor(Inventory inventory, Robots robots, int minutesRemaining, int value)
            => _cache[(inventory, robots, minutesRemaining)] = value;

        public bool HasCachedValueFor(Inventory inventory, Robots robots, int minutesRemaining)
            => _cache.ContainsKey((inventory, robots, minutesRemaining));

        public int GetCachedValueFor(Inventory inventory, Robots robots, int minutesRemaining)
            => _cache[(inventory, robots, minutesRemaining)];
    }
}

