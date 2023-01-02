using System.Diagnostics;

namespace Aoc2022_Day22;

internal class FoldedCubeMovementStrategy : IMovementStrategy
{
    private readonly HashSet<CubeFace> _faces;
    private readonly HashSet<CubeVertex> _vertices;

    public FoldedCubeMovementStrategy(string[] data)
    {
        // Detect the positioning of the faces in the map.
        var faces = CubeFace.DetectAll(data).ToHashSet();
        var faceSize = faces.First().Size;

        // Connect the corners of the faces to form vertices (three corners meet to form a vertex).
        var vertices = faces.SelectMany(face => face.GetCorners().Select(c => (c.Row, c.Column, Face: face)))
                            .GroupBy(c => ((int)Math.Round(c.Row / (double)faceSize), (int)Math.Round(c.Column / (double)faceSize)))
                            .Select(g => new CubeVertex(g))
                            .ToHashSet();
        while (vertices.Count > 8)
        {
            var resolvedVertices = vertices.Where(x => x.Corners.Count == 3);
            foreach (var resolvedVertex in resolvedVertices)
            {
                var vertexFaces = resolvedVertex.Corners.Select(x => x.Face).ToArray();
                var adjacentCorners = resolvedVertex.Corners.SelectMany(c => c.Face.GetAdjacentCorners((c.Row, c.Column)))
                                                    .ToHashSet();
                var adjacentVertices = vertices.Where(v => v.Corners.Any(c => adjacentCorners.Contains((c.Row, c.Column))));
                var merge = adjacentVertices.Where(a => vertexFaces.Count(f => a.Corners.Any(c => c.Face == f)) == 1)
                                            .ToArray();
                if (merge.Length != 2)
                    continue;
                merge[0].Corners.UnionWith(merge[1].Corners);
                vertices.Remove(merge[1]);
            }
        }

        _faces = faces;
        _vertices = vertices;
    }

    public MapPosition MoveForward(MapPosition position)
    {
        var face = _faces.Single(f => f.Contains(position));
        if (!face.IsLeaving(position))
            return position.Facing switch
                   {
                       Direction.Right => position with { Column = position.Column + 1 },
                       Direction.Down  => position with { Row = position.Row + 1 },
                       Direction.Left  => position with { Column = position.Column - 1 },
                       Direction.Up    => position with { Row = position.Row - 1 },
                       _               => throw new ArgumentOutOfRangeException(nameof(position), $"Unknown direction: {position.Facing}")
                   };
        var leavingEdgeCorners = face.GetLeavingEdgeCorners(position.Facing);
        var leavingEdgeVertices = leavingEdgeCorners.Select(c => _vertices.Single(v => v.Corners.Contains((c.Row, c.Column, face))))
                                                    .ToArray();
        // The edge vertices shares two common faces, the leaving and entering faces.
        var enteringFace = leavingEdgeVertices.SelectMany(v => v.Corners.Where(z => z.Face != face))
                                              .GroupBy(c => c.Face)
                                              .Where(g => g.Count() == 2)
                                              .Select(g => g.Key)
                                              .Single();
        var enteringEdgeCorners = leavingEdgeVertices.Select(v => v.Corners.Single(c => c.Face == enteringFace))
                                                     .ToArray();
        var distanceAlongEdge = Math.Abs(position.Column - leavingEdgeCorners[0].Column) + Math.Abs(position.Row - leavingEdgeCorners[0].Row);
        return enteringFace.GetEntryPosition(enteringEdgeCorners, distanceAlongEdge);
    }

    private record CubeVertex
    {
        public HashSet<(int Row, int Column, CubeFace Face)> Corners { get; } = new(3);

        public CubeVertex(IEnumerable<(int Row, int Column, CubeFace Face)> corners)
            => Corners = corners.ToHashSet();
    }

    private record CubeFace(Area Area)
    {
        public int Size => Area.MaxColumn - Area.MinColumn + 1;

        public bool Contains(MapPosition position)
            => position.Row >= Area.MinRow && position.Row <= Area.MaxRow &&
               position.Column >= Area.MinColumn && position.Column <= Area.MaxColumn;

        public bool IsLeaving(MapPosition position)
            => position.Facing switch
               {
                   Direction.Right => position.Column == Area.MaxColumn,
                   Direction.Down  => position.Row == Area.MaxRow,
                   Direction.Left  => position.Column == Area.MinColumn,
                   Direction.Up    => position.Row == Area.MinRow,
                   _               => throw new ArgumentOutOfRangeException(nameof(position), $"Unknown direction: {position.Facing}")
               };

        public IEnumerable<(int Row, int Column)> GetCorners()
        {
            yield return (Area.MinRow, Area.MinColumn);
            yield return (Area.MinRow, Area.MaxColumn);
            yield return (Area.MaxRow, Area.MinColumn);
            yield return (Area.MaxRow, Area.MaxColumn);
        }

        public IEnumerable<(int Row, int Column)> GetAdjacentCorners((int Row, int Column) to)
            => GetCorners().Where(c => Math.Abs(c.Row - to.Row) + Math.Abs(c.Column - to.Column) == Area.MaxColumn - Area.MinColumn);

        public (int Row, int Column)[] GetLeavingEdgeCorners(Direction direction)
            => direction switch
               {
                   Direction.Right => new[] { (Area.MinRow, Area.MaxColumn), (Area.MaxRow, Area.MaxColumn) },
                   Direction.Down  => new[] { (Area.MaxRow, Area.MinColumn), (Area.MaxRow, Area.MaxColumn) },
                   Direction.Left  => new[] { (Area.MinRow, Area.MinColumn), (Area.MaxRow, Area.MinColumn) },
                   Direction.Up    => new[] { (Area.MinRow, Area.MinColumn), (Area.MinRow, Area.MaxColumn) },
                   _               => throw new ArgumentOutOfRangeException(nameof(direction), $"Unknown direction: {direction}")
               };

        public MapPosition GetEntryPosition((int Row, int Column, CubeFace Face)[] edgeCorners, int distanceAlongEdge)
        {
            if (edgeCorners.All(c => c.Column == Area.MaxColumn))
            {
                // Entering at the right edge.
                var distanceSign = edgeCorners[0].Row < edgeCorners[1].Row ? 1 : -1;
                return new MapPosition(edgeCorners[0].Row + distanceAlongEdge * distanceSign, Area.MaxColumn, Direction.Left);
            }
            if (edgeCorners.All(c => c.Column == Area.MinColumn))
            {
                // Entering at the left edge.
                var distanceSign = edgeCorners[0].Row < edgeCorners[1].Row ? 1 : -1;
                return new MapPosition(edgeCorners[0].Row + distanceAlongEdge * distanceSign, Area.MinColumn, Direction.Right);
            }
            if (edgeCorners.All(c => c.Row == Area.MaxRow))
            {
                // Entering at the bottom edge.
                var distanceSign = edgeCorners[0].Column < edgeCorners[1].Column ? 1 : -1;
                return new MapPosition(Area.MaxRow, edgeCorners[0].Column + distanceAlongEdge * distanceSign, Direction.Up);
            }
            if (edgeCorners.All(c => c.Row == Area.MinRow))
            {
                // Entering at the top edge.
                var distanceSign = edgeCorners[0].Column < edgeCorners[1].Column ? 1 : -1;
                return new MapPosition(Area.MinRow, edgeCorners[0].Column + distanceAlongEdge * distanceSign, Direction.Down);
            }

            throw new UnreachableException();
        }

        public static CubeFace[] DetectAll(string[] data)
        {
            var faceSize = (int)Math.Sqrt(data.SelectMany(s => s).Count(c => c != ' ') / 6.0);
            var faces = new List<CubeFace>(6);
            for (var row = 0; row < data.Length / faceSize; row++)
            for (var column = 0; column < 4; column++)
            {
                var area = new Area(MinColumn: faceSize * column,
                                    MaxColumn: faceSize * (column + 1) - 1,
                                    MinRow: faceSize * row,
                                    MaxRow: faceSize * (row + 1) - 1);
                var isFace = data[area.MaxRow].Length > area.MaxColumn && data[area.MaxRow][area.MaxColumn] != ' ';
                if (isFace)
                    faces.Add(new CubeFace(area));
            }
            return faces.ToArray();
        }
    }

    private readonly record struct Area(int MinColumn, int MaxColumn, int MinRow, int MaxRow);
}
