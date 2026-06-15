using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Inventory
{
    public static class PartyEquipmentOperations
    {
        public static bool TryEquipFromBag(
            CharacterSaveData member,
            PartyInventory bag,
            int bagSlotIndex,
            IInventoryContentRules rules,
            out string failureReason
        )
        {
            failureReason = string.Empty;
            if (member == null || bag == null || rules == null)
            {
                failureReason = "Invalid state.";
                return false;
            }

            EquipmentLoadout loadout = member.Equipment ?? new EquipmentLoadout();
            if (
                !InventoryRules.TryEquip(
                    bag,
                    bagSlotIndex,
                    loadout,
                    member.ClassId,
                    rules,
                    out failureReason
                )
            )
            {
                return false;
            }

            member.Equipment = loadout;
            return true;
        }

        /// <summary>
        /// Unequips the worn slot when occupied (requires an empty bag slot), then equips from the bag row.
        /// </summary>
        public static bool TryReplaceFromBag(
            CharacterSaveData member,
            PartyInventory bag,
            int bagSlotIndex,
            EquipSlot wornSlot,
            IInventoryContentRules rules,
            out string failureReason
        )
        {
            failureReason = string.Empty;
            if (member == null || bag == null || rules == null)
            {
                failureReason = "Invalid state.";
                return false;
            }

            EquipmentLoadout loadout = member.Equipment ?? new EquipmentLoadout();
            if (!loadout.IsSlotEmpty(wornSlot))
            {
                if (!InventoryRules.HasEmptySlot(bag))
                {
                    failureReason = "Bag is full.";
                    return false;
                }

                if (!TryUnequipToBag(member, bag, wornSlot, out failureReason))
                {
                    return false;
                }
            }

            return TryEquipFromBag(member, bag, bagSlotIndex, rules, out failureReason);
        }

        public static bool TryUnequipToBag(
            CharacterSaveData member,
            PartyInventory bag,
            EquipSlot equipSlot,
            out string failureReason
        )
        {
            failureReason = string.Empty;
            if (member == null || bag == null)
            {
                failureReason = "Invalid state.";
                return false;
            }

            EquipmentLoadout loadout = member.Equipment ?? new EquipmentLoadout();
            if (!InventoryRules.TryUnequip(bag, equipSlot, loadout, out failureReason))
            {
                return false;
            }

            member.Equipment = loadout;
            return true;
        }
    }
}
