using System.Collections.Generic;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class BattleOutcomeChecker
    {
        public static BattleResult? Evaluate(BattleState state)
        {
            if (AllEnemiesDefeated(state))
            {
                return BattleResult.Victory;
            }

            if (AllCorePartyDefeated(state))
            {
                return BattleResult.Wipe;
            }

            return null;
        }

        public static bool AllEnemiesDefeated(BattleState state)
        {
            bool anyEnemy = false;
            foreach (Combatant enemy in state.EnemySlots)
            {
                if (enemy == null)
                {
                    continue;
                }

                anyEnemy = true;
                if (!enemy.IsDead)
                {
                    return false;
                }
            }

            return anyEnemy;
        }

        public static bool AllCorePartyDefeated(BattleState state)
        {
            bool anyCore = false;
            foreach (Combatant core in state.CoreSlots)
            {
                if (core == null)
                {
                    continue;
                }

                anyCore = true;
                if (!core.IsDead)
                {
                    return false;
                }
            }

            return anyCore;
        }

        public static IEnumerable<Combatant> AllLivingCombatants(BattleState state)
        {
            foreach (Combatant c in state.CoreSlots)
            {
                if (c != null && !c.IsDead)
                {
                    yield return c;
                }
            }

            foreach (Combatant? aux in state.AuxSlots)
            {
                if (aux != null && !aux.IsDead)
                {
                    yield return aux;
                }
            }

            foreach (Combatant enemy in state.EnemySlots)
            {
                if (enemy != null && !enemy.IsDead)
                {
                    yield return enemy;
                }
            }
        }
    }
}
