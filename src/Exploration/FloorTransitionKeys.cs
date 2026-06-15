namespace GridDungeon.Core.Exploration
{
    /// <summary>Non-floor <see cref="FloorTransitionCatalog"/> destination keys (ADR 032).</summary>
    public static class FloorTransitionKeys
    {
        public const string Hub = "hub";

        public static bool IsHubDestination(string enterFloorKey) => enterFloorKey == Hub;
    }
}
