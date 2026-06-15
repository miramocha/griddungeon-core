using System;
using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Inventory;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Progression
{
    /// <summary>Resolves XP and loot from encounter definitions (Core, testable).</summary>
    public static class BattleRewardCalculator
    {
        public static BattleRewardResult Resolve(
            CombatEntryContext entry,
            IReadOnlyList<EnemyRewardData> enemies,
            bool isFoeContactFight,
            Func<double> rollUnitInterval,
            bool skipRewards = false
        )
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (skipRewards)
            {
                return BattleRewardResult.SkippedResult;
            }

            if (enemies == null || enemies.Count == 0)
            {
                return BattleRewardResult.Empty;
            }

            int totalXp = 0;
            var loot = new List<BattleLootDrop>();

            foreach (EnemyRewardData enemy in enemies)
            {
                totalXp += Math.Max(0, enemy.XpReward);
                AppendLoot(enemy, loot, rollUnitInterval);
            }

            if (isFoeContactFight && totalXp > 0)
            {
                totalXp = (int)Math.Round(totalXp * CharacterProgression.FoeContactXpMultiplier);
            }

            return new BattleRewardResult { TotalXp = totalXp, Loot = loot };
        }

        static void AppendLoot(
            EnemyRewardData enemy,
            List<BattleLootDrop> loot,
            Func<double> rollUnitInterval
        )
        {
            IReadOnlyList<BattleLootDrop> rolled = LootResolver.Resolve(
                enemy.LootTable,
                enemy.LootResolveMode,
                rollUnitInterval
            );
            if (rolled.Count == 0)
            {
                return;
            }

            loot.AddRange(rolled);
        }
    }

    public readonly struct EnemyRewardData
    {
        public string EnemyId { get; }
        public int XpReward { get; }
        public LootTable LootTable { get; }
        public LootResolveMode LootResolveMode { get; }

        public EnemyRewardData(
            string enemyId,
            int xpReward,
            LootTable lootTable,
            LootResolveMode lootResolveMode = LootResolveMode.IndependentEntries
        )
        {
            EnemyId = enemyId ?? string.Empty;
            XpReward = xpReward;
            LootTable = lootTable;
            LootResolveMode = lootResolveMode;
        }
    }
}
