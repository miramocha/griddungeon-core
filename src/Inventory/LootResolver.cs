using System;
using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Progression;

namespace GridDungeon.Core.Inventory
{
    /// <summary>Rolls loot from a table using independent or weighted-pick resolve modes.</summary>
    public static class LootResolver
    {
        public static IReadOnlyList<BattleLootDrop> Resolve(
            LootTable table,
            LootResolveMode mode,
            Func<double> rollUnitInterval
        )
        {
            if (table.Entries == null || table.Entries.Length == 0)
            {
                return Array.Empty<BattleLootDrop>();
            }

            return mode switch
            {
                LootResolveMode.PickOneWeighted => ResolvePickOneWeighted(
                    table.Entries,
                    rollUnitInterval
                ),
                _ => ResolveIndependentEntries(table.Entries, rollUnitInterval),
            };
        }

        public static IReadOnlyList<BattleLootDrop> Resolve(
            LootTableData tableData,
            Func<double> rollUnitInterval
        ) => Resolve(tableData.Table, tableData.ResolveMode, rollUnitInterval);

        static List<BattleLootDrop> ResolveIndependentEntries(
            LootEntry[] entries,
            Func<double> rollUnitInterval
        )
        {
            var loot = new List<BattleLootDrop>();
            foreach (LootEntry entry in entries)
            {
                if (string.IsNullOrEmpty(entry.ItemId) || entry.DropChance <= 0f)
                {
                    continue;
                }

                double roll = rollUnitInterval();
                if (roll >= entry.DropChance)
                {
                    continue;
                }

                int qty = ResolveQuantity(entry, rollUnitInterval);
                if (qty <= 0)
                {
                    continue;
                }

                loot.Add(new BattleLootDrop(entry.ItemId, qty));
            }

            return loot;
        }

        static IReadOnlyList<BattleLootDrop> ResolvePickOneWeighted(
            LootEntry[] entries,
            Func<double> rollUnitInterval
        )
        {
            float totalWeight = 0f;
            foreach (LootEntry entry in entries)
            {
                if (string.IsNullOrEmpty(entry.ItemId) || entry.DropChance <= 0f)
                {
                    continue;
                }

                totalWeight += entry.DropChance;
            }

            if (totalWeight <= 0f)
            {
                return Array.Empty<BattleLootDrop>();
            }

            double pick = rollUnitInterval() * totalWeight;
            float cumulative = 0f;
            LootEntry? winner = null;
            foreach (LootEntry entry in entries)
            {
                if (string.IsNullOrEmpty(entry.ItemId) || entry.DropChance <= 0f)
                {
                    continue;
                }

                cumulative += entry.DropChance;
                if (pick < cumulative)
                {
                    winner = entry;
                    break;
                }
            }

            if (!winner.HasValue)
            {
                return Array.Empty<BattleLootDrop>();
            }

            LootEntry selected = winner.Value;
            int qty = ResolveQuantity(selected, rollUnitInterval);
            if (qty <= 0)
            {
                return Array.Empty<BattleLootDrop>();
            }

            return new[] { new BattleLootDrop(selected.ItemId, qty) };
        }

        static int ResolveQuantity(LootEntry entry, Func<double> rollUnitInterval)
        {
            int min = Math.Max(0, entry.MinQty);
            int max = Math.Max(min, entry.MaxQty);
            if (max == min)
            {
                return min;
            }

            double roll = rollUnitInterval();
            int span = max - min + 1;
            return Math.Min(max, min + (int)(roll * span));
        }
    }

    public readonly struct LootTableData
    {
        public LootTable Table { get; }
        public LootResolveMode ResolveMode { get; }

        public LootTableData(LootTable table, LootResolveMode resolveMode)
        {
            Table = table;
            ResolveMode = resolveMode;
        }

        public static LootTableData Empty { get; } =
            new LootTableData(default, LootResolveMode.IndependentEntries);
    }
}
