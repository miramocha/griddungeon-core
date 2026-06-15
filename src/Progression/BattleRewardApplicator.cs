using System;
using System.Collections.Generic;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Hub;
using GridDungeon.Core.Inventory;
using GridDungeon.Core.Models;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Progression
{
    /// <summary>Applies a resolved battle reward to party saves, inventory, and runtime cores.</summary>
    public static class BattleRewardApplicator
    {
        public static BattleRewardApplySummary Apply(
            BattleRewardResult reward,
            CharacterSaveData[] partySaves,
            PartyInventory inventory,
            Combatant[] coreSlots,
            IInventoryContentRules inventoryRules
        )
        {
            if (reward.Skipped || !reward.HasRewards)
            {
                return BattleRewardApplySummary.Empty;
            }

            HashSet<string> coreMemberIds = CollectCoreMemberIds(coreSlots);
            var levelUps = new List<CharacterLevelUpNotice>();
            IEquipmentStatSource? equipmentStatSource = inventoryRules as IEquipmentStatSource;
            if (reward.TotalXp > 0 && partySaves != null)
            {
                foreach (CharacterSaveData member in partySaves)
                {
                    if (
                        member == null
                        || string.IsNullOrEmpty(member.ClassId)
                        || !coreMemberIds.Contains(member.CharacterId)
                    )
                    {
                        continue;
                    }

                    ProgressionApplyResult applied = CharacterProgression.AddExperience(
                        member,
                        reward.TotalXp,
                        member.ClassId,
                        equipmentStatSource
                    );
                    if (applied.LevelsGained > 0)
                    {
                        string displayName = Mvp1GuildRosterFactory.ResolveMemberDisplayName(
                            member.Name,
                            member.ClassId
                        );
                        levelUps.Add(new CharacterLevelUpNotice(displayName, member.Level));
                    }
                }
            }

            IReadOnlyList<BattleLootDrop> appliedLoot = ApplyLoot(
                reward.Loot,
                inventory,
                inventoryRules
            );

            return new BattleRewardApplySummary(reward.TotalXp, levelUps, appliedLoot);
        }

        static HashSet<string> CollectCoreMemberIds(Combatant[] coreSlots)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (coreSlots == null)
            {
                return ids;
            }

            foreach (Combatant? combatant in coreSlots)
            {
                if (
                    combatant != null
                    && combatant.Kind == CombatantKind.Core
                    && !string.IsNullOrEmpty(combatant.Id)
                )
                {
                    ids.Add(combatant.Id);
                }
            }

            return ids;
        }

        static List<BattleLootDrop> ApplyLoot(
            IReadOnlyList<BattleLootDrop> loot,
            PartyInventory inventory,
            IInventoryContentRules rules
        )
        {
            var applied = new List<BattleLootDrop>();
            if (loot == null || loot.Count == 0)
            {
                return applied;
            }

            foreach (BattleLootDrop drop in loot)
            {
                if (string.IsNullOrEmpty(drop.ItemId) || drop.Quantity <= 0)
                {
                    continue;
                }

                if (
                    !InventoryRules.TryAddConsumable(
                        inventory,
                        drop.ItemId,
                        drop.Quantity,
                        rules,
                        out int quantityNotAdded
                    )
                )
                {
                    int addedQty = drop.Quantity - quantityNotAdded;
                    if (addedQty > 0)
                    {
                        applied.Add(new BattleLootDrop(drop.ItemId, addedQty));
                    }

                    continue;
                }

                applied.Add(new BattleLootDrop(drop.ItemId, drop.Quantity));
            }

            return applied;
        }

        static CharacterSaveData? FindSave(CharacterSaveData[] partySaves, string characterId)
        {
            foreach (CharacterSaveData save in partySaves)
            {
                if (save != null && save.CharacterId == characterId)
                {
                    return save;
                }
            }

            return null;
        }
    }

    public sealed class BattleRewardApplySummary
    {
        /// <summary>Encounter XP granted to each eligible core member (not split).</summary>
        public int XpGrantedPerMember { get; }
        public IReadOnlyList<CharacterLevelUpNotice> LevelUps { get; }
        public IReadOnlyList<BattleLootDrop> AppliedLoot { get; }

        public int TotalLevelUps => LevelUps.Count;

        public int LootQuantityAdded
        {
            get
            {
                int total = 0;
                foreach (BattleLootDrop drop in AppliedLoot)
                {
                    total += drop.Quantity;
                }

                return total;
            }
        }

        public BattleRewardApplySummary(
            int xpGrantedPerMember,
            IReadOnlyList<CharacterLevelUpNotice> levelUps,
            IReadOnlyList<BattleLootDrop> appliedLoot
        )
        {
            XpGrantedPerMember = xpGrantedPerMember;
            LevelUps = levelUps ?? Array.Empty<CharacterLevelUpNotice>();
            AppliedLoot = appliedLoot ?? Array.Empty<BattleLootDrop>();
        }

        public static BattleRewardApplySummary Empty { get; } =
            new BattleRewardApplySummary(
                0,
                Array.Empty<CharacterLevelUpNotice>(),
                Array.Empty<BattleLootDrop>()
            );
    }
}
