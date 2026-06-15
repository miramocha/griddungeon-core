using System.Collections.Generic;
using System.Linq;
using GridDungeon.Core.Content;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class TurnQueueBuilder
    {
        const float k_SpeedUpMultiplier = 1.2f;
        const float k_SpeedDownMultiplier = 0.8f;

        public static List<Combatant> Build(IEnumerable<Combatant> combatants) =>
            Build(combatants, statuses: null);

        public static List<Combatant> Build(
            IEnumerable<Combatant> combatants,
            IReadOnlyDictionary<string, StatusData>? statuses
        )
        {
            _ = statuses;
            return combatants
                .Where(c => !c.IsDead)
                .OrderByDescending(GetEffectiveAgi)
                .ThenBy(c => c.Id)
                .ToList();
        }

        public static float GetEffectiveAgi(Combatant combatant)
        {
            float agi = combatant.Stats.Agi;
            bool speedUp = HasActiveStatus(combatant, "speed_up");
            bool speedDown = HasActiveStatus(combatant, "speed_down");

            if (speedUp && speedDown)
            {
                return agi;
            }

            if (speedUp)
            {
                return agi * k_SpeedUpMultiplier;
            }

            if (speedDown)
            {
                return agi * k_SpeedDownMultiplier;
            }

            return agi;
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
