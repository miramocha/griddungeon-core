using GridDungeon.Core.Content;

namespace GridDungeon.Core.Inventory
{
    /// <summary>Equipment stat/resist bonuses for <see cref="EquipmentStatAggregator"/> (Runtime SOs implement).</summary>
    public interface IEquipmentStatSource
    {
        bool TryGetEquipmentBonuses(
            string equipId,
            out CharacterBaseStats statBonus,
            out StatusResistBonuses resistBonuses
        );
    }
}
