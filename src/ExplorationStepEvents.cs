namespace GridDungeon.Core
{
    /// <summary>
    /// Which exploration events fire for a displacement attempt (ADR 001 / dungeon-navigation).
    /// </summary>
    public readonly struct DisplacementStepOutcome
    {
        public bool EnteredNewCell { get; }
        public bool EmitPartyEnteredCell => EnteredNewCell;
        public bool EmitPartyStep => EnteredNewCell;
        public bool EmitBumpWall => !EnteredNewCell;

        public DisplacementStepOutcome(bool enteredNewCell) => EnteredNewCell = enteredNewCell;

        public static DisplacementStepOutcome FromWalkable(bool walkable) =>
            new(enteredNewCell: walkable);
    }

    public static class ExplorationStepEvents
    {
        public static bool EmitPartyStepOnTurn() => false;
    }
}
