namespace GridDungeon.Core.Content
{
    /// <summary>
    /// Locked MVP1 content IDs (design-docs 05-class-design-mvp1, mvp1-enemy-roster, mvp1-class-skills).
    /// Used by ContentDB authoring and Edit Mode validation (#12).
    /// </summary>
    public static class Mvp1LockedContentRegistry
    {
        public static readonly string[] EnemyIds =
        {
            "stray_hound",
            "rust_mite",
            "gutter_crow",
            "scrapling",
            "shackle_rat",
            "venom_slime",
            "alley_thug",
            "rubble_guard",
            "s1_warden",
        };

        public static readonly string[] EnemySkillIds =
        {
            "enemy_attack",
            "atk_peck_volt",
            "atk_bind_arm",
            "atk_poison_spit",
            "atk_heavy_swing",
            "atk_guard_slam",
            "atk_warden_bind",
            "atk_warden_venom",
        };

        public static readonly string[] ClassSkillIds =
        {
            "vanguard_guard",
            "vanguard_shield_bash",
            "vanguard_protect",
            "breaker_power_slash",
            "breaker_cleave",
            "breaker_pierce_drive",
            "medic_heal",
            "medic_purify",
            "medic_revive",
            "summoner_volt_bolt",
            "deploy_scout_drone",
            "summoner_focus",
            "marksman_aimed_shot",
            "marksman_bind_shot",
            "marksman_volley",
            "tactician_rally",
            "tactician_weaken",
            "tactician_field_mend",
            "volt_burst",
        };

        public static readonly string[] EncounterGroupIds =
        {
            "grp_alley_stalker",
            "grp_alley_stalker_tutorial",
            "grp_s1_warden",
            "grp_b1_chaff_hound",
            "grp_b1_chaff_mite",
            "grp_b2_chaff",
            "grp_b2_shackle_rat",
            "grp_b2_venom_slime",
            "grp_b3_mix_hounds",
            "grp_b3_rubble_pair",
            "grp_b3_control",
        };

        public const string LootChaffConsumable = "loot_chaff_consumable";
        public const string LootS1Warden = "loot_s1_warden";
        public const string LootGatherPatchKit = "loot_gather_patch_kit";

        public static readonly string[] LootTableIds =
        {
            LootChaffConsumable,
            LootS1Warden,
            LootGatherPatchKit,
        };

        public static readonly string[] ItemIds =
        {
            "patch_kit",
            "stim_draft",
            "trauma_kit",
            "return_thread",
            "analysis_glass",
        };

        public static readonly string[] EquipmentIds =
        {
            "guild_shortsword",
            "leather_coif",
            "leather_jacket",
            "leather_boots",
            "scout_charm",
        };

        public static readonly string[] StatusIds =
        {
            "poison",
            "bind_arm",
            "bind_head",
            "offense_up",
            "offense_down",
            "defense_up",
            "magic_up",
        };

        public const string ScoutDroneSummonId = "scout_drone";
    }
}
