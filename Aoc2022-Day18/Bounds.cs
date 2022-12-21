namespace Aoc2022_Day18;

internal readonly record struct Bounds(int MinX, int MaxX, int MinY, int MaxY, int MinZ, int MaxZ)
{
    public bool Contains(Position position) => position.X >= MinX && position.X <= MaxX &&
                                               position.Y >= MinY && position.Y <= MaxY &&
                                               position.Z >= MinZ && position.Z <= MaxZ;
    public IEnumerable<Position> GetAllSurfacePositions()
    {
        for (var x = MinX; x <= MaxX; x += MaxX - MinX)
        for (var y = MinY; y <= MaxY; y++)
        for (var z = MinZ; z <= MaxZ; z++)
            yield return (x, y, z);
        for (var x = MinX + 1; x < MaxX; x++)
        for (var y = MinY; y <= MaxY; y += MaxY - MinY)
        for (var z = MinZ; z <= MaxZ; z++)
            yield return (x, y, z);
        for (var x = MinX + 1; x < MaxX; x++)
        for (var y = MinY + 1; y < MaxY; y++)
        for (var z = MinZ; z <= MaxZ; z+= MaxZ - MinZ)
            yield return (x, y, z);
    }

    public static Bounds Calculate(IReadOnlyCollection<Position> positions)
        => new(MinX: positions.Min(c => c.X), MaxX: positions.Max(c => c.X),
               MinY: positions.Min(c => c.Y), MaxY: positions.Max(c => c.Y),
               MinZ: positions.Min(c => c.Z), MaxZ: positions.Max(c => c.Z));
}
