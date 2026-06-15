using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class CombatStatModifiers
    {
        const float k_OffenseUpMultiplier = 1.25f;
        const float k_OffenseDownMultiplier = 0.75f;
        const float k_DefenseUpMultiplier = 0.75f;
        const float k_DefenseDownMultiplier = 1.25f;

        public static float GetPhysicalOffenseMultiplier(Combatant attacker)
        {
            float mult = 1f;
            if (HasActiveStatus(attacker, "offense_up"))
            {
                mult *= k_OffenseUpMultiplier;
            }

            if (HasActiveStatus(attacker, "offense_down"))
            {
                mult *= k_OffenseDownMultiplier;
            }

            return mult;
        }

        public static float GetPhysicalDefenseMultiplier(Combatant defender)
        {
            float mult = 1f;
            if (HasActiveStatus(defender, "defense_up"))
            {
                mult *= k_DefenseUpMultiplier;
            }

            if (HasActiveStatus(defender, "defense_down"))
            {
                mult *= k_DefenseDownMultiplier;
            }

            return mult;
        }

        static bool HasActiveStatus(Combatant combatant, string statusId)
        {
            foreach (StatusInstance instance in combatant.Statuses)
            {
                if (instance.DefinitionId == statusId && instance.TurnsRemaining > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
