using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Inventory
{
    /// <summary>Content lookups for stack limits and equip validation (Runtime SOs implement in #153).</summary>
    public interface IInventoryContentRules
    {
        int GetMaxStack(string itemId);

        bool TryGetEquipment(string equipId, out InventoryEquipmentDefinition equipment);
    }

    public readonly struct InventoryEquipmentDefinition
    {
        public InventoryEquipmentDefinition(
            string equipId,
            EquipSlot slot,
            string[] allowedClassIds,
            string displayName = ""
        )
        {
            EquipId = equipId ?? string.Empty;
            Slot = slot;
            AllowedClassIds = allowedClassIds ?? System.Array.Empty<string>();
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? EquipId : displayName;
        }

        public string EquipId { get; }

        public string DisplayName { get; }
        public EquipSlot Slot { get; }
        public string[] AllowedClassIds { get; }

        public bool IsAllowedForClass(string classId)
        {
            if (AllowedClassIds == null || AllowedClassIds.Length == 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(classId))
            {
                return false;
            }

            foreach (string allowed in AllowedClassIds)
            {
                if (allowed == classId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
