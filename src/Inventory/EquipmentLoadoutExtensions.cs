using System;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Inventory
{
    public static class EquipmentLoadoutExtensions
    {
        public static string GetEquipId(this EquipmentLoadout loadout, EquipSlot slot)
        {
            if (loadout == null)
            {
                throw new ArgumentNullException(nameof(loadout));
            }

            return slot switch
            {
                EquipSlot.Weapon => loadout.WeaponId,
                EquipSlot.Head => loadout.HeadId,
                EquipSlot.Body => loadout.BodyId,
                EquipSlot.Legs => loadout.LegsId,
                EquipSlot.Accessory => loadout.AccessoryId,
                _ => string.Empty,
            };
        }

        public static void SetEquipId(this EquipmentLoadout loadout, EquipSlot slot, string equipId)
        {
            if (loadout == null)
            {
                throw new ArgumentNullException(nameof(loadout));
            }

            equipId ??= string.Empty;
            switch (slot)
            {
                case EquipSlot.Weapon:
                    loadout.WeaponId = equipId;
                    break;
                case EquipSlot.Head:
                    loadout.HeadId = equipId;
                    break;
                case EquipSlot.Body:
                    loadout.BodyId = equipId;
                    break;
                case EquipSlot.Legs:
                    loadout.LegsId = equipId;
                    break;
                case EquipSlot.Accessory:
                    loadout.AccessoryId = equipId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        public static bool IsSlotEmpty(this EquipmentLoadout loadout, EquipSlot slot) =>
            string.IsNullOrEmpty(loadout.GetEquipId(slot));

        public static EquipmentLoadout CloneLoadout(this EquipmentLoadout? loadout)
        {
            if (loadout == null)
            {
                return new EquipmentLoadout();
            }

            return new EquipmentLoadout
            {
                WeaponId = loadout.WeaponId,
                HeadId = loadout.HeadId,
                BodyId = loadout.BodyId,
                LegsId = loadout.LegsId,
                AccessoryId = loadout.AccessoryId,
            };
        }
    }
}
