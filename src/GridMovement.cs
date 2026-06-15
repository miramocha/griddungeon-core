using GridDungeon.Core.Enums;

namespace GridDungeon.Core
{
    public static class GridMovement
    {
        public static GridPosition Offset(
            GridPosition cell,
            FacingDirection facing,
            int forward,
            int strafe
        )
        {
            int dx = 0;
            int dy = 0;
            switch (facing)
            {
                case FacingDirection.North:
                    dx = strafe;
                    dy = forward;
                    break;
                case FacingDirection.South:
                    dx = -strafe;
                    dy = -forward;
                    break;
                case FacingDirection.East:
                    dx = forward;
                    dy = -strafe;
                    break;
                case FacingDirection.West:
                    dx = -forward;
                    dy = strafe;
                    break;
            }

            return new GridPosition(cell.X + dx, cell.Y + dy);
        }

        /// <summary>Grid delta for one step attempt (forward/strafe relative to <paramref name="facing"/>).</summary>
        public static (int dx, int dy) GetDisplacementDelta(
            FacingDirection facing,
            int forward,
            int strafe
        )
        {
            var origin = new GridPosition(0, 0);
            GridPosition next = Offset(origin, facing, forward, strafe);
            return (next.X - origin.X, next.Y - origin.Y);
        }

        /// <summary>Cardinal facing of a grid delta (+Y = North). Non-unit deltas return <paramref name="fallback"/>.</summary>
        public static FacingDirection DeltaToFacing(int dx, int dy, FacingDirection fallback) =>
            (dx, dy) switch
            {
                (0, 1) => FacingDirection.North,
                (0, -1) => FacingDirection.South,
                (1, 0) => FacingDirection.East,
                (-1, 0) => FacingDirection.West,
                _ => fallback,
            };
    }
}
