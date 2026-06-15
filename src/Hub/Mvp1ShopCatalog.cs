using System.Collections.Generic;
using GridDungeon.Core.Content;

namespace GridDungeon.Core.Hub
{
    /// <summary>MVP1 day-one hub shop stock (#153). Prices live on content SOs.</summary>
    public static class Mvp1ShopCatalog
    {
        public static IReadOnlyList<string> DayOneConsumableIds =>
            Mvp1LockedContentRegistry.ItemIds;

        public static IReadOnlyList<string> DayOneEquipmentIds =>
            Mvp1LockedContentRegistry.EquipmentIds;

        public static bool IsDayOneStock(string stockId) =>
            IsDayOneConsumable(stockId) || IsDayOneEquipment(stockId);

        public static bool IsDayOneConsumable(string itemId) =>
            !string.IsNullOrEmpty(itemId)
            && System.Array.IndexOf(Mvp1LockedContentRegistry.ItemIds, itemId) >= 0;

        public static bool IsDayOneEquipment(string equipId) =>
            !string.IsNullOrEmpty(equipId)
            && System.Array.IndexOf(Mvp1LockedContentRegistry.EquipmentIds, equipId) >= 0;
    }
}
