using System;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>
    /// Flee success rate (RPG Maker–style agility comparison). Resolves on the actor's AGI turn.
    /// </summary>
    public static class FleeCalculator
    {
        public const float MinSuccessPercent = 5f;
        public const float MaxSuccessPercent = 95f;

        /// <summary>
        /// Success chance in [MinSuccessPercent, MaxSuccessPercent].
        /// Formula: (1.5 − enemyAvgAgi / partyAvgAgi) × 100 when both sides have living units.
        /// </summary>
        public static float ComputeSuccessPercent(BattleState state)
        {
            float partyAgi = AveragePartyLivingAgi(state);
            float enemyAgi = AverageLivingAgi(state.EnemySlots);
            if (partyAgi <= 0f)
            {
                partyAgi = 1f;
            }

            if (enemyAgi <= 0f)
            {
                enemyAgi = 1f;
            }

            float raw = (1.5f - enemyAgi / partyAgi) * 100f;
            return Math.Clamp(raw, MinSuccessPercent, MaxSuccessPercent);
        }

        public static bool RollSuccess(BattleState state, Func<float> rollPercent)
        {
            if (!state.FleeEnabled)
            {
                return false;
            }

            float roll = rollPercent();
            return roll <= ComputeSuccessPercent(state);
        }

        static float AveragePartyLivingAgi(BattleState state)
        {
            float sum = 0f;
            int count = 0;
            AccumulateLivingAgi(state.CoreSlots, ref sum, ref count);
            AccumulateLivingAgi(state.AuxSlots, ref sum, ref count);
            return count == 0 ? 0f : sum / count;
        }

        static float AverageLivingAgi(Combatant?[] slots)
        {
            float sum = 0f;
            int count = 0;
            AccumulateLivingAgi(slots, ref sum, ref count);
            return count == 0 ? 0f : sum / count;
        }

        static void AccumulateLivingAgi(Combatant?[] slots, ref float sum, ref int count)
        {
            foreach (Combatant? c in slots)
            {
                if (c == null || c.IsDead)
                {
                    continue;
                }

                sum += c.Stats.Agi;
                count++;
            }
        }
    }
}
