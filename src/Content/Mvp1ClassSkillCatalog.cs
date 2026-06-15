using System;
using System.Collections.Generic;

namespace GridDungeon.Core.Content
{
    /// <summary>Default MVP1 skill kit per class (design-docs mvp1-class-skills.md).</summary>
    public static class Mvp1ClassSkillCatalog
    {
        public static string[] GetDefaultSkillIds(string classId) =>
            classId switch
            {
                "vanguard" => new[]
                {
                    "vanguard_guard",
                    "vanguard_shield_bash",
                    "vanguard_protect",
                },
                "breaker" => new[]
                {
                    "breaker_power_slash",
                    "breaker_cleave",
                    "breaker_pierce_drive",
                },
                "medic" => new[] { "medic_heal", "medic_purify", "medic_revive" },
                "summoner" => new[]
                {
                    "summoner_volt_bolt",
                    "deploy_scout_drone",
                    "summoner_focus",
                },
                "marksman" => new[]
                {
                    "marksman_aimed_shot",
                    "marksman_bind_shot",
                    "marksman_volley",
                },
                "tactician" => new[]
                {
                    "tactician_rally",
                    "tactician_weaken",
                    "tactician_field_mend",
                },
                _ => Array.Empty<string>(),
            };

        public static bool ClassHasDeploySkill(string classId) => classId == "summoner";
    }
}
