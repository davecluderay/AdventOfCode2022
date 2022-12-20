using System.Collections.ObjectModel;

namespace Aoc2022_Day16;

internal class ValveMap
{
    public const string StartFromValveId = "AA";

    private readonly ReadOnlyDictionary<string, int> _maskBitsByValveId;
    private readonly ReadOnlyDictionary<string, int> _flowRatesByValveId;
    private readonly ReadOnlyDictionary<string, Connection[]> _connectionsByValveId;

    private ValveMap(HashSet<string> valveIds, Dictionary<string, int> flowRatesByValveId, Dictionary<string, Connection[]> connectionsByValveId)
    {
        _flowRatesByValveId = flowRatesByValveId.AsReadOnly();
        _connectionsByValveId = connectionsByValveId.AsReadOnly();
        _maskBitsByValveId = valveIds.Select((id, ix) => (ValveId: id, Bit: ix))
                                     .ToDictionary(t => t.ValveId, t => t.Bit)
                                     .AsReadOnly();
    }

    public int FlowRateOfValve(string valveId)
        => _flowRatesByValveId.GetValueOrDefault(valveId, 0);

    public int MaskBitFor(string valveId)
        => _maskBitsByValveId.GetValueOrDefault(valveId, 0);

    public IReadOnlyCollection<Connection> ConnectionsFrom(string valveId)
        => _connectionsByValveId.GetValueOrDefault(valveId, Array.Empty<Connection>());

    public static ValveMap Read(string? fileName = null)
    {
        var rooms = InputFile.ReadAllLines(fileName)
                             .Select(Room.Read)
                             .ToDictionary(r => r.ValveId);

        var valveIds = rooms.Values.Where(r => r.ValveFlowRate > 0 || r.ValveId == StartFromValveId)
                            .Select(r => r.ValveId)
                            .ToHashSet();

        var flowRatesByValveId = valveIds.ToDictionary(v => v, v => rooms[v].ValveFlowRate);

        // Pre-calculate the connections to all other significant valves.
        var connectionsByValveId = new Dictionary<string, Connection[]>();
        foreach (var valveId in valveIds)
        {
            var connections = new List<Connection>(valveIds.Count - 1);
            var examine = new Queue<(int MinutesAway, string ValveId)>(new[] { (0, valveId) });
            var visited = new HashSet<string> { valveId };
            while (examine.Count > 0)
            {
                var (minutesAway, otherValveId) = examine.Dequeue();

                if (otherValveId != valveId && valveIds.Contains(otherValveId))
                {
                    connections.Add(new Connection(otherValveId, minutesAway));
                }

                foreach (var nextValveId in rooms[otherValveId].HasTunnelsLeadingToValveIds)
                {
                    if (!visited.Contains(nextValveId))
                    {
                        visited.Add(nextValveId);
                        examine.Enqueue((minutesAway + 1, nextValveId));
                    }
                }
            }
            connectionsByValveId[valveId] = connections.ToArray();
        }

        return new ValveMap(valveIds, flowRatesByValveId, connectionsByValveId);
    }
    
    public readonly record struct Connection(string ValveId, int MinutesAway);
}
