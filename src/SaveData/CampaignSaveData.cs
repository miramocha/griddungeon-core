using System;

namespace GridDungeon.Core.SaveData
{
    [Serializable]
    public sealed class CampaignSaveData
    {
        public bool S1IntroMovementComplete;
        public bool S1B1FGateBriefingSeen;

        /// <summary>Pre-gate-rename JSON key only; merged into <see cref="S1B1FGateBriefingSeen"/> on load.</summary>
        public bool S1B1FMouthBriefingSeen;

        public bool S1PartyReady;
        public bool S1TutorialDiveStarted;
        public bool S1B2FStalkerBriefingSeen;
        public bool S1SynchroUnlocked;
        public bool S1SynchroProtocolTutorialDone;
        public bool S1FirstFoeTutorialComplete;
        public bool S1StratumCleared;

        /// <summary>JsonUtility keeps the legacy mouth briefing key in older saves.</summary>
        public static void NormalizeLegacyJsonFields(CampaignSaveData campaign)
        {
            if (campaign.S1B1FMouthBriefingSeen && !campaign.S1B1FGateBriefingSeen)
            {
                campaign.S1B1FGateBriefingSeen = true;
            }
        }
    }
}
