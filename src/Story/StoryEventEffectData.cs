using System;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Story
{
    [Serializable]
    public struct StoryEventEffectData
    {
        public StoryEventEffectKind Kind;
        public CampaignFlagId FlagId;
        public bool FlagValue;
        public float SynchroPercent;
        public string EncounterGroupId;
        public bool NoFlee;
        public string ProtocolSkillId;
    }
}
