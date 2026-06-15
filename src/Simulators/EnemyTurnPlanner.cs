using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>Default enemy AGI action — first usable authored skill, else basic attack.</summary>
    public static class EnemyTurnPlanner
    {
        public static CombatAction Plan(
            Combatant enemy,
            BattleState state,
            IReadOnlyList<string> authoredSkillIds,
            IReadOnlyDictionary<string, SkillData> skills
        )
        {
            if (enemy == null || state == null || skills == null)
            {
                return new CombatAction { Command = CombatCommand.Attack };
            }

            if (authoredSkillIds != null)
            {
                foreach (string skillId in authoredSkillIds)
                {
                    if (
                        string.IsNullOrEmpty(skillId)
                        || !skills.TryGetValue(skillId, out SkillData skill)
                    )
                    {
                        continue;
                    }

                    IReadOnlyList<Combatant> targets = ValidTargetCalculator.GetValidTargets(
                        state,
                        skill.Targeting,
                        actorTargetsEnemies: false,
                        enemy,
                        skillId
                    );
                    if (targets.Count == 0)
                    {
                        continue;
                    }

                    return new CombatAction
                    {
                        Command = CombatCommand.Skill,
                        SkillId = skillId,
                        TargetId = targets[0].Id,
                    };
                }
            }

            IReadOnlyList<Combatant> attackTargets = ValidTargetCalculator.GetValidTargets(
                state,
                CombatSkillDefaults.Attack.Targeting,
                actorTargetsEnemies: false,
                enemy
            );
            return new CombatAction
            {
                Command = CombatCommand.Attack,
                TargetId = attackTargets.Count > 0 ? attackTargets[0].Id : null,
            };
        }
    }
}
