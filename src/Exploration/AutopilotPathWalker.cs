using System.Collections.Generic;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Exploration
{
    /// <summary>Next grid step for autopilot along a cardinal path.</summary>
    public static class AutopilotPathWalker
    {
        public static AutopilotWalkerAction GetNextAction(
            IReadOnlyList<GridPosition> path,
            int pathIndex,
            GridPosition currentCell,
            FacingDirection facing
        )
        {
            if (path == null || path.Count == 0 || pathIndex >= path.Count)
            {
                return AutopilotWalkerAction.Done;
            }

            GridPosition next = path[pathIndex];
            if (currentCell == next)
            {
                return AutopilotWalkerAction.Done;
            }

            int dx = next.X - currentCell.X;
            int dy = next.Y - currentCell.Y;
            if (System.Math.Abs(dx) + System.Math.Abs(dy) != 1)
            {
                return AutopilotWalkerAction.Done;
            }

            FacingDirection targetFacing = GridMovement.DeltaToFacing(dx, dy, facing);
            if (facing == targetFacing)
            {
                return AutopilotWalkerAction.StepForward;
            }

            return TurnRightSteps(facing, targetFacing) <= TurnLeftSteps(facing, targetFacing)
                ? AutopilotWalkerAction.TurnRight
                : AutopilotWalkerAction.TurnLeft;
        }

        static int TurnRightSteps(FacingDirection from, FacingDirection to) =>
            ((int)to - (int)from + 4) % 4;

        static int TurnLeftSteps(FacingDirection from, FacingDirection to) =>
            ((int)from - (int)to + 4) % 4;
    }
}
