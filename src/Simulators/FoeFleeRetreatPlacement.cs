using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>Post-combat grid placement after flee — FOE contact only (ADR 011).</summary>
    public static class FoeFleeRetreatPlacement
    {
        public static bool TryResolvePostFleeCell(
            BattleResult result,
            CombatEntryContext entry,
            GridPosition partyCell,
            FloorCollisionQuery isWalkable,
            out GridPosition placementCell
        )
        {
            placementCell = default;
            if (!entry.ShouldMovePartyToRetreatCell(result))
            {
                return false;
            }

            GridPosition retreatCell = RetreatCellCalculator.GetRetreatCell(
                partyCell,
                entry.PartyFacing
            );
            if (!RetreatCellCalculator.IsRetreatCellWalkable(retreatCell, isWalkable))
            {
                return false;
            }

            placementCell = retreatCell;
            return true;
        }
    }
}
