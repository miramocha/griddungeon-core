using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class HitChanceCalculator
    {
        public static float Calculate(Combatant attacker, Combatant defender, float blindMod = 0f)
        {
            const float baseHit = 80f;
            float value =
                baseHit + attacker.Stats.Agi * 0.5f - defender.Stats.Agi * 0.3f + blindMod;
            return System.Math.Clamp(value, 5f, 95f);
        }
    }
}
