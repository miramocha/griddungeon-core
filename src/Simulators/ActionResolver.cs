using System;
using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class ActionResolver
    {
        const string k_GuardModId = "guard";
        const float k_GuardDamageMultiplier = 0.5f;
        public const float SynchroGainPerCoreAction = 0.15f;

        public static CombatActionResult Resolve(
            CombatAction action,
            Combatant actor,
            BattleState state,
            IReadOnlyDictionary<string, SkillData> skills,
            IReadOnlyDictionary<string, StatusData> statuses,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances,
            Func<float>? rollPercent = null
        )
        {
            rollPercent ??= () => (float)new Random().NextDouble() * 100f;

            return action.Command switch
            {
                CombatCommand.Attack => ResolveAttack(
                    action,
                    actor,
                    state,
                    skills,
                    statuses,
                    enemyResistances,
                    rollPercent
                ),
                CombatCommand.Guard => ResolveGuard(actor),
                CombatCommand.Skill => ResolveSkill(
                    actor,
                    action,
                    state,
                    skills,
                    statuses,
                    enemyResistances,
                    rollPercent
                ),
                CombatCommand.Item => ResolveItem(actor, action),
                CombatCommand.Flee => ResolveFlee(actor, state, rollPercent),
                _ => new CombatActionResult { Actor = actor },
            };
        }

        static CombatActionResult ResolveAttack(
            CombatAction action,
            Combatant actor,
            BattleState state,
            IReadOnlyDictionary<string, SkillData> skills,
            IReadOnlyDictionary<string, StatusData> statuses,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances,
            Func<float> rollPercent
        )
        {
            SkillData skill = ResolveSkillData(
                CombatSkillDefaults.Attack.Id,
                skills,
                CombatSkillDefaults.Attack
            );
            bool targetEnemies = actor.Kind != CombatantKind.Enemy;
            Combatant? target = ResolveLivingTarget(
                state,
                action.TargetId,
                skill.Targeting,
                targetEnemies,
                actor,
                CombatSkillDefaults.Attack.Id
            );
            if (target == null)
            {
                return new CombatActionResult { Actor = actor };
            }
            return ApplyOffensiveSkill(
                actor,
                target,
                skill,
                statuses,
                enemyResistances,
                rollPercent,
                synchroGain: actor.Kind == CombatantKind.Core
            );
        }

        static CombatActionResult ResolveFlee(
            Combatant actor,
            BattleState state,
            Func<float> rollPercent
        )
        {
            bool succeeded =
                actor.Kind == CombatantKind.Core && FleeCalculator.RollSuccess(state, rollPercent);
            return new CombatActionResult { Actor = actor, FleeSucceeded = succeeded };
        }

        static CombatActionResult ResolveDeploy(Combatant actor, BattleState state, SkillData skill)
        {
            if (!SummonDeployResolver.CanDeploy(state, skill))
            {
                return new CombatActionResult { Actor = actor };
            }

            if (actor.CurrentMp < skill.MpCost)
            {
                return new CombatActionResult { Actor = actor };
            }

            actor.CurrentMp -= skill.MpCost;
            return new CombatActionResult
            {
                Actor = actor,
                Hit = true,
                DeploySummonId = skill.SummonDefinitionId,
                SynchroBarDelta = actor.Kind == CombatantKind.Core ? SynchroGainPerCoreAction : 0f,
            };
        }

        static CombatActionResult ResolveGuard(Combatant actor)
        {
            actor.BattleMods.RemoveAll(m => m.ModId == k_GuardModId);
            actor.BattleMods.Add(
                new BattleModifier
                {
                    ModId = k_GuardModId,
                    Magnitude = k_GuardDamageMultiplier,
                    TurnsRemaining = 1,
                }
            );

            return new CombatActionResult
            {
                Actor = actor,
                Target = actor,
                Hit = true,
                SynchroBarDelta = actor.Kind == CombatantKind.Core ? SynchroGainPerCoreAction : 0f,
            };
        }

        static CombatActionResult ResolveSkill(
            Combatant actor,
            CombatAction action,
            BattleState state,
            IReadOnlyDictionary<string, SkillData> skills,
            IReadOnlyDictionary<string, StatusData> statuses,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances,
            Func<float> rollPercent
        )
        {
            if (string.IsNullOrEmpty(action.SkillId))
            {
                return ResolveAttack(
                    action,
                    actor,
                    state,
                    skills,
                    statuses,
                    enemyResistances,
                    rollPercent
                );
            }

            SkillData skill = ResolveSkillData(action.SkillId, skills, CombatSkillDefaults.Attack);
            if ((skill.UseContexts & SkillUseContext.Combat) == 0)
            {
                return new CombatActionResult { Actor = actor };
            }

            if (StatusSystem.IsBlocked(actor, skill))
            {
                return new CombatActionResult { Actor = actor };
            }

            if (skill.Type == SkillType.Deploy)
            {
                return ResolveDeploy(actor, state, skill);
            }

            if (actor.CurrentMp < skill.MpCost)
            {
                return new CombatActionResult { Actor = actor };
            }

            actor.CurrentMp -= skill.MpCost;

            if (skill.Targeting.Kind is TargetKind.AllEnemies or TargetKind.AllAllies)
            {
                return ResolveAreaSkill(
                    actor,
                    action,
                    state,
                    skill,
                    statuses,
                    enemyResistances,
                    rollPercent
                );
            }

            return skill.Type switch
            {
                SkillType.Heal => ResolveHeal(actor, action, state, skill, statuses),
                SkillType.Buff or SkillType.Debuff => ResolveStatusSkill(
                    actor,
                    action,
                    state,
                    skill,
                    statuses
                ),
                _ => ResolveOffensiveSkillAction(
                    actor,
                    action,
                    state,
                    skill,
                    statuses,
                    enemyResistances,
                    rollPercent
                ),
            };
        }

        static CombatActionResult ResolveOffensiveSkillAction(
            Combatant actor,
            CombatAction action,
            BattleState state,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances,
            Func<float> rollPercent
        )
        {
            bool targetEnemies = actor.Kind != CombatantKind.Enemy;
            Combatant? target = ResolveLivingTarget(
                state,
                action.TargetId,
                skill.Targeting,
                targetEnemies,
                actor,
                action.SkillId
            );
            if (target == null)
            {
                return new CombatActionResult { Actor = actor };
            }

            return ApplyOffensiveSkill(
                actor,
                target,
                skill,
                statuses,
                enemyResistances,
                rollPercent,
                synchroGain: actor.Kind == CombatantKind.Core
            );
        }

        static CombatActionResult ResolveAreaSkill(
            Combatant actor,
            CombatAction action,
            BattleState state,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances,
            Func<float> rollPercent
        )
        {
            bool targetEnemies = actor.Kind != CombatantKind.Enemy;
            IReadOnlyList<Combatant> targets = ValidTargetCalculator.GetValidTargets(
                state,
                skill.Targeting,
                targetEnemies,
                actor,
                action.SkillId
            );
            if (targets.Count == 0)
            {
                return new CombatActionResult { Actor = actor };
            }

            var merged = new CombatActionResult { Actor = actor, Hit = true };
            bool synchroGain = actor.Kind == CombatantKind.Core;
            foreach (Combatant target in targets)
            {
                CombatActionResult hit = skill.Type switch
                {
                    SkillType.Heal => ResolveHealOnTarget(actor, target, skill),
                    SkillType.Buff or SkillType.Debuff => ResolveStatusOnTarget(
                        actor,
                        target,
                        skill,
                        statuses
                    ),
                    _ => ApplyOffensiveSkill(
                        actor,
                        target,
                        skill,
                        statuses,
                        enemyResistances,
                        rollPercent,
                        synchroGain
                    ),
                };
                merged.Target = target;
                merged.DamageDealt += hit.DamageDealt;
                merged.HealingDone += hit.HealingDone;
                merged.TargetDied |= hit.TargetDied;
                merged.SynchroBarDelta += hit.SynchroBarDelta;
                if (!string.IsNullOrEmpty(hit.StatusApplied))
                {
                    merged.StatusApplied = hit.StatusApplied;
                }
            }

            return merged;
        }

        static CombatActionResult ResolveHeal(
            Combatant actor,
            CombatAction action,
            BattleState state,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses
        )
        {
            Combatant? target =
                FindCombatant(state, action.TargetId)
                ?? FindLivingCombatant(state, action.TargetId)
                ?? actor;
            if (target == null)
            {
                return new CombatActionResult { Actor = actor };
            }

            if (skill.Id == Mvp1LockedSkillIds.MedicRevive)
            {
                return ResolveRevive(actor, target);
            }

            if (target.IsDead || target.IsDowned)
            {
                return new CombatActionResult { Actor = actor };
            }

            if (skill.Id == Mvp1LockedSkillIds.MedicPurify)
            {
                return ResolvePurify(actor, target, skill, statuses);
            }

            return ResolveHealOnTarget(actor, target, skill);
        }

        static CombatActionResult ResolvePurify(
            Combatant actor,
            Combatant target,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses
        )
        {
            int cleansed = StatusSystem.CleanseControlAndDoT(target, statuses);
            CombatActionResult result = ResolveHealOnTarget(actor, target, skill);
            if (cleansed > 0)
            {
                result.StatusCleansed = "control_dot";
            }

            return result;
        }

        static CombatActionResult ResolveRevive(Combatant actor, Combatant target)
        {
            if (!target.IsDowned)
            {
                return new CombatActionResult { Actor = actor };
            }

            int restored = Math.Max(1, (int)Math.Ceiling(target.Stats.Hp * 0.25f));
            target.CurrentHp = restored;
            return new CombatActionResult
            {
                Actor = actor,
                Target = target,
                Hit = true,
                HealingDone = restored,
                SynchroBarDelta = actor.Kind == CombatantKind.Core ? SynchroGainPerCoreAction : 0f,
            };
        }

        static CombatActionResult ResolveHealOnTarget(
            Combatant actor,
            Combatant target,
            SkillData skill
        )
        {
            int healing = DamageCalculator.CalculateHeal(actor, skill);
            int before = target.CurrentHp;
            target.CurrentHp = Math.Min(target.Stats.Hp, target.CurrentHp + healing);
            int done = target.CurrentHp - before;

            return new CombatActionResult
            {
                Actor = actor,
                Target = target,
                Hit = true,
                HealingDone = done,
                SynchroBarDelta = actor.Kind == CombatantKind.Core ? SynchroGainPerCoreAction : 0f,
            };
        }

        static CombatActionResult ResolveStatusSkill(
            Combatant actor,
            CombatAction action,
            BattleState state,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses
        )
        {
            bool targetEnemies = skill.Type == SkillType.Debuff;
            TargetingRule targeting = skill.Targeting;
            Combatant? target = ResolveLivingTarget(
                state,
                action.TargetId,
                targeting,
                targetEnemies,
                actor,
                action.SkillId
            );
            if (target == null || skill.Inflict == null)
            {
                return new CombatActionResult { Actor = actor };
            }

            return ResolveStatusOnTarget(actor, target, skill, statuses);
        }

        static CombatActionResult ResolveStatusOnTarget(
            Combatant actor,
            Combatant target,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses
        )
        {
            if (skill.Inflict == null)
            {
                return new CombatActionResult { Actor = actor };
            }

            if (skill.Id == Mvp1LockedSkillIds.VanguardProtect)
            {
                ApplyVanguardProtectGuard(target);
                return new CombatActionResult
                {
                    Actor = actor,
                    Target = target,
                    Hit = true,
                    StatusApplied = k_GuardModId,
                    SynchroBarDelta =
                        actor.Kind == CombatantKind.Core ? SynchroGainPerCoreAction : 0f,
                };
            }

            StatusInflict inflict = skill.Inflict.Value;
            if (!statuses.TryGetValue(inflict.StatusId, out StatusData def))
            {
                return new CombatActionResult { Actor = actor };
            }

            int duration =
                inflict.DurationOverride > 0 ? inflict.DurationOverride : def.DefaultDurationTurns;
            StatusSystem.Apply(
                target,
                new StatusInstance
                {
                    DefinitionId = inflict.StatusId,
                    TurnsRemaining = duration,
                    SourceCombatantId = actor.Id,
                },
                def
            );

            return new CombatActionResult
            {
                Actor = actor,
                Target = target,
                Hit = true,
                StatusApplied = inflict.StatusId,
                SynchroBarDelta = actor.Kind == CombatantKind.Core ? SynchroGainPerCoreAction : 0f,
            };
        }

        static void ApplyVanguardProtectGuard(Combatant target)
        {
            target.BattleMods.RemoveAll(m => m.ModId == k_GuardModId);
            target.BattleMods.Add(
                new BattleModifier
                {
                    ModId = k_GuardModId,
                    Magnitude = k_GuardDamageMultiplier,
                    TurnsRemaining = 1,
                }
            );
        }

        static CombatActionResult ResolveItem(Combatant actor, CombatAction action)
        {
            _ = action;
            const int k_StubHeal = 20;
            int before = actor.CurrentHp;
            actor.CurrentHp = Math.Min(actor.Stats.Hp, actor.CurrentHp + k_StubHeal);
            return new CombatActionResult
            {
                Actor = actor,
                Target = actor,
                Hit = true,
                HealingDone = actor.CurrentHp - before,
                SynchroBarDelta = SynchroGainPerCoreAction,
            };
        }

        static CombatActionResult ApplyOffensiveSkill(
            Combatant actor,
            Combatant target,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances,
            Func<float> rollPercent,
            bool synchroGain
        )
        {
            float hitChance = HitChanceCalculator.Calculate(actor, target);
            bool hit = rollPercent() <= hitChance;
            var result = new CombatActionResult
            {
                Actor = actor,
                Target = target,
                Hit = hit,
                SynchroBarDelta = synchroGain ? SynchroGainPerCoreAction : 0f,
            };

            if (!hit)
            {
                return result;
            }

            int damage =
                skill.Type == SkillType.Elemental
                    ? DamageCalculator.CalculateElemental(
                        actor,
                        target,
                        skill,
                        GetResistances(target, enemyResistances)
                    )
                    : DamageCalculator.CalculatePhysical(actor, target, skill, target.BattleMods);

            damage = ApplyGuardReduction(target, damage);
            target.CurrentHp = Math.Max(0, target.CurrentHp - damage);
            result.DamageDealt = damage;
            result.TargetDied = target.IsDead;

            TryInflictOnHit(actor, target, skill, statuses, result);
            RemoveGuardAfterHit(target);
            return result;
        }

        static void TryInflictOnHit(
            Combatant actor,
            Combatant target,
            SkillData skill,
            IReadOnlyDictionary<string, StatusData> statuses,
            CombatActionResult result
        )
        {
            if (target.IsDead || skill.Inflict == null)
            {
                return;
            }

            StatusInflict inflict = skill.Inflict.Value;
            if (!statuses.TryGetValue(inflict.StatusId, out StatusData def))
            {
                return;
            }

            int duration =
                inflict.DurationOverride > 0 ? inflict.DurationOverride : def.DefaultDurationTurns;
            StatusSystem.Apply(
                target,
                new StatusInstance
                {
                    DefinitionId = inflict.StatusId,
                    TurnsRemaining = duration,
                    SourceCombatantId = actor.Id,
                },
                def
            );
            result.StatusApplied = inflict.StatusId;
        }

        static int ApplyGuardReduction(Combatant target, int damage)
        {
            foreach (BattleModifier mod in target.BattleMods)
            {
                if (mod.ModId == k_GuardModId && mod.TurnsRemaining != 0)
                {
                    return Math.Max(1, (int)Math.Floor(damage * mod.Magnitude));
                }
            }

            return damage;
        }

        static void RemoveGuardAfterHit(Combatant target) =>
            target.BattleMods.RemoveAll(m => m.ModId == k_GuardModId);

        static ElementResistances GetResistances(
            Combatant target,
            IReadOnlyDictionary<string, ElementResistances>? enemyResistances
        )
        {
            if (
                enemyResistances != null
                && enemyResistances.TryGetValue(target.DefinitionId, out ElementResistances res)
            )
            {
                return res;
            }

            return new ElementResistances
            {
                Slash = 1f,
                Pierce = 1f,
                Fire = 1f,
                Ice = 1f,
                Volt = 1f,
            };
        }

        static SkillData ResolveSkillData(
            string skillId,
            IReadOnlyDictionary<string, SkillData> skills,
            SkillData fallback
        ) => skills.TryGetValue(skillId, out SkillData skill) ? skill : fallback;

        public static Combatant? PickDefaultTarget(BattleState state, bool targetEnemies)
        {
            if (targetEnemies)
            {
                return FirstLivingInRow(state.EnemySlots, FormationRow.Front)
                    ?? FirstLivingInRow(state.EnemySlots, FormationRow.Back);
            }

            return FirstLivingCore(state, FormationRow.Front)
                ?? FirstLivingCore(state, FormationRow.Back)
                ?? FirstLivingAux(state, FormationRow.Front)
                ?? FirstLivingAux(state, FormationRow.Back);
        }

        static Combatant? FirstLivingInRow(Combatant[] slots, FormationRow row)
        {
            foreach (Combatant c in slots)
            {
                if (c != null && c.Row == row && !c.IsDead)
                {
                    return c;
                }
            }

            return null;
        }

        static Combatant? FirstLivingCore(BattleState state, FormationRow row)
        {
            foreach (Combatant c in state.CoreSlots)
            {
                if (c != null && c.Row == row && !c.IsDead)
                {
                    return c;
                }
            }

            return null;
        }

        static Combatant? FirstLivingAux(BattleState state, FormationRow row)
        {
            int index = row == FormationRow.Front ? 0 : 1;
            Combatant? aux = state.AuxSlots[index];
            return aux != null && !aux.IsDead ? aux : null;
        }

        public static Combatant? FindCombatant(BattleState state, string? combatantId)
        {
            if (string.IsNullOrEmpty(combatantId))
            {
                return null;
            }

            foreach (Combatant c in state.CoreSlots)
            {
                if (c != null && c.Id == combatantId)
                {
                    return c;
                }
            }

            foreach (Combatant? aux in state.AuxSlots)
            {
                if (aux != null && aux.Id == combatantId)
                {
                    return aux;
                }
            }

            foreach (Combatant enemy in state.EnemySlots)
            {
                if (enemy != null && enemy.Id == combatantId)
                {
                    return enemy;
                }
            }

            return null;
        }

        static Combatant? FindLivingCombatant(BattleState state, string? combatantId)
        {
            Combatant? target = FindCombatant(state, combatantId);
            return target != null && !target.IsDead ? target : null;
        }

        /// <summary>
        /// Queued <see cref="CombatAction.TargetId"/> when still alive; otherwise first valid living
        /// target for the skill rule (AGI playback after an earlier kill).
        /// </summary>
        static Combatant? ResolveLivingTarget(
            BattleState state,
            string? targetId,
            TargetingRule rule,
            bool targetEnemies,
            Combatant? actor = null,
            string? skillId = null
        )
        {
            IReadOnlyList<Combatant> valid = ValidTargetCalculator.GetValidTargets(
                state,
                rule,
                targetEnemies,
                actor,
                skillId
            );

            if (!string.IsNullOrEmpty(targetId))
            {
                for (int i = 0; i < valid.Count; i++)
                {
                    if (valid[i].Id == targetId)
                    {
                        return valid[i];
                    }
                }
            }

            return valid.Count > 0 ? valid[0] : null;
        }
    }
}
