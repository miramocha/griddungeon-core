using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core
{
    public interface IReadOnlyFloorMapState
    {
        bool IsVisited(GridPosition cell);
        WallMask GetWalls(GridPosition cell);
        bool TryGetFeature(GridPosition cell, out FeatureState feature);
        bool TryGetFoeIcon(GridPosition cell, out string foeId);
    }
}
