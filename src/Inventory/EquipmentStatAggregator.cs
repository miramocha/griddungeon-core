using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Inventory
{
    public static class EquipmentStatAggregator
    {
        public static CombatantStats Apply(
            CombatantStats baseStats,
            EquipmentLoadout? loadout,
            IEquipmentStatSource source
        )
        {
            if (loadout == null || source == null)
            {
                return baseStats;
            }

            CharacterBaseStats bonus = SumStatBonuses(loadout, source);
            CharacterBaseStats combined = AddBaseStats(ToCharacterBaseStats(baseStats), bonus);
            return StatMapping.ToCombatantStats(combined);
        }

        /// <summary>
        /// Sums equipment resist bonuses. MVP1 combat/status resolution does not consume this yet;
        /// wire through <c>StatusSystem</c> when equipment resist is implemented.
        /// </summary>
        public static StatusResistBonuses SumResistBonuses(
            EquipmentLoadout? loadout,
            IEquipmentStatSource source
        )
        {
            if (loadout == null || source == null)
            {
                return default;
            }

            StatusResistBonuses total = default;
            foreach (EquipSlot slot in EquipSlots.All)
            {
                string equipId = loadout.GetEquipId(slot);
                if (string.IsNullOrEmpty(equipId))
                {
                    continue;
                }

                if (!source.TryGetEquipmentBonuses(equipId, out _, out StatusResistBonuses resist))
                {
                    continue;
                }

                total = AddResists(total, resist);
            }

            return total;
        }

        static CharacterBaseStats SumStatBonuses(
            EquipmentLoadout loadout,
            IEquipmentStatSource source
        )
        {
            CharacterBaseStats total = default;
            foreach (EquipSlot slot in EquipSlots.All)
            {
                string equipId = loadout.GetEquipId(slot);
                if (string.IsNullOrEmpty(equipId))
                {
                    continue;
                }

                if (!source.TryGetEquipmentBonuses(equipId, out CharacterBaseStats bonus, out _))
                {
                    continue;
                }

                total = AddBaseStats(total, bonus);
            }

            return total;
        }

        static CharacterBaseStats AddBaseStats(CharacterBaseStats a, CharacterBaseStats b) =>
            new()
            {
                Hp = a.Hp + b.Hp,
                Mp = a.Mp + b.Mp,
                Str = a.Str + b.Str,
                Tec = a.Tec + b.Tec,
                Agi = a.Agi + b.Agi,
                Vit = a.Vit + b.Vit,
                Luc = a.Luc + b.Luc,
            };

        static StatusResistBonuses AddResists(StatusResistBonuses a, StatusResistBonuses b) =>
            new()
            {
                PoisonRes = a.PoisonRes + b.PoisonRes,
                SleepRes = a.SleepRes + b.SleepRes,
                PanicRes = a.PanicRes + b.PanicRes,
                BindHeadRes = a.BindHeadRes + b.BindHeadRes,
                BindArmRes = a.BindArmRes + b.BindArmRes,
                BindLegRes = a.BindLegRes + b.BindLegRes,
            };

        static CharacterBaseStats ToCharacterBaseStats(CombatantStats stats) =>
            new()
            {
                Hp = stats.Hp,
                Mp = stats.Mp,
                Str = stats.Str,
                Tec = stats.Tec,
                Agi = stats.Agi,
                Vit = stats.Vit,
                Luc = stats.Luc,
            };
    }

    static class EquipSlots
    {
        public static readonly EquipSlot[] All =
        {
            EquipSlot.Weapon,
            EquipSlot.Head,
            EquipSlot.Body,
            EquipSlot.Legs,
            EquipSlot.Accessory,
        };
    }
}
