using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Simulators
{
    public static class CombatSkillDefaults
    {
        public static readonly SkillData Attack = new(
            "attack",
            SkillType.Physical,
            DamageElement.Slash,
            BodyPart.None,
            0,
            10f,
            new TargetingRule
            {
                Kind = TargetKind.SingleEnemy,
                CanTargetBack = true,
                Pierce = false,
            },
            null,
            "Attack",
            descriptionEn: "Strike one enemy with a standard attack."
        );
    }
}
