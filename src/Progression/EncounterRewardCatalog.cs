using System;
using System.Collections.Generic;
using GridDungeon.Core.Content;

namespace GridDungeon.Core.Progression
{
    /// <summary>Collects enemy reward rows for an encounter group from slot configs.</summary>
    public static class EncounterRewardCatalog
    {
        public static IReadOnlyList<EnemyRewardData> CollectEnemyRewards(
            EncounterGroupData group,
            Func<string, EnemyRewardData?> resolveEnemy
        )
        {
            if (resolveEnemy == null)
            {
                throw new ArgumentNullException(nameof(resolveEnemy));
            }

            var rewards = new List<EnemyRewardData>();
            AppendRow(group.FrontRow, resolveEnemy, rewards);
            AppendRow(group.BackRow, resolveEnemy, rewards);
            return rewards;
        }

        static void AppendRow(
            EnemySlotConfig[] row,
            Func<string, EnemyRewardData?> resolveEnemy,
            List<EnemyRewardData> rewards
        )
        {
            if (row == null)
            {
                return;
            }

            foreach (EnemySlotConfig slot in row)
            {
                if (string.IsNullOrEmpty(slot.EnemyDefinitionId))
                {
                    continue;
                }

                EnemyRewardData? data = resolveEnemy(slot.EnemyDefinitionId);
                if (data.HasValue)
                {
                    rewards.Add(data.Value);
                }
            }
        }
    }

    public readonly struct EncounterGroupData
    {
        public EnemySlotConfig[] FrontRow { get; }
        public EnemySlotConfig[] BackRow { get; }

        public EncounterGroupData(EnemySlotConfig[] frontRow, EnemySlotConfig[] backRow)
        {
            FrontRow = frontRow ?? Array.Empty<EnemySlotConfig>();
            BackRow = backRow ?? Array.Empty<EnemySlotConfig>();
        }
    }
}
