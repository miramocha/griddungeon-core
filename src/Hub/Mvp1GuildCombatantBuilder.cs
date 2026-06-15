using System;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Inventory;
using GridDungeon.Core.Models;
using GridDungeon.Core.Progression;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Hub
{
    public static class Mvp1GuildCombatantBuilder
    {
        public static Combatant FromSave(
            CharacterSaveData save,
            int slotIndex,
            IEquipmentStatSource? statSource = null
        )
        {
            int level = CharacterProgression.ResolveLevel(save);

            EquipmentLoadout loadout = save.Equipment?.CloneLoadout() ?? new EquipmentLoadout();
            CombatantStats combatStats = CombatantEquipmentBuild.BuildEffectiveStats(
                save.ClassId,
                level,
                loadout,
                statSource
            );
            FormationRow row = Mvp1GuildRosterFactory.GetPreferredRow(save.ClassId);
            (int hp, int mp) vitals = CharacterSaveVitals.Resolve(save, combatStats);

            var combatant = new Combatant
            {
                Id = save.CharacterId,
                DefinitionId = save.ClassId,
                DisplayName = Mvp1GuildRosterFactory.ResolveMemberDisplayName(
                    save.Name,
                    save.ClassId
                ),
                Kind = CombatantKind.Core,
                Row = row,
                SlotIndex = slotIndex,
                Level = level,
                Experience = Math.Max(0, save.Experience),
                Stats = combatStats,
                CurrentHp = vitals.hp,
                CurrentMp = vitals.mp,
                Equipment = loadout,
            };
            Mvp1GuildRosterFactory.ApplyAllocatedSkillsToCombatant(combatant, save);
            return combatant;
        }
    }
}
