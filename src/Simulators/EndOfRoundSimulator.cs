using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class EndOfRoundSimulator
    {
        const float k_PoisonMaxHpPercent = 0.05f;

        public static List<string> Execute(
            BattleState state,
            IReadOnlyDictionary<string, StatusData> statusDefs,
            int round
        )
        {
            var log = new List<string>();
            foreach (Combatant combatant in BattleOutcomeChecker.AllLivingCombatants(state))
            {
                ApplyRegenTicks(combatant, statusDefs, log);
                ApplyDoTTicks(combatant, statusDefs, log);
                StatusSystem.Tick(combatant, statusDefs);
            }

            TickSummonDurations(state, log);

            if (log.Count == 0)
            {
                log.Add($"Round {round} end — status durations ticked.");
            }

            return log;
        }

        static void TickSummonDurations(BattleState state, List<string> log)
        {
            for (int i = 0; i < state.AuxSlots.Length; i++)
            {
                Combatant? summon = state.AuxSlots[i];
                if (
                    summon == null
                    || summon.Kind != CombatantKind.Summon
                    || summon.SummonTurnsRemaining <= 0
                )
                {
                    continue;
                }

                summon.SummonTurnsRemaining--;
                if (summon.SummonTurnsRemaining > 0)
                {
                    continue;
                }

                state.AuxSlots[i] = null;
                summon.CurrentHp = 0;
                log.Add($"{summon.Id} dismissed (duration expired).");
            }
        }

        static void ApplyRegenTicks(
            Combatant combatant,
            IReadOnlyDictionary<string, StatusData> statusDefs,
            List<string> log
        )
        {
            foreach (StatusInstance instance in combatant.Statuses)
            {
                if (instance.TurnsRemaining <= 0)
                {
                    continue;
                }

                if (!statusDefs.TryGetValue(instance.DefinitionId, out StatusData def))
                {
                    continue;
                }

                if (def.Category != StatusCategory.StatBuff || def.Magnitude <= 0f)
                {
                    continue;
                }

                if (
                    !instance.DefinitionId.Contains(
                        "regen",
                        System.StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    continue;
                }

                int heal = (int)System.Math.Floor(def.Magnitude);
                if (heal <= 0)
                {
                    continue;
                }

                int before = combatant.CurrentHp;
                combatant.CurrentHp = System.Math.Min(
                    combatant.Stats.Hp,
                    combatant.CurrentHp + heal
                );
                int gained = combatant.CurrentHp - before;
                if (gained > 0)
                {
                    log.Add($"{combatant.Id} regains {gained} HP.");
                }
            }
        }

        static void ApplyDoTTicks(
            Combatant combatant,
            IReadOnlyDictionary<string, StatusData> statusDefs,
            List<string> log
        )
        {
            foreach (StatusInstance instance in combatant.Statuses)
            {
                if (instance.TurnsRemaining <= 0)
                {
                    continue;
                }

                if (!statusDefs.TryGetValue(instance.DefinitionId, out StatusData def))
                {
                    continue;
                }

                if (def.Category != StatusCategory.DoT)
                {
                    continue;
                }

                int damage = instance.DefinitionId.Contains(
                    "poison",
                    System.StringComparison.OrdinalIgnoreCase
                )
                    ? System.Math.Max(
                        1,
                        (int)System.Math.Floor(combatant.Stats.Hp * k_PoisonMaxHpPercent)
                    )
                    : System.Math.Max(1, (int)def.Magnitude);

                combatant.CurrentHp = System.Math.Max(0, combatant.CurrentHp - damage);
                log.Add($"{combatant.Id} takes {damage} {instance.DefinitionId} damage.");
            }
        }
    }
}
