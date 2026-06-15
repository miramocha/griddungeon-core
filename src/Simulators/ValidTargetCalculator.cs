using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>Shared valid-target filter for player picks and AI defaults.</summary>
    public static class ValidTargetCalculator
    {
        public static IReadOnlyList<Combatant> GetValidTargets(
            BattleState state,
            TargetingRule rule,
            bool actorTargetsEnemies,
            Combatant? actor = null,
            string? skillId = null
        )
        {
            return rule.Kind switch
            {
                TargetKind.SingleEnemy when actorTargetsEnemies => FilterEnemyTargets(
                    state.EnemySlots,
                    rule
                ),
                TargetKind.SingleEnemy => FilterPartyTargets(state, rule),
                TargetKind.SingleAlly => FilterSingleAllyTargets(state, actor, skillId),
                TargetKind.AllAllies => CollectLivingAllies(state),
                TargetKind.AllEnemies when actorTargetsEnemies => CollectLivingEnemies(
                    state.EnemySlots
                ),
                TargetKind.AuxFront or TargetKind.AuxBack => System.Array.Empty<Combatant>(),
                _ => System.Array.Empty<Combatant>(),
            };
        }

        static IReadOnlyList<Combatant> FilterSingleAllyTargets(
            BattleState state,
            Combatant? actor,
            string? skillId
        )
        {
            if (skillId == Mvp1LockedSkillIds.MedicRevive)
            {
                return CollectDownedAllies(state);
            }

            IReadOnlyList<Combatant> allies = CollectLivingAllies(state);
            if (skillId != Mvp1LockedSkillIds.VanguardProtect || actor == null)
            {
                return allies;
            }

            var sameRow = new List<Combatant>();
            foreach (Combatant ally in allies)
            {
                if (ally.Row == actor.Row)
                {
                    sameRow.Add(ally);
                }
            }

            return sameRow;
        }

        static List<Combatant> CollectDownedAllies(BattleState state)
        {
            var result = new List<Combatant>();
            foreach (Combatant? c in state.CoreSlots)
            {
                if (c != null && c.IsDowned)
                {
                    result.Add(c);
                }
            }

            foreach (Combatant? c in state.AuxSlots)
            {
                if (c != null && c.IsDowned)
                {
                    result.Add(c);
                }
            }

            return result;
        }

        static IReadOnlyList<Combatant> FilterPartyTargets(BattleState state, TargetingRule rule)
        {
            if (rule.Pierce)
            {
                return CollectLivingAllies(state);
            }

            List<Combatant> front = CollectLivingPartyInRow(state, FormationRow.Front);
            if (front.Count > 0)
            {
                return front;
            }

            if (!rule.CanTargetBack)
            {
                return System.Array.Empty<Combatant>();
            }

            return CollectLivingPartyInRow(state, FormationRow.Back);
        }

        static List<Combatant> CollectLivingPartyInRow(BattleState state, FormationRow row)
        {
            var result = new List<Combatant>();
            foreach (Combatant? c in state.CoreSlots)
            {
                if (c != null && c.Row == row && !c.IsDead)
                {
                    result.Add(c);
                }
            }

            int auxIndex = row == FormationRow.Front ? 0 : 1;
            if (auxIndex < state.AuxSlots.Length)
            {
                Combatant? aux = state.AuxSlots[auxIndex];
                if (aux != null && !aux.IsDead)
                {
                    result.Add(aux);
                }
            }

            return result;
        }

        static IReadOnlyList<Combatant> FilterEnemyTargets(
            Combatant[] enemySlots,
            TargetingRule rule
        )
        {
            if (rule.Pierce)
            {
                return CollectLivingEnemies(enemySlots);
            }

            List<Combatant> front = CollectLivingInRow(enemySlots, FormationRow.Front);
            if (front.Count > 0)
            {
                return front;
            }

            if (!rule.CanTargetBack)
            {
                return System.Array.Empty<Combatant>();
            }

            return CollectLivingInRow(enemySlots, FormationRow.Back);
        }

        static List<Combatant> CollectLivingEnemies(Combatant[] slots)
        {
            var result = new List<Combatant>();
            foreach (Combatant c in slots)
            {
                if (c != null && !c.IsDead)
                {
                    result.Add(c);
                }
            }

            return result;
        }

        static List<Combatant> CollectLivingInRow(Combatant[] slots, FormationRow row)
        {
            var result = new List<Combatant>();
            foreach (Combatant c in slots)
            {
                if (c != null && c.Row == row && !c.IsDead)
                {
                    result.Add(c);
                }
            }

            return result;
        }

        static List<Combatant> CollectLivingAllies(BattleState state)
        {
            var result = new List<Combatant>();
            foreach (Combatant? c in state.CoreSlots)
            {
                if (c != null && !c.IsDead)
                {
                    result.Add(c);
                }
            }

            foreach (Combatant? c in state.AuxSlots)
            {
                if (c != null && !c.IsDead)
                {
                    result.Add(c);
                }
            }

            return result;
        }
    }
}
