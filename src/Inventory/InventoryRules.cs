using System;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Inventory
{
    public static class InventoryRules
    {
        public static bool HasEmptySlot(PartyInventory inventory) =>
            FindFirstEmptySlotIndex(inventory) >= 0;

        public static bool TryAddConsumable(
            PartyInventory inventory,
            string itemId,
            int quantity,
            IInventoryContentRules rules,
            out int quantityNotAdded
        )
        {
            quantityNotAdded = quantity;
            if (!TryValidateInventory(inventory, rules, out quantityNotAdded, itemId, quantity))
            {
                return false;
            }

            int maxStack = rules.GetMaxStack(itemId);
            int remaining = quantity;
            while (remaining > 0)
            {
                if (!TryAddConsumablePass(inventory, itemId, ref remaining, maxStack))
                {
                    quantityNotAdded = remaining;
                    return false;
                }
            }

            quantityNotAdded = 0;
            return true;
        }

        public static bool TryAddEquipmentInstance(
            PartyInventory inventory,
            EquipmentInstance instance
        )
        {
            if (inventory?.Slots == null || instance.IsEmpty)
            {
                return false;
            }

            int emptyIndex = FindFirstEmptySlotIndex(inventory);
            if (emptyIndex < 0)
            {
                return false;
            }

            inventory.Slots[emptyIndex] = InventorySlot.FromEquipment(instance);
            return true;
        }

        public static bool TryRemoveConsumable(
            PartyInventory inventory,
            string itemId,
            int quantity,
            out int quantityRemoved
        )
        {
            quantityRemoved = 0;
            if (inventory?.Slots == null || string.IsNullOrEmpty(itemId) || quantity <= 0)
            {
                return false;
            }

            int remaining = quantity;
            for (int i = 0; i < inventory.Slots.Length && remaining > 0; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (slot == null)
                {
                    continue;
                }

                if (
                    slot.Kind is not (InventorySlotKind.Consumable or InventorySlotKind.Material)
                    || slot.Stack.ItemId != itemId
                )
                {
                    continue;
                }

                int take = Math.Min(remaining, slot.Stack.Quantity);
                int left = slot.Stack.Quantity - take;
                quantityRemoved += take;
                remaining -= take;
                inventory.Slots[i] =
                    left > 0 ? InventorySlot.FromConsumable(itemId, left) : InventorySlot.Empty();
            }

            return quantityRemoved > 0;
        }

        public static bool TryRemoveEquipmentInstance(
            PartyInventory inventory,
            string instanceId,
            out EquipmentInstance removed
        )
        {
            removed = default;
            if (inventory?.Slots == null || string.IsNullOrEmpty(instanceId))
            {
                return false;
            }

            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (slot == null || slot.Kind != InventorySlotKind.Equipment)
                {
                    continue;
                }

                if (slot.Equipment.InstanceId != instanceId)
                {
                    continue;
                }

                removed = slot.Equipment;
                inventory.Slots[i] = InventorySlot.Empty();
                return true;
            }

            return false;
        }

        public static bool TryRemoveFirstEquipmentByEquipId(
            PartyInventory inventory,
            string equipId,
            out EquipmentInstance removed
        )
        {
            removed = default;
            if (inventory?.Slots == null || string.IsNullOrEmpty(equipId))
            {
                return false;
            }

            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (slot == null || slot.Kind != InventorySlotKind.Equipment)
                {
                    continue;
                }

                if (slot.Equipment.EquipId != equipId)
                {
                    continue;
                }

                removed = slot.Equipment;
                inventory.Slots[i] = InventorySlot.Empty();
                return true;
            }

            return false;
        }

        public static int FindFirstUnidentifiedEquipmentSlot(
            PartyInventory inventory,
            string equipId
        )
        {
            if (inventory?.Slots == null || string.IsNullOrEmpty(equipId))
            {
                return -1;
            }

            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (slot == null || slot.Kind != InventorySlotKind.Equipment)
                {
                    continue;
                }

                if (slot.Equipment.EquipId == equipId && !slot.Equipment.IsIdentified)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool TryFindFirstConsumable(
            PartyInventory inventory,
            out string itemId,
            out int slotIndex
        )
        {
            itemId = string.Empty;
            slotIndex = -1;
            if (inventory?.Slots == null)
            {
                return false;
            }

            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (slot == null || slot.Kind != InventorySlotKind.Consumable || slot.Stack.IsEmpty)
                {
                    continue;
                }

                itemId = slot.Stack.ItemId;
                slotIndex = i;
                return true;
            }

            return false;
        }

        public static int CountConsumable(PartyInventory inventory, string itemId)
        {
            if (inventory?.Slots == null || string.IsNullOrEmpty(itemId))
            {
                return 0;
            }

            int total = 0;
            foreach (InventorySlot slot in inventory.Slots)
            {
                if (
                    slot != null
                    && slot.Kind == InventorySlotKind.Consumable
                    && slot.Stack.ItemId == itemId
                )
                {
                    total += slot.Stack.Quantity;
                }
            }

            return total;
        }

        public static int CountEquipmentByEquipId(PartyInventory inventory, string equipId)
        {
            if (inventory?.Slots == null || string.IsNullOrEmpty(equipId))
            {
                return 0;
            }

            int total = 0;
            foreach (InventorySlot slot in inventory.Slots)
            {
                if (
                    slot != null
                    && slot.Kind == InventorySlotKind.Equipment
                    && slot.Equipment.EquipId == equipId
                )
                {
                    total++;
                }
            }

            return total;
        }

        public static bool TryEquip(
            PartyInventory inventory,
            int bagSlotIndex,
            EquipmentLoadout loadout,
            string classId,
            IInventoryContentRules rules,
            out string failureReason
        )
        {
            failureReason = string.Empty;
            if (inventory?.Slots == null || loadout == null || rules == null)
            {
                failureReason = "Invalid inventory state.";
                return false;
            }

            if (bagSlotIndex < 0 || bagSlotIndex >= inventory.Slots.Length)
            {
                failureReason = "Invalid bag slot.";
                return false;
            }

            InventorySlot slot = inventory.Slots[bagSlotIndex];
            if (slot == null || slot.Kind != InventorySlotKind.Equipment || slot.Equipment.IsEmpty)
            {
                failureReason = "No equipment in that slot.";
                return false;
            }

            string equipId = slot.Equipment.EquipId;
            if (!rules.TryGetEquipment(equipId, out InventoryEquipmentDefinition definition))
            {
                failureReason = "Unknown equipment.";
                return false;
            }

            if (!definition.IsAllowedForClass(classId))
            {
                failureReason = "Class cannot equip that item.";
                return false;
            }

            if (!loadout.IsSlotEmpty(definition.Slot))
            {
                failureReason = "Equipment slot already occupied.";
                return false;
            }

            loadout.SetEquipId(definition.Slot, equipId);
            inventory.Slots[bagSlotIndex] = InventorySlot.Empty();
            return true;
        }

        public static bool TryUnequip(
            PartyInventory inventory,
            EquipSlot equipSlot,
            EquipmentLoadout loadout,
            out string failureReason
        )
        {
            failureReason = string.Empty;
            if (inventory?.Slots == null || loadout == null)
            {
                failureReason = "Invalid inventory state.";
                return false;
            }

            string equipId = loadout.GetEquipId(equipSlot);
            if (string.IsNullOrEmpty(equipId))
            {
                failureReason = "Nothing equipped in that slot.";
                return false;
            }

            int emptyIndex = FindFirstEmptySlotIndex(inventory);
            if (emptyIndex < 0)
            {
                failureReason = "Bag is full.";
                return false;
            }

            var instance = new EquipmentInstance
            {
                InstanceId = CreateInstanceId(),
                EquipId = equipId,
                IsIdentified = true,
            };
            inventory.Slots[emptyIndex] = InventorySlot.FromEquipment(instance);
            loadout.SetEquipId(equipSlot, string.Empty);
            return true;
        }

        public static bool TryIdentify(PartyInventory inventory, int bagSlotIndex)
        {
            if (
                inventory?.Slots == null
                || bagSlotIndex < 0
                || bagSlotIndex >= inventory.Slots.Length
            )
            {
                return false;
            }

            InventorySlot slot = inventory.Slots[bagSlotIndex];
            if (slot == null || slot.Kind != InventorySlotKind.Equipment)
            {
                return false;
            }

            if (slot.Equipment.IsIdentified)
            {
                return true;
            }

            EquipmentInstance identified = slot.Equipment;
            identified.IsIdentified = true;
            inventory.Slots[bagSlotIndex] = InventorySlot.FromEquipment(identified);
            return true;
        }

        public static string CreateInstanceId() => Guid.NewGuid().ToString("N");

        public static EquipmentInstance CreateEquipmentInstance(
            string equipId,
            bool isIdentified = true
        ) =>
            new()
            {
                InstanceId = CreateInstanceId(),
                EquipId = equipId ?? string.Empty,
                IsIdentified = isIdentified,
            };

        static bool TryValidateInventory(
            PartyInventory inventory,
            IInventoryContentRules rules,
            out int quantityNotAdded,
            string itemId,
            int quantity
        )
        {
            quantityNotAdded = quantity;
            if (inventory?.Slots == null || rules == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(itemId) || quantity <= 0)
            {
                return false;
            }

            return true;
        }

        static bool TryAddConsumablePass(
            PartyInventory inventory,
            string itemId,
            ref int remaining,
            int maxStack
        )
        {
            if (maxStack <= 0)
            {
                maxStack = InventoryConstants.DefaultMaxStack;
            }

            for (int i = 0; i < inventory.Slots!.Length && remaining > 0; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (
                    slot != null
                    && slot.Kind == InventorySlotKind.Consumable
                    && slot.Stack.ItemId == itemId
                    && slot.Stack.Quantity < maxStack
                )
                {
                    int room = maxStack - slot.Stack.Quantity;
                    int add = Math.Min(room, remaining);
                    inventory.Slots[i] = InventorySlot.FromConsumable(
                        itemId,
                        slot.Stack.Quantity + add
                    );
                    remaining -= add;
                }
            }

            while (remaining > 0)
            {
                int emptyIndex = FindFirstEmptySlotIndex(inventory);
                if (emptyIndex < 0)
                {
                    return false;
                }

                int add = Math.Min(maxStack, remaining);
                inventory.Slots[emptyIndex] = InventorySlot.FromConsumable(itemId, add);
                remaining -= add;
            }

            return true;
        }

        static int FindFirstEmptySlotIndex(PartyInventory inventory)
        {
            if (inventory?.Slots == null)
            {
                return -1;
            }

            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                InventorySlot slot = inventory.Slots[i];
                if (slot == null || slot.Kind == InventorySlotKind.Empty)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
