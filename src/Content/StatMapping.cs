using GridDungeon.Core.Models;

namespace GridDungeon.Core.Content
{
    public static class StatMapping
    {
        public static CombatantStats ToCombatantStats(CharacterBaseStats stats) =>
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
}
