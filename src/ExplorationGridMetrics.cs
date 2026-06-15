namespace GridDungeon.Core
{
    /// <summary>
    /// World-space scale for exploration grid cells (MVP1 launch floors; each cell is N world units).
    /// </summary>
    public static class ExplorationGridMetrics
    {
        public const int Mvp1FloorGridCells = 21;

        public const float WorldUnitsPerCell = 10f;

        /// <summary>Floor art scenes authored at 1 unit per logic cell; runtime expands prop spacing to <see cref="WorldUnitsPerCell"/>.</summary>
        public const float LegacyFloorArtCellSize = 1f;

        /// <summary>FPV eye height as fraction of cell size (3 world units at 10 units/cell).</summary>
        public const float EyeHeightPerCell = 0.3f;

        public static float DefaultEyeHeight => WorldUnitsPerCell * EyeHeightPerCell;

        public static float CellCenterOffset => WorldUnitsPerCell * 0.5f;
    }
}
