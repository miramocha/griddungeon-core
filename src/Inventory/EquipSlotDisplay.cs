using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Inventory
{
    public static class EquipSlotDisplay
    {
        public static string GetLabel(EquipSlot slot) =>
            slot switch
            {
                EquipSlot.Weapon => "weapon",
                EquipSlot.Head => "head",
                EquipSlot.Body => "body",
                EquipSlot.Legs => "legs",
                EquipSlot.Accessory => "accessory",
                _ => slot.ToString().ToLowerInvariant(),
            };
    }
}
