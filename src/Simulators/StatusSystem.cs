using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class StatusSystem
    {
        public static void Apply(Combatant target, StatusInstance instance, StatusData def)
        {
            _ = def;
            target.Statuses.Add(instance);
        }

        public static void Refresh(Combatant target, string statusId, int newDuration)
        {
            foreach (StatusInstance s in target.Statuses)
            {
                if (s.DefinitionId == statusId)
                {
                    s.TurnsRemaining = newDuration;
                }
            }
        }

        public static void Tick(Combatant target, IReadOnlyDictionary<string, StatusData> defs)
        {
            _ = defs;
            for (int i = target.Statuses.Count - 1; i >= 0; i--)
            {
                target.Statuses[i].TurnsRemaining--;
                if (target.Statuses[i].TurnsRemaining <= 0)
                {
                    target.Statuses.RemoveAt(i);
                }
            }
        }

        /// <summary>Removes active statuses whose definition matches <paramref name="category"/>.</summary>
        public static int CleanseCategory(
            Combatant target,
            StatusCategory category,
            IReadOnlyDictionary<string, StatusData> defs
        )
        {
            return target.Statuses.RemoveAll(s =>
                defs.TryGetValue(s.DefinitionId, out StatusData def) && def.Category == category
            );
        }

        /// <summary>MVP1 medic purify — Control + DoT per mvp1-class-skills.md.</summary>
        public static int CleanseControlAndDoT(
            Combatant target,
            IReadOnlyDictionary<string, StatusData> defs
        )
        {
            return target.Statuses.RemoveAll(s =>
                defs.TryGetValue(s.DefinitionId, out StatusData def)
                && (def.Category == StatusCategory.Control || def.Category == StatusCategory.DoT)
            );
        }

        public static bool IsBlocked(Combatant actor, SkillData skill)
        {
            if (skill.BodyPart == BodyPart.None)
            {
                return false;
            }

            foreach (StatusInstance s in actor.Statuses)
            {
                if (s.DefinitionId.Contains("bind", System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
