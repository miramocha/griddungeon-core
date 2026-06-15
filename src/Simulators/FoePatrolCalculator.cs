using GridDungeon.Core.Content;

namespace GridDungeon.Core.Simulators
{
    public static class FoePatrolCalculator
    {
        public static bool ShouldAdvanceThisPartyStep(int partyStepCount, int stepsPerMove) =>
            stepsPerMove > 0 && partyStepCount > 0 && partyStepCount % stepsPerMove == 0;

        public static bool TryAdvancePatrol(
            int patrolPathIndex,
            GridPosition currentCell,
            PatrolWaypoint[]? patrolPath,
            out int nextPatrolPathIndex,
            out GridPosition nextCell
        )
        {
            nextPatrolPathIndex = patrolPathIndex;
            nextCell = currentCell;

            if (patrolPath == null || patrolPath.Length <= 1)
            {
                return false;
            }

            nextPatrolPathIndex = (patrolPathIndex + 1) % patrolPath.Length;
            nextCell = patrolPath[nextPatrolPathIndex].ToGrid();
            return nextCell != currentCell;
        }
    }
}
