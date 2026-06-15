using GridDungeon.Core;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>
    /// EO-style enemy formation compaction when front-row slots empty ([combat.md] row collapse).
    /// </summary>
    public static class EnemyRowCollapse
    {
        public static bool NeedsCollapse(BattleState state)
        {
            Combatant[] slots = state.EnemySlots;
            for (int col = 0; col < BattleFormation.MaxEnemyFront; col++)
            {
                if (FrontSlotNeedsCollapse(slots, col))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears dead enemies, promotes back-column survivors, compacts each row. Returns true if layout changed.
        /// </summary>
        public static bool Apply(BattleState state)
        {
            Combatant[] slots = state.EnemySlots;
            bool changed = ClearDeadEnemies(slots);

            if (!HasLivingEnemy(slots))
            {
                return changed;
            }

            for (int col = 0; col < BattleFormation.MaxEnemyFront; col++)
            {
                changed |= PromoteColumnIfNeeded(slots, col);
            }

            changed |= CompactRow(slots, 0, BattleFormation.MaxEnemyFront - 1);
            changed |= CompactRow(
                slots,
                BattleFormation.MaxEnemyFront,
                BattleFormation.MaxEnemySlots - 1
            );
            return changed;
        }

        static bool FrontSlotNeedsCollapse(Combatant[] slots, int column)
        {
            int frontIndex = column;
            int backIndex = column + BattleFormation.MaxEnemyFront;
            Combatant? front = slots[frontIndex];
            if (front != null && front.IsDead)
            {
                return true;
            }

            if (front != null)
            {
                return false;
            }

            Combatant? back = slots[backIndex];
            return back != null && !back.IsDead;
        }

        static bool ClearDeadEnemies(Combatant[] slots)
        {
            // Keep defeated refs when the whole side is down so BattleOutcomeChecker can award victory.
            if (!HasLivingEnemy(slots))
            {
                return false;
            }

            bool changed = false;
            for (int i = 0; i < slots.Length; i++)
            {
                Combatant? enemy = slots[i];
                if (enemy == null || !enemy.IsDead)
                {
                    continue;
                }

                slots[i] = null!;
                changed = true;
            }

            return changed;
        }

        static bool HasLivingEnemy(Combatant[] slots)
        {
            foreach (Combatant? enemy in slots)
            {
                if (enemy != null && !enemy.IsDead)
                {
                    return true;
                }
            }

            return false;
        }

        static bool PromoteColumnIfNeeded(Combatant[] slots, int column)
        {
            int frontIndex = column;
            int backIndex = column + BattleFormation.MaxEnemyFront;
            Combatant? front = slots[frontIndex];
            if (front != null && !front.IsDead)
            {
                return false;
            }

            Combatant? back = slots[backIndex];
            if (back == null || back.IsDead)
            {
                if (front != null && front.IsDead && HasLivingEnemy(slots))
                {
                    slots[frontIndex] = null!;
                    return true;
                }

                return false;
            }

            slots[frontIndex] = back;
            slots[backIndex] = null!;
            back.Row = FormationRow.Front;
            back.SlotIndex = frontIndex;
            return true;
        }

        static bool CompactRow(Combatant[] slots, int rowStart, int rowEnd)
        {
            var living = new System.Collections.Generic.List<Combatant>();
            for (int i = rowStart; i <= rowEnd; i++)
            {
                Combatant? enemy = slots[i];
                if (enemy != null)
                {
                    living.Add(enemy);
                }
            }

            FormationRow row =
                rowStart < BattleFormation.MaxEnemyFront ? FormationRow.Front : FormationRow.Back;

            bool changed = false;
            for (int i = 0; i < living.Count; i++)
            {
                int slotIndex = rowStart + i;
                Combatant enemy = living[i];
                if (enemy.SlotIndex != slotIndex || enemy.Row != row || slots[slotIndex] != enemy)
                {
                    changed = true;
                }
            }

            for (int i = rowStart; i <= rowEnd; i++)
            {
                slots[i] = null!;
            }

            for (int i = 0; i < living.Count; i++)
            {
                int slotIndex = rowStart + i;
                Combatant enemy = living[i];
                enemy.SlotIndex = slotIndex;
                enemy.Row = row;
                slots[slotIndex] = enemy;
            }

            return changed;
        }
    }
}
