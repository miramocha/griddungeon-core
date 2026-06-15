using System.Collections.Generic;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Content
{
    /// <summary>MVP1 skill/summon stubs until ContentDB assets (#12).</summary>
    public static class Mvp1CombatContentStubs
    {
        public static void RegisterSkills(IDictionary<string, SkillData> skills)
        {
            TargetingRule singleEnemyBack = new()
            {
                Kind = TargetKind.SingleEnemy,
                CanTargetBack = true,
                Pierce = false,
            };

            skills[Mvp1LockedSkillIds.DeployScoutDrone] = new SkillData(
                Mvp1LockedSkillIds.DeployScoutDrone,
                SkillType.Deploy,
                DamageElement.None,
                BodyPart.None,
                8,
                0f,
                new TargetingRule { Kind = TargetKind.AuxBack },
                null,
                "Deploy Scout Drone",
                summonDefinitionId: Mvp1LockedSkillIds.ScoutDroneSummonId
            );

            skills[Mvp1LockedSkillIds.VoltBurst] = new SkillData(
                Mvp1LockedSkillIds.VoltBurst,
                SkillType.Elemental,
                DamageElement.Volt,
                BodyPart.None,
                6,
                14f,
                singleEnemyBack,
                null,
                "Volt Burst"
            );

            skills["summoner_volt_bolt"] = new SkillData(
                "summoner_volt_bolt",
                SkillType.Elemental,
                DamageElement.Volt,
                BodyPart.None,
                5,
                12f,
                singleEnemyBack,
                null,
                "Volt Bolt"
            );

            skills["summoner_focus"] = new SkillData(
                "summoner_focus",
                SkillType.Buff,
                DamageElement.None,
                BodyPart.None,
                4,
                0f,
                new TargetingRule { Kind = TargetKind.Self },
                new StatusInflict
                {
                    StatusId = "magic_up",
                    Chance = 100f,
                    DurationOverride = 2,
                },
                "Focus"
            );
        }

        public static SummonData ScoutDrone =>
            new(
                Mvp1LockedSkillIds.ScoutDroneSummonId,
                new CharacterBaseStats
                {
                    Hp = 28,
                    Mp = 20,
                    Str = 6,
                    Tec = 10,
                    Agi = 9,
                    Vit = 6,
                    Luc = 5,
                },
                3,
                FormationRow.Back,
                System.Array.Empty<SummonAction>(),
                "Scout Drone",
                new[] { Mvp1LockedSkillIds.VoltBurst }
            );
    }
}
