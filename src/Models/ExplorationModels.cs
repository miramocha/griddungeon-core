using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Models
{
    public sealed class FoeInstance
    {
        public string FoeId { get; set; } = string.Empty;
        public string EncounterGroupId { get; set; } = string.Empty;
        public GridPosition Cell { get; set; }
        public int PatrolPathIndex { get; set; }
        public int StepsPerMove { get; set; }
        public PatrolWaypoint[]? PatrolPath { get; set; }
        public bool NoFlee { get; set; }
        public bool TutorialFirstFoe { get; set; }
        public bool IsAlive { get; set; } = true;
        public int Tier { get; set; }
    }

    public sealed class FeatureState
    {
        public FeatureType Type { get; set; }
        public bool IsInteracted { get; set; }
    }

    public sealed class FloorMapState
    {
        public string FloorKey { get; set; } = string.Empty;
        public bool[,]? Visited { get; set; }
        public WallMask[,]? Walls { get; set; }
        public Dictionary<GridPosition, FeatureState> Features { get; } = new();
        public Dictionary<GridPosition, string> FoeIcons { get; } = new();
    }
}
