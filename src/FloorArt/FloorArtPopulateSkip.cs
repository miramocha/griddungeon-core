using GridDungeon.Core;

namespace GridDungeon.Core.FloorArt
{
    /// <summary>
    /// Cells excluded from auto walkable populate (gather nodes, future key markers).
    /// </summary>
    public static class FloorArtPopulateSkip
    {
        public static bool ShouldSkipWallPopulate(string floorKey, GridPosition cell) => false;

        public static bool ShouldSkipWalkablePopulate(
            string floorKey,
            GridPosition cell,
            bool hasGatherNode
        ) => hasGatherNode;
    }
}
