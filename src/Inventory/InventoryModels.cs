using System;

namespace GridDungeon.Core.Inventory
{
    [Serializable]
    public struct ItemStack
    {
        public string ItemId;
        public int Quantity;

        public bool IsEmpty => string.IsNullOrEmpty(ItemId) || Quantity <= 0;

        public static ItemStack Create(string itemId, int quantity) =>
            new() { ItemId = itemId ?? string.Empty, Quantity = quantity };
    }

    [Serializable]
    public struct EquipmentInstance
    {
        public string InstanceId;
        public string EquipId;
        public bool IsIdentified;

        public bool IsEmpty => string.IsNullOrEmpty(InstanceId) || string.IsNullOrEmpty(EquipId);
    }

    [Serializable]
    public sealed class InventorySlot
    {
        public InventorySlotKind Kind;
        public ItemStack Stack;
        public EquipmentInstance Equipment;

        public static InventorySlot Empty() =>
            new()
            {
                Kind = InventorySlotKind.Empty,
                Stack = default,
                Equipment = default,
            };

        public static InventorySlot FromConsumable(string itemId, int quantity) =>
            new()
            {
                Kind = InventorySlotKind.Consumable,
                Stack = ItemStack.Create(itemId, quantity),
                Equipment = default,
            };

        public static InventorySlot FromMaterial(string itemId, int quantity) =>
            new()
            {
                Kind = InventorySlotKind.Material,
                Stack = ItemStack.Create(itemId, quantity),
                Equipment = default,
            };

        public static InventorySlot FromEquipment(EquipmentInstance instance) =>
            new()
            {
                Kind = InventorySlotKind.Equipment,
                Stack = default,
                Equipment = instance,
            };
    }

    [Serializable]
    public sealed class PartyInventory
    {
        public InventorySlot[] Slots = Array.Empty<InventorySlot>();

        public static PartyInventory CreateEmpty(
            int slotCount = InventoryConstants.PartyBagSlotCount
        )
        {
            if (slotCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slotCount));
            }

            var slots = new InventorySlot[slotCount];
            for (int i = 0; i < slotCount; i++)
            {
                slots[i] = InventorySlot.Empty();
            }

            return new PartyInventory { Slots = slots };
        }

        public PartyInventory Clone()
        {
            if (Slots == null || Slots.Length == 0)
            {
                return CreateEmpty();
            }

            var copy = new InventorySlot[Slots.Length];
            for (int i = 0; i < Slots.Length; i++)
            {
                InventorySlot slot = Slots[i] ?? InventorySlot.Empty();
                copy[i] = new InventorySlot
                {
                    Kind = slot.Kind,
                    Stack = slot.Stack,
                    Equipment = slot.Equipment,
                };
            }

            return new PartyInventory { Slots = copy };
        }

        public int OccupiedSlotCount
        {
            get
            {
                int count = 0;
                if (Slots == null)
                {
                    return 0;
                }

                foreach (InventorySlot slot in Slots)
                {
                    if (slot != null && slot.Kind != InventorySlotKind.Empty)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
    }
}
