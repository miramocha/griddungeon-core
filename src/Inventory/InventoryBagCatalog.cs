using System;
using System.Collections.Generic;

namespace GridDungeon.Core.Inventory
{
    public sealed class InventoryBagSlotRow
    {
        public int SlotIndex { get; set; }
        public InventorySlotKind Kind { get; set; }
        public ItemStack Stack { get; set; }
        public EquipmentInstance Equipment { get; set; }
    }

    public sealed class InventoryBagTabModel
    {
        public string TabId { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public InventoryBagSlotRow[] Slots { get; set; } = Array.Empty<InventoryBagSlotRow>();
    }

    public sealed class InventoryBagPresentationModel
    {
        public InventoryBagTabModel[] Tabs { get; set; } = Array.Empty<InventoryBagTabModel>();
        public int DefaultTabIndex { get; set; }
    }

    public static class InventoryBagCatalog
    {
        public static InventoryBagPresentationModel BuildTabs(PartyInventory? inventory)
        {
            inventory ??= PartyInventory.CreateEmpty();

            return new InventoryBagPresentationModel
            {
                Tabs = new[]
                {
                    CreateTab(
                        InventoryConstants.TabIdAll,
                        InventoryConstants.TabLabelAll,
                        inventory,
                        null
                    ),
                    CreateTab(
                        InventoryConstants.TabIdConsumables,
                        InventoryConstants.TabLabelConsumables,
                        inventory,
                        InventorySlotKind.Consumable
                    ),
                    CreateTab(
                        InventoryConstants.TabIdEquipment,
                        InventoryConstants.TabLabelEquipment,
                        inventory,
                        InventorySlotKind.Equipment
                    ),
                    CreateTab(
                        InventoryConstants.TabIdMaterials,
                        InventoryConstants.TabLabelMaterials,
                        inventory,
                        InventorySlotKind.Material
                    ),
                },
                DefaultTabIndex = 0,
            };
        }

        static InventoryBagTabModel CreateTab(
            string tabId,
            string label,
            PartyInventory inventory,
            InventorySlotKind? filterKind
        )
        {
            var rows = new List<InventoryBagSlotRow>();
            if (inventory.Slots != null)
            {
                for (int i = 0; i < inventory.Slots.Length; i++)
                {
                    InventorySlot slot = inventory.Slots[i] ?? InventorySlot.Empty();
                    if (slot.Kind == InventorySlotKind.Empty)
                    {
                        continue;
                    }

                    if (filterKind.HasValue && slot.Kind != filterKind.Value)
                    {
                        continue;
                    }

                    rows.Add(
                        new InventoryBagSlotRow
                        {
                            SlotIndex = i,
                            Kind = slot.Kind,
                            Stack = slot.Stack,
                            Equipment = slot.Equipment,
                        }
                    );
                }
            }

            return new InventoryBagTabModel
            {
                TabId = tabId,
                Label = label,
                Slots = rows.ToArray(),
            };
        }
    }
}
