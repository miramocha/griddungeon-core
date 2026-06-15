using System;
using GridDungeon.Core;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Story;

namespace GridDungeon.Core.Content
{
    [Serializable]
    public struct FloorExitLink
    {
        public string ExitId;
        public int CellX;
        public int CellY;
        public FloorExitDirection Direction;
        public FloorExitTargetKind TargetKind;
        public string TargetFloorKey;
        public int TargetSpawnCellX;
        public int TargetSpawnCellY;
        public FacingDirection TargetFacing;
        public StoryEventEffectData[] OnUseEffects;

        public GridPosition Cell => new(CellX, CellY);

        public GridPosition TargetSpawnCell => new(TargetSpawnCellX, TargetSpawnCellY);

        public static FloorExitLink Create(
            string exitId,
            GridPosition cell,
            FloorExitDirection direction,
            FloorExitTargetKind targetKind,
            string targetFloorKey,
            GridPosition targetSpawnCell,
            FacingDirection targetFacing = FacingDirection.North,
            StoryEventEffectData[]? onUseEffects = null
        ) =>
            new()
            {
                ExitId = exitId ?? string.Empty,
                CellX = cell.X,
                CellY = cell.Y,
                Direction = direction,
                TargetKind = targetKind,
                TargetFloorKey = targetFloorKey ?? string.Empty,
                TargetSpawnCellX = targetSpawnCell.X,
                TargetSpawnCellY = targetSpawnCell.Y,
                TargetFacing = targetFacing,
                OnUseEffects = onUseEffects ?? Array.Empty<StoryEventEffectData>(),
            };
    }
}
