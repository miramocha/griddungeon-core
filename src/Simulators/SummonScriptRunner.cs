using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class SummonScriptRunner
    {
        public static CombatAction ResolveNext(SummonData summon, int turnIndex, BattleState state)
        {
            _ = state;
            SummonAction[] script = summon.ActionScript;
            if (script == null || script.Length == 0)
            {
                return new CombatAction { Command = CombatCommand.Attack };
            }

            SummonAction step = script[turnIndex % script.Length];
            return new CombatAction { Command = CombatCommand.Skill, SkillId = step.SkillId };
        }
    }
}
