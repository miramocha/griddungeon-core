using GridDungeon.Core.Models;

namespace GridDungeon.Core.Hub
{
    public static class HubHospitalRules
    {
        public static int GetHealCost(Combatant member)
        {
            if (member == null || member.IsDead)
            {
                return 0;
            }

            int missingHp = member.Stats.Hp - member.CurrentHp;
            if (missingHp <= 0)
            {
                return 0;
            }

            return Mvp1HubConstants.HealPartyBaseCost
                + missingHp * Mvp1HubConstants.HealPartyPerMissingHp;
        }

        public static int GetHealCost(Combatant[] coreSlots)
        {
            int missingHp = 0;
            foreach (Combatant? member in coreSlots)
            {
                if (member == null || member.IsDead)
                {
                    continue;
                }

                missingHp += member.Stats.Hp - member.CurrentHp;
            }

            if (missingHp <= 0)
            {
                return 0;
            }

            return Mvp1HubConstants.HealPartyBaseCost
                + missingHp * Mvp1HubConstants.HealPartyPerMissingHp;
        }

        public static int GetReviveCost(Combatant character)
        {
            if (character == null || !character.IsDead)
            {
                return 0;
            }

            return Mvp1HubConstants.ReviveBaseCost;
        }
    }
}
