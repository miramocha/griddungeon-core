using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>Resolves whether a player command needs an interactive target pick.</summary>
    public static class CombatTargeting
    {
        public static bool RequiresPlayerTarget(
            CombatAction action,
            IReadOnlyDictionary<string, SkillData> skills
        )
        {
            switch (action.Command)
            {
                case CombatCommand.Attack:
                    return true;
                case CombatCommand.Guard:
                case CombatCommand.Flee:
                case CombatCommand.Protocol:
                case CombatCommand.Item:
                    return false;
                case CombatCommand.Skill:
                    TargetingRule rule = ResolveRule(action, skills);
                    if (
                        skills.TryGetValue(action.SkillId ?? string.Empty, out SkillData skill)
                        && skill.Type == SkillType.Deploy
                    )
                    {
                        return false;
                    }

                    return rule.Kind is TargetKind.SingleEnemy or TargetKind.SingleAlly;
                default:
                    return false;
            }
        }

        public static TargetingRule ResolveRule(
            CombatAction action,
            IReadOnlyDictionary<string, SkillData> skills
        )
        {
            if (action.Command == CombatCommand.Attack)
            {
                return CombatSkillDefaults.Attack.Targeting;
            }

            if (action.Command != CombatCommand.Skill || string.IsNullOrEmpty(action.SkillId))
            {
                return CombatSkillDefaults.Attack.Targeting;
            }

            return skills.TryGetValue(action.SkillId, out SkillData skill)
                ? skill.Targeting
                : CombatSkillDefaults.Attack.Targeting;
        }

        public static bool ActorTargetsEnemies(TargetingRule rule, CombatantKind actorKind)
        {
            if (rule.Kind == TargetKind.SingleEnemy)
            {
                return true;
            }

            if (rule.Kind == TargetKind.SingleAlly)
            {
                return false;
            }

            return actorKind == CombatantKind.Enemy;
        }

        /// <summary>
        /// Queued <see cref="CombatAction.TargetId"/> is dead or no longer valid for the skill rule
        /// (AGI will retarget at resolve time).
        /// </summary>
        public static bool IsQueuedTargetStale(
            BattleState state,
            CombatAction action,
            IReadOnlyDictionary<string, SkillData> skills,
            Combatant? queuedBy = null
        )
        {
            if (!RequiresPlayerTarget(action, skills) || string.IsNullOrEmpty(action.TargetId))
            {
                return false;
            }

            Combatant? target = ActionResolver.FindCombatant(state, action.TargetId);
            if (target == null || target.IsDead)
            {
                return true;
            }

            TargetingRule rule = ResolveRule(action, skills);
            CombatantKind actorKind = queuedBy?.Kind ?? CombatantKind.Core;
            bool targetEnemies = ActorTargetsEnemies(rule, actorKind);
            string? skillId = action.Command == CombatCommand.Skill ? action.SkillId : null;
            IReadOnlyList<Combatant> valid = ValidTargetCalculator.GetValidTargets(
                state,
                rule,
                targetEnemies,
                queuedBy,
                skillId
            );
            for (int i = 0; i < valid.Count; i++)
            {
                if (valid[i].Id == action.TargetId)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
