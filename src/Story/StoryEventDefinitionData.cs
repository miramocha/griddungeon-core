using System;

namespace GridDungeon.Core.Story
{
    [Serializable]
    public sealed class StoryEventDefinitionData
    {
        public string StoryEventId = string.Empty;
        public bool PlayOnce = true;
        public StoryEventStepData[] Steps = Array.Empty<StoryEventStepData>();
        public StoryEventEffectData[] CompletionEffects = Array.Empty<StoryEventEffectData>();
    }
}
