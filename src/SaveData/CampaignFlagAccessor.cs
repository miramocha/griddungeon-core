namespace GridDungeon.Core.SaveData
{
    public static class CampaignFlagAccessor
    {
        public static bool Get(CampaignSaveData campaign, CampaignFlagId id) =>
            id switch
            {
                CampaignFlagId.S1IntroMovementComplete => campaign.S1IntroMovementComplete,
                CampaignFlagId.S1B1FGateBriefingSeen => campaign.S1B1FGateBriefingSeen,
                CampaignFlagId.S1PartyReady => campaign.S1PartyReady,
                CampaignFlagId.S1TutorialDiveStarted => campaign.S1TutorialDiveStarted,
                CampaignFlagId.S1B2FStalkerBriefingSeen => campaign.S1B2FStalkerBriefingSeen,
                CampaignFlagId.S1SynchroUnlocked => campaign.S1SynchroUnlocked,
                CampaignFlagId.S1SynchroProtocolTutorialDone =>
                    campaign.S1SynchroProtocolTutorialDone,
                CampaignFlagId.S1FirstFoeTutorialComplete => campaign.S1FirstFoeTutorialComplete,
                CampaignFlagId.S1StratumCleared => campaign.S1StratumCleared,
                _ => throw new System.ArgumentOutOfRangeException(nameof(id), id, null),
            };

        public static void Set(CampaignSaveData campaign, CampaignFlagId id, bool value)
        {
            switch (id)
            {
                case CampaignFlagId.S1IntroMovementComplete:
                    campaign.S1IntroMovementComplete = value;
                    break;
                case CampaignFlagId.S1B1FGateBriefingSeen:
                    campaign.S1B1FGateBriefingSeen = value;
                    break;
                case CampaignFlagId.S1PartyReady:
                    campaign.S1PartyReady = value;
                    break;
                case CampaignFlagId.S1TutorialDiveStarted:
                    campaign.S1TutorialDiveStarted = value;
                    break;
                case CampaignFlagId.S1B2FStalkerBriefingSeen:
                    campaign.S1B2FStalkerBriefingSeen = value;
                    break;
                case CampaignFlagId.S1SynchroUnlocked:
                    campaign.S1SynchroUnlocked = value;
                    break;
                case CampaignFlagId.S1SynchroProtocolTutorialDone:
                    campaign.S1SynchroProtocolTutorialDone = value;
                    break;
                case CampaignFlagId.S1FirstFoeTutorialComplete:
                    campaign.S1FirstFoeTutorialComplete = value;
                    break;
                case CampaignFlagId.S1StratumCleared:
                    campaign.S1StratumCleared = value;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }

        public static string GetActLabel(CampaignSaveData campaign) =>
            !campaign.S1IntroMovementComplete ? "Act1-movement"
            : !campaign.S1PartyReady ? "Act2-hub"
            : !campaign.S1TutorialDiveStarted ? "Act2-party-ready"
            : "Act3-dive";
    }
}
