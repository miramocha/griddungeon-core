using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Exploration
{
    /// <summary>Neutral exploration spawn/resume target (ADR 025).</summary>
    public readonly struct ExplorationTarget
    {
        public string LocationId { get; }
        public string FloorId { get; }
        public string FloorKey { get; }
        public GridPosition SpawnCell { get; }
        public FacingDirection SpawnFacing { get; }

        public ExplorationTarget(
            string locationId,
            string floorId,
            string floorKey,
            GridPosition spawnCell,
            FacingDirection spawnFacing
        )
        {
            LocationId = locationId;
            FloorId = floorId;
            FloorKey = floorKey;
            SpawnCell = spawnCell;
            SpawnFacing = spawnFacing;
        }
    }
}
