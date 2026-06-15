namespace GridDungeon.Core.Inventory
{
    public static class PartyInventoryNormalizer
    {
        public static PartyInventory Normalize(PartyInventory? inventory)
        {
            if (inventory?.Slots == null || inventory.Slots.Length == 0)
            {
                return PartyInventory.CreateEmpty();
            }

            if (inventory.Slots.Length == InventoryConstants.PartyBagSlotCount)
            {
                return inventory;
            }

            PartyInventory normalized = PartyInventory.CreateEmpty();
            int copyLength = System.Math.Min(inventory.Slots.Length, normalized.Slots.Length);
            for (int i = 0; i < copyLength; i++)
            {
                normalized.Slots[i] = inventory.Slots[i] ?? InventorySlot.Empty();
            }

            return normalized;
        }
    }
}
