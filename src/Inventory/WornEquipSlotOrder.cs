using System;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Inventory
{
    /// <summary>Canonical worn-slot display order for party equipment UI and coordinator state.</summary>
    public static class WornEquipSlotOrder
    {
        public static readonly EquipSlot[] DisplayOrder =
        {
            EquipSlot.Weapon,
            EquipSlot.Head,
            EquipSlot.Body,
            EquipSlot.Legs,
            EquipSlot.Accessory,
        };

        public static int IndexOf(EquipSlot slot)
        {
            for (int i = 0; i < DisplayOrder.Length; i++)
            {
                if (DisplayOrder[i] == slot)
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(slot));
        }
    }
}
