using GridDungeon.Core.Models;

namespace GridDungeon.Core.SaveData
{
    public static class CharacterSaveVitals
    {
        public const int Unset = -1;

        public static void ApplySnapshot(CharacterSaveData save, Combatant combatant)
        {
            save.VitalsSerialized = true;
            save.CurrentHp = combatant.CurrentHp;
            save.CurrentMp = combatant.CurrentMp;
        }

        public static (int hp, int mp) Resolve(CharacterSaveData save, CombatantStats stats)
        {
            int hp =
                save.CurrentHp == Unset
                    ? stats.Hp
                    : System.Math.Min(stats.Hp, System.Math.Max(0, save.CurrentHp));
            int mp =
                save.CurrentMp == Unset
                    ? stats.Mp
                    : System.Math.Min(stats.Mp, System.Math.Max(0, save.CurrentMp));
            return (hp, mp);
        }

        /// <summary>JsonUtility omits missing ints as 0; pre-vitals saves had no HP/MP fields.</summary>
        public static void NormalizeLegacyJsonFields(CharacterSaveData save)
        {
            if (!save.VitalsSerialized && save.CurrentHp == 0 && save.CurrentMp == 0)
            {
                save.CurrentHp = Unset;
                save.CurrentMp = Unset;
            }
        }
    }
}
