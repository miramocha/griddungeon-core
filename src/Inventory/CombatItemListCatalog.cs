using System;
using System.Collections.Generic;

namespace GridDungeon.Core.Inventory
{
    public readonly struct CombatItemListRow
    {
        public CombatItemListRow(int bagSlotIndex, string itemId, int quantity)
        {
            BagSlotIndex = bagSlotIndex;
            ItemId = itemId ?? string.Empty;
            Quantity = Math.Max(0, quantity);
        }

        public int BagSlotIndex { get; }

        public string ItemId { get; }

        public int Quantity { get; }
    }

    /// <summary>
    /// Combat <c>Item</c> command row list — one row per occupied consumable bag slot (#157).
    /// Use-context / effect filtering deferred.
    /// </summary>
    public static class CombatItemListCatalog
    {
        public static bool HasUsableItems(PartyInventory? bag)
        {
            if (bag?.Slots == null || bag.Slots.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < bag.Slots.Length; i++)
            {
                InventorySlot? slot = bag.Slots[i];
                if (slot?.Kind == InventorySlotKind.Consumable && !slot.Stack.IsEmpty)
                {
                    return true;
                }
            }

            return false;
        }

        public static CombatItemListRow[] Build(PartyInventory? bag)
        {
            if (bag?.Slots == null || bag.Slots.Length == 0)
            {
                return Array.Empty<CombatItemListRow>();
            }

            var rows = new List<CombatItemListRow>();
            for (int i = 0; i < bag.Slots.Length; i++)
            {
                InventorySlot? slot = bag.Slots[i];
                if (slot == null || slot.Kind != InventorySlotKind.Consumable || slot.Stack.IsEmpty)
                {
                    continue;
                }

                rows.Add(new CombatItemListRow(i, slot.Stack.ItemId, slot.Stack.Quantity));
            }

            return rows.ToArray();
        }
    }
}
