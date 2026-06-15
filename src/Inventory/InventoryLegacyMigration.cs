using System.Collections.Generic;

namespace GridDungeon.Core.Inventory
{
    /// <summary>Maps pre-#152 <c>GatheredItemIds</c> stub lists into bag stacks.</summary>
    public static class InventoryLegacyMigration
    {
        public static PartyInventory MigrateGatheredItemIds(
            IReadOnlyList<string> gatheredItemIds,
            IInventoryContentRules rules,
            int slotCount = InventoryConstants.PartyBagSlotCount
        )
        {
            PartyInventory inventory = PartyInventory.CreateEmpty(slotCount);
            if (gatheredItemIds == null || gatheredItemIds.Count == 0)
            {
                return inventory;
            }

            foreach (string itemId in gatheredItemIds)
            {
                if (string.IsNullOrEmpty(itemId))
                {
                    continue;
                }

                InventoryRules.TryAddConsumable(inventory, itemId, 1, rules, out _);
            }

            return inventory;
        }
    }
}
