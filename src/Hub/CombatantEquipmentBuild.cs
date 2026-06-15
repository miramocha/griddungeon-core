using GridDungeon.Core.Inventory;
using GridDungeon.Core.Models;
using GridDungeon.Core.Progression;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Hub
{
    public static class CombatantEquipmentBuild
    {
        public static CombatantStats BuildEffectiveStats(
            string classId,
            int level,
            EquipmentLoadout? equipment,
            IEquipmentStatSource? statSource
        )
        {
            CombatantStats baseStats = CharacterProgression.GetCombatStatsAtLevel(classId, level);
            if (statSource == null || equipment == null)
            {
                return baseStats;
            }

            return EquipmentStatAggregator.Apply(baseStats, equipment, statSource);
        }

        public static void ApplyEffectiveStats(
            Combatant combatant,
            CharacterSaveData save,
            IEquipmentStatSource? statSource
        )
        {
            if (combatant == null || save == null || string.IsNullOrEmpty(save.ClassId))
            {
                return;
            }

            int level =
                combatant.Level > 0 ? combatant.Level : CharacterProgression.ResolveLevel(save);
            EquipmentLoadout loadout = save.Equipment?.CloneLoadout() ?? new EquipmentLoadout();
            combatant.Equipment = loadout;
            CombatantStats effective = BuildEffectiveStats(
                save.ClassId,
                level,
                loadout,
                statSource
            );
            combatant.Stats = effective;
            combatant.CurrentHp = System.Math.Min(combatant.CurrentHp, effective.Hp);
            combatant.CurrentMp = System.Math.Min(combatant.CurrentMp, effective.Mp);
        }
    }
}
