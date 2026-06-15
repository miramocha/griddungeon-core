using GridDungeon.Core.Enums;
using GridDungeon.Core.Hub;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Inventory
{
    /// <summary>Projects worn loadout stats for the party menu equipment picker preview.</summary>
    public static class PartyEquipmentStatPreview
    {
        public static CombatantStats BuildProjectedStats(
            string classId,
            int level,
            EquipmentLoadout currentLoadout,
            EquipSlot wornSlot,
            PartyEquipmentPickerRow pickerRow,
            PartyInventory bag,
            IEquipmentStatSource? statSource
        )
        {
            EquipmentLoadout projected = currentLoadout.CloneLoadout();
            switch (pickerRow.Kind)
            {
                case PartyEquipmentPickerRowKind.Remove:
                    projected.SetEquipId(wornSlot, string.Empty);
                    break;
                case PartyEquipmentPickerRowKind.BagSlot:
                    if (
                        bag?.Slots == null
                        || pickerRow.BagSlotIndex < 0
                        || pickerRow.BagSlotIndex >= bag.Slots.Length
                    )
                    {
                        break;
                    }

                    InventorySlot slot = bag.Slots[pickerRow.BagSlotIndex];
                    if (
                        slot == null
                        || slot.Kind != InventorySlotKind.Equipment
                        || slot.Equipment.IsEmpty
                    )
                    {
                        break;
                    }

                    projected.SetEquipId(wornSlot, slot.Equipment.EquipId);
                    break;
            }

            return CombatantEquipmentBuild.BuildEffectiveStats(
                classId,
                level,
                projected,
                statSource
            );
        }
    }
}
