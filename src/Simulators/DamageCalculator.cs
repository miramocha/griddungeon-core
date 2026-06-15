using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class DamageCalculator
    {
        public static int CalculatePhysical(
            Combatant attacker,
            Combatant defender,
            SkillData skill,
            IReadOnlyList<BattleModifier> mods
        )
        {
            _ = mods;
            float raw =
                (skill.Power + attacker.Stats.Str * 0.5f)
                * CombatStatModifiers.GetPhysicalOffenseMultiplier(attacker);
            float mitigated =
                raw
                * (100f / (100f + defender.Stats.Vit))
                * CombatStatModifiers.GetPhysicalDefenseMultiplier(defender);
            return System.Math.Max(1, (int)System.Math.Floor(mitigated));
        }

        public static int CalculateElemental(
            Combatant attacker,
            Combatant defender,
            SkillData skill,
            ElementResistances resistances
        )
        {
            float raw = skill.Power + attacker.Stats.Tec * 0.3f;
            float mitigated = raw * (100f / (100f + defender.Stats.Tec));
            float mult = skill.Element switch
            {
                Enums.DamageElement.Slash => resistances.Slash,
                Enums.DamageElement.Pierce => resistances.Pierce,
                Enums.DamageElement.Fire => resistances.Fire,
                Enums.DamageElement.Ice => resistances.Ice,
                Enums.DamageElement.Volt => resistances.Volt,
                _ => 1f,
            };
            return System.Math.Max(1, (int)System.Math.Floor(mitigated * mult));
        }

        public static int CalculateHeal(Combatant caster, SkillData skill) =>
            (int)System.Math.Floor(skill.Power + caster.Stats.Tec * 0.4f);
    }
}
