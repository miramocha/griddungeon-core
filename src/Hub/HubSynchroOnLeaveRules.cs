using GridDungeon.Core.SaveData;
using GridDungeon.Core.Simulators;

namespace GridDungeon.Core.Hub
{
    public static class HubSynchroOnLeaveRules
    {
        /// <summary>S1 hub exit: full Synchro bar for stratum entry (#251).</summary>
        public static float ResolveSynchroBarForStratumEntry(CampaignSaveData campaign) =>
            ProtocolResolver.FullBar;
    }
}
