using System;
using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class ProtocolResolver
    {
        public const float FullBar = 1f;
        const float k_BarEpsilon = 1e-5f;

        public static bool IsBarFull(float synchroBar) => synchroBar >= FullBar - k_BarEpsilon;

        public static float AdditionalNavigatorGain(float baseDelta, AuraModifiers aura) =>
            baseDelta * aura.SynchroGainBonus;

        public static int CountLivingCore(BattleState state)
        {
            int count = 0;
            foreach (Combatant? core in state.CoreSlots)
            {
                if (core != null && !core.IsDead)
                {
                    count++;
                }
            }

            return count;
        }

        public static bool IsSkillInNavigatorKit(
            string protocolSkillId,
            string[] navigatorProtocolSkillIds
        )
        {
            if (string.IsNullOrEmpty(protocolSkillId) || navigatorProtocolSkillIds == null)
            {
                return false;
            }

            foreach (string allowedId in navigatorProtocolSkillIds)
            {
                if (allowedId == protocolSkillId)
                {
                    return true;
                }
            }

            return false;
        }

        public static ProtocolResolveResult Resolve(ProtocolSkillData skill, BattleState state)
        {
            List<Combatant> participants = CollectParticipants(state, skill.ParticipantCount);
            if (participants.Count == 0)
            {
                return new ProtocolResolveResult();
            }

            return skill.EffectType switch
            {
                ProtocolEffectType.DamageAllEnemies => ResolveStrike(skill, state, participants),
                ProtocolEffectType.HealAllAllies => ResolveMend(skill, state, participants),
                _ => new ProtocolResolveResult(),
            };
        }

        static ProtocolResolveResult ResolveStrike(
            ProtocolSkillData skill,
            BattleState state,
            List<Combatant> participants
        )
        {
            Combatant lead = PickLeadParticipant(participants);
            SkillData strikeSkill = ToStrikeSkill(skill, participants);
            var results = new List<CombatActionResult>();

            foreach (Combatant enemy in state.EnemySlots)
            {
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                int damage = DamageCalculator.CalculatePhysical(
                    lead,
                    enemy,
                    strikeSkill,
                    enemy.BattleMods
                );
                enemy.CurrentHp = Math.Max(0, enemy.CurrentHp - damage);
                results.Add(
                    new CombatActionResult
                    {
                        Actor = lead,
                        Target = enemy,
                        Hit = true,
                        DamageDealt = damage,
                        TargetDied = enemy.IsDead,
                    }
                );
            }

            return new ProtocolResolveResult { TargetResults = results };
        }

        static ProtocolResolveResult ResolveMend(
            ProtocolSkillData skill,
            BattleState state,
            List<Combatant> participants
        )
        {
            Combatant lead = PickLeadParticipant(participants);
            SkillData mendSkill = ToHealSkill(skill);
            var results = new List<CombatActionResult>();

            foreach (Combatant? core in state.CoreSlots)
            {
                if (core == null || core.IsDead)
                {
                    continue;
                }

                int healing = DamageCalculator.CalculateHeal(lead, mendSkill);
                int hpBefore = core.CurrentHp;
                core.CurrentHp = Math.Min(core.Stats.Hp, core.CurrentHp + healing);
                int done = core.CurrentHp - hpBefore;
                results.Add(
                    new CombatActionResult
                    {
                        Actor = lead,
                        Target = core,
                        Hit = true,
                        HealingDone = done,
                    }
                );
            }

            return new ProtocolResolveResult { TargetResults = results };
        }

        static List<Combatant> CollectParticipants(BattleState state, int maxCount)
        {
            var living = new List<Combatant>();
            foreach (Combatant? core in state.CoreSlots)
            {
                if (core != null && !core.IsDead)
                {
                    living.Add(core);
                }
            }

            int take = Math.Min(maxCount, living.Count);
            return living.Count <= take ? living : living.GetRange(0, take);
        }

        static Combatant PickLeadParticipant(List<Combatant> participants)
        {
            Combatant lead = participants[0];
            for (int i = 1; i < participants.Count; i++)
            {
                if (participants[i].Stats.Str > lead.Stats.Str)
                {
                    lead = participants[i];
                }
            }

            return lead;
        }

        static SkillData ToStrikeSkill(ProtocolSkillData skill, List<Combatant> participants)
        {
            float strBonus = 0f;
            foreach (Combatant p in participants)
            {
                strBonus += p.Stats.Str * 0.5f;
            }

            return new SkillData(
                skill.Id,
                SkillType.Physical,
                DamageElement.Slash,
                BodyPart.None,
                0,
                skill.Power + strBonus,
                new TargetingRule
                {
                    Kind = TargetKind.AllEnemies,
                    CanTargetBack = true,
                    Pierce = false,
                },
                null
            );
        }

        static SkillData ToHealSkill(ProtocolSkillData skill) =>
            new(
                skill.Id,
                SkillType.Heal,
                DamageElement.None,
                BodyPart.None,
                0,
                skill.Power,
                new TargetingRule { Kind = TargetKind.AllAllies },
                null
            );
    }

    public sealed class ProtocolResolveResult
    {
        public IReadOnlyList<CombatActionResult> TargetResults { get; set; } =
            Array.Empty<CombatActionResult>();
    }
}
