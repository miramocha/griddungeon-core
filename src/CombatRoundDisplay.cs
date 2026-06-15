namespace GridDungeon.Core
{
    /// <summary>Player-facing combat round text from <see cref="Models.BattleState.Round"/>.</summary>
    public static class CombatRoundDisplay
    {
        /// <summary>
        /// HUD label for the active round. <paramref name="completedEndOfRoundTicks"/> is
        /// <c>BattleState.Round</c> (0 at battle start; incremented in
        /// <see cref="Simulators.CombatSimulator.SimulateRound"/> after each end-of-round).
        /// </summary>
        public static string FormatHudLabel(int completedEndOfRoundTicks) =>
            $"Round {completedEndOfRoundTicks + 1}";
    }
}
