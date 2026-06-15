using System.Collections.Generic;
using System.Text;

namespace GridDungeon.Core.Progression
{
    public static class BattleRewardSummaryFormatter
    {
        public static string Format(BattleRewardResult reward, BattleRewardApplySummary applied)
        {
            if (reward.Skipped || !reward.HasRewards)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            if (reward.TotalXp > 0)
            {
                builder.Append($"+{reward.TotalXp} XP (party)");
            }

            AppendLevelUpsInline(builder, applied.LevelUps);

            AppendLoot(builder, applied.AppliedLoot);
            return builder.ToString();
        }

        /// <summary>Multi-line copy for the post-battle reward screen.</summary>
        public static string FormatScreenBody(
            BattleRewardResult reward,
            BattleRewardApplySummary applied
        )
        {
            if (reward.Skipped || !reward.HasRewards)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            if (reward.TotalXp > 0)
            {
                builder.AppendLine(
                    $"<color=#F0F4FA>+{reward.TotalXp} XP to each party member</color>"
                );
            }

            AppendLevelUpLines(builder, applied.LevelUps, useRichText: true);

            AppendLootLines(builder, applied.AppliedLoot);
            return builder.ToString().TrimEnd();
        }

        /// <summary>Guarantees copy for the victory overlay when rolls succeeded but nothing landed in the bag.</summary>
        public static string EnsureScreenBody(
            BattleRewardResult reward,
            BattleRewardApplySummary applied,
            string formattedBody
        )
        {
            if (!string.IsNullOrEmpty(formattedBody))
            {
                return formattedBody;
            }

            if (reward.Skipped || !reward.HasRewards)
            {
                return string.Empty;
            }

            if (reward.TotalXp > 0)
            {
                return FormatScreenBody(reward, applied);
            }

            if (reward.Loot != null && reward.Loot.Count > 0)
            {
                return "<color=#E8ECF4>Loot could not fit in the party bag.</color>";
            }

            return "<color=#E8ECF4>Victory rewards collected.</color>";
        }

        public static string FormatLevelUpLine(CharacterLevelUpNotice notice) =>
            $"{notice.DisplayName} leveled up to level {notice.NewLevel}!";

        static void AppendLevelUpsInline(
            StringBuilder builder,
            IReadOnlyList<CharacterLevelUpNotice> levelUps
        )
        {
            if (levelUps == null || levelUps.Count == 0)
            {
                return;
            }

            for (int i = 0; i < levelUps.Count; i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" · ");
                }

                builder.Append(FormatLevelUpLine(levelUps[i]));
            }
        }

        static void AppendLevelUpLines(
            StringBuilder builder,
            IReadOnlyList<CharacterLevelUpNotice> levelUps,
            bool useRichText
        )
        {
            if (levelUps == null || levelUps.Count == 0)
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            for (int i = 0; i < levelUps.Count; i++)
            {
                string line = FormatLevelUpLine(levelUps[i]);
                builder.AppendLine(useRichText ? $"<color=#F0F4FA>{line}</color>" : line);
            }
        }

        static void AppendLootLines(StringBuilder builder, IReadOnlyList<BattleLootDrop> loot)
        {
            if (loot == null || loot.Count == 0)
            {
                return;
            }

            builder.AppendLine();
            builder.AppendLine("<color=#E8ECF4>Loot</color>");
            for (int i = 0; i < loot.Count; i++)
            {
                BattleLootDrop drop = loot[i];
                string line =
                    drop.Quantity > 1 ? $"  {drop.ItemId} ×{drop.Quantity}" : $"  {drop.ItemId}";
                builder.AppendLine($"<color=#E8ECF4>{line}</color>");
            }
        }

        static void AppendLoot(StringBuilder builder, IReadOnlyList<BattleLootDrop> loot)
        {
            if (loot == null || loot.Count == 0)
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.Append(" · ");
            }

            builder.Append("Loot: ");
            for (int i = 0; i < loot.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                BattleLootDrop drop = loot[i];
                builder.Append(drop.Quantity > 1 ? $"{drop.ItemId}×{drop.Quantity}" : drop.ItemId);
            }
        }
    }
}
