namespace GridDungeon.Core.Inventory
{
    public static class InventoryConstants
    {
        public const int PartyBagSlotCount = 30;

        /// <summary>Fallback when <see cref="IInventoryContentRules"/> has no item definition yet (#153).</summary>
        public const int DefaultMaxStack = 99;

        public const string TabIdAll = "all";
        public const string TabIdConsumables = "consumables";
        public const string TabIdEquipment = "equipment";
        public const string TabIdMaterials = "materials";

        public const string TabLabelAll = "All";
        public const string TabLabelConsumables = "Consumables";
        public const string TabLabelEquipment = "Equipment";
        public const string TabLabelMaterials = "Materials";
    }
}
