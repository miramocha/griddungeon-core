using System;

namespace GridDungeon.Core.Story
{
    [Serializable]
    public struct StoryEventStepData
    {
        public StoryEventStepKind Kind;
        public string SpeakerId;
        public string TextKey;
        public string TextEn;
        public float WaitSeconds;
        public StoryEventEffectData[] Effects;
    }
}
