namespace GridDungeon.Core.FloorArt
{
    /// <summary>
    /// Classifies walkable cells from a 4-bit mask of cardinal walkable neighbors (N=1, E=2, S=4, W=8).
    /// </summary>
    public static class FloorArtWalkableTileClassifier
    {
        public const int North = 1;
        public const int East = 2;
        public const int South = 4;
        public const int West = 8;

        public static WalkableTilePlacement Classify(GridPosition cell, int walkableNeighborMask)
        {
            if (walkableNeighborMask == (North | South))
            {
                return new WalkableTilePlacement(
                    cell,
                    FloorArtWalkableTileKind.HallwayStraight,
                    0f
                );
            }

            if (walkableNeighborMask == (East | West))
            {
                return new WalkableTilePlacement(
                    cell,
                    FloorArtWalkableTileKind.HallwayStraight,
                    90f
                );
            }

            if (walkableNeighborMask == (North | East))
            {
                return new WalkableTilePlacement(cell, FloorArtWalkableTileKind.HallwayCorner, 0f);
            }

            if (walkableNeighborMask == (South | East))
            {
                return new WalkableTilePlacement(cell, FloorArtWalkableTileKind.HallwayCorner, 90f);
            }

            if (walkableNeighborMask == (South | West))
            {
                return new WalkableTilePlacement(
                    cell,
                    FloorArtWalkableTileKind.HallwayCorner,
                    180f
                );
            }

            if (walkableNeighborMask == (North | West))
            {
                return new WalkableTilePlacement(
                    cell,
                    FloorArtWalkableTileKind.HallwayCorner,
                    270f
                );
            }

            return new WalkableTilePlacement(cell, FloorArtWalkableTileKind.FloorDefault, 0f);
        }
    }
}
