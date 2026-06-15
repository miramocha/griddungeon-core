using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Hub
{
    public static class GuildPartyRules
    {
        public const int RequiredCoreSlots = 6;

        public static bool IsPartyFormationReady(CharacterSaveData[] partySlots)
        {
            if (partySlots == null || partySlots.Length < RequiredCoreSlots)
            {
                return false;
            }

            for (int i = 0; i < RequiredCoreSlots; i++)
            {
                CharacterSaveData slot = partySlots[i];
                if (slot == null || string.IsNullOrEmpty(slot.ClassId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
