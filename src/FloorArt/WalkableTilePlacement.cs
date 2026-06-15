namespace GridDungeon.Core.FloorArt
{
    public readonly struct WalkableTilePlacement
    {
        public WalkableTilePlacement(
            GridPosition cell,
            FloorArtWalkableTileKind kind,
            float rotationYDegrees
        )
        {
            Cell = cell;
            Kind = kind;
            RotationYDegrees = rotationYDegrees;
        }

        public GridPosition Cell { get; }
        public FloorArtWalkableTileKind Kind { get; }
        public float RotationYDegrees { get; }
    }
}
