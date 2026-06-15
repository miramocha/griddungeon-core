using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Simulators
{
    public delegate bool FloorCollisionQuery(GridPosition cell);

    public static class RetreatCellCalculator
    {
        public static GridPosition GetRetreatCell(GridPosition partyCell, FacingDirection facing) =>
            GridMovement.Offset(partyCell, facing, forward: -1, strafe: 0);

        public static bool IsRetreatCellWalkable(
            GridPosition retreatCell,
            FloorCollisionQuery collision
        ) => collision(retreatCell);
    }
}
