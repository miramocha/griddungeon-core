namespace GridDungeon.Core.Content
{
    /// <summary>
    /// MVP1 summon lookup for Core simulators until full ContentDB wiring (#12).
    /// Keep <see cref="SummonData"/> in sync with runtime
    /// <c>SummonDataLookup</c> / <see cref="ContentDatabase"/> assets.
    /// </summary>
    public static class Mvp1SummonCatalog
    {
        public static bool TryGetSummonData(string summonDefinitionId, out SummonData data)
        {
            if (summonDefinitionId == Mvp1LockedSkillIds.ScoutDroneSummonId)
            {
                data = Mvp1CombatContentStubs.ScoutDrone;
                return true;
            }

            data = default;
            return false;
        }
    }
}
