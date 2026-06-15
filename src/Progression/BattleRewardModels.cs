using System;
using System.Collections.Generic;

namespace GridDungeon.Core.Progression
{
    public readonly struct CharacterLevelUpNotice
    {
        public string DisplayName { get; }
        public int NewLevel { get; }

        public CharacterLevelUpNotice(string displayName, int newLevel)
        {
            DisplayName = displayName ?? string.Empty;
            NewLevel = Math.Max(1, newLevel);
        }
    }

    public readonly struct BattleLootDrop
    {
        public string ItemId { get; }
        public int Quantity { get; }

        public BattleLootDrop(string itemId, int quantity)
        {
            ItemId = itemId ?? string.Empty;
            Quantity = Math.Max(0, quantity);
        }
    }

    public sealed class BattleRewardResult
    {
        public int TotalXp { get; set; }
        public IReadOnlyList<BattleLootDrop> Loot { get; set; } = Array.Empty<BattleLootDrop>();
        public bool Skipped { get; set; }

        public bool HasRewards => !Skipped && (TotalXp > 0 || Loot.Count > 0);

        public static BattleRewardResult SkippedResult { get; } =
            new BattleRewardResult { Skipped = true };

        public static BattleRewardResult Empty { get; } = new BattleRewardResult();
    }
}
