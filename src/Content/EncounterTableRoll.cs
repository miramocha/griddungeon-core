using System;

namespace GridDungeon.Core.Content
{
    /// <summary>Weighted pick from floor random encounter tables (Core — no Unity RNG).</summary>
    public static class EncounterTableRoll
    {
        public static bool TryPickGroup(EncounterTable table, float roll01, out string groupId)
        {
            groupId = string.Empty;
            EncounterWeight[] entries = table.Entries;
            if (entries == null || entries.Length == 0)
            {
                return false;
            }

            float total = 0f;
            foreach (EncounterWeight entry in entries)
            {
                if (entry.Weight > 0f)
                {
                    total += entry.Weight;
                }
            }

            if (total <= 0f)
            {
                return false;
            }

            float roll = roll01 * total;
            foreach (EncounterWeight entry in entries)
            {
                if (entry.Weight <= 0f)
                {
                    continue;
                }

                roll -= entry.Weight;
                if (roll <= 0f)
                {
                    groupId = entry.GroupId;
                    return true;
                }
            }

            groupId = entries[^1].GroupId;
            return !string.IsNullOrEmpty(groupId);
        }
    }
}
