using System;
using System.Collections.Generic;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Inventory
{
    public enum PartyEquipmentPickerRowKind
    {
        Remove = 0,
        BagSlot = 1,
    }

    public readonly struct PartyEquipmentPickerRow
    {
        public PartyEquipmentPickerRow(
            PartyEquipmentPickerRowKind kind,
            int bagSlotIndex,
            string label
        )
        {
            Kind = kind;
            BagSlotIndex = bagSlotIndex;
            Label = label ?? string.Empty;
        }

        public PartyEquipmentPickerRowKind Kind { get; }

        /// <summary>Bag index when <see cref="Kind"/> is <see cref="PartyEquipmentPickerRowKind.BagSlot"/>.</summary>
        public int BagSlotIndex { get; }

        public string Label { get; }
    }

    /// <summary>Bag-row filter and sub-picker rows for the party menu Equipment pane (ADR 036 §7).</summary>
    public static class PartyEquipmentCatalog
    {
        public const string RemoveRowLabel = "Remove";

        public static PartyEquipmentPickerRow[] BuildPickerRows(
            PartyInventory bag,
            EquipmentLoadout loadout,
            EquipSlot wornSlot,
            string classId,
            IInventoryContentRules rules
        )
        {
            if (bag?.Slots == null || loadout == null || rules == null)
            {
                return Array.Empty<PartyEquipmentPickerRow>();
            }

            bool slotFilled = !loadout.IsSlotEmpty(wornSlot);
            bool canReplace = !slotFilled || InventoryRules.HasEmptySlot(bag);
            var rows = new List<PartyEquipmentPickerRow>();

            if (slotFilled)
            {
                rows.Add(
                    new PartyEquipmentPickerRow(
                        PartyEquipmentPickerRowKind.Remove,
                        -1,
                        RemoveRowLabel
                    )
                );
            }

            if (!canReplace)
            {
                return rows.ToArray();
            }

            foreach (int bagIndex in FilterEquippableBagSlotIndices(bag, wornSlot, classId, rules))
            {
                rows.Add(
                    new PartyEquipmentPickerRow(
                        PartyEquipmentPickerRowKind.BagSlot,
                        bagIndex,
                        BuildBagRowLabel(bag, bagIndex, rules)
                    )
                );
            }

            return rows.ToArray();
        }

        public static int[] FilterEquippableBagSlotIndices(
            PartyInventory bag,
            EquipSlot wornSlot,
            string classId,
            IInventoryContentRules rules
        )
        {
            if (bag?.Slots == null)
            {
                return Array.Empty<int>();
            }

            var indices = new List<int>();
            for (int i = 0; i < bag.Slots.Length; i++)
            {
                if (IsEquippableBagSlot(bag, i, wornSlot, classId, rules))
                {
                    indices.Add(i);
                }
            }

            return indices.ToArray();
        }

        public static bool IsEquippableBagSlot(
            PartyInventory bag,
            int bagSlotIndex,
            EquipSlot wornSlot,
            string classId,
            IInventoryContentRules rules
        )
        {
            if (bag?.Slots == null || rules == null)
            {
                return false;
            }

            if (bagSlotIndex < 0 || bagSlotIndex >= bag.Slots.Length)
            {
                return false;
            }

            InventorySlot slot = bag.Slots[bagSlotIndex];
            if (slot == null || slot.Kind != InventorySlotKind.Equipment || slot.Equipment.IsEmpty)
            {
                return false;
            }

            if (!slot.Equipment.IsIdentified)
            {
                return false;
            }

            if (
                !rules.TryGetEquipment(
                    slot.Equipment.EquipId,
                    out InventoryEquipmentDefinition definition
                )
            )
            {
                return false;
            }

            if (definition.Slot != wornSlot)
            {
                return false;
            }

            return definition.IsAllowedForClass(classId);
        }

        static string BuildBagRowLabel(
            PartyInventory bag,
            int bagSlotIndex,
            IInventoryContentRules rules
        )
        {
            InventorySlot slot = bag.Slots[bagSlotIndex];
            if (
                slot == null
                || slot.Kind != InventorySlotKind.Equipment
                || !rules.TryGetEquipment(
                    slot.Equipment.EquipId,
                    out InventoryEquipmentDefinition definition
                )
            )
            {
                return $"Bag slot {bagSlotIndex + 1}";
            }

            return EquipSlotDisplay.GetLabel(definition.Slot) + ": " + definition.DisplayName;
        }
    }
}
