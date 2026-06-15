using System;
using GridDungeon.Core.Content;
using GridDungeon.Core.Hub;
using GridDungeon.Core.Inventory;
using GridDungeon.Core.Models;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Progression
{
    /// <summary>MVP1 XP curve and level-up stat growth (character-progression.md).</summary>
    public static class CharacterProgression
    {
        public const int MaxMvp1Level = 12;

        /// <summary>FOE contact fights grant bonus XP vs random ambush (design tuning stub).</summary>
        public const float FoeContactXpMultiplier = 1.25f;

        public static int GetTotalXpRequiredForLevel(int level)
        {
            if (level <= 1)
            {
                return 0;
            }

            return 30 * (level - 1) * level / 2;
        }

        public static int GetLevelForTotalXp(int totalXp)
        {
            int level = 1;
            while (level < MaxMvp1Level && totalXp >= GetTotalXpRequiredForLevel(level + 1))
            {
                level++;
            }

            return level;
        }

        /// <summary>Level from XP when present; otherwise saved level (matches combatant builders).</summary>
        public static int ResolveLevel(CharacterSaveData save)
        {
            if (save == null)
            {
                return 1;
            }

            if (save.Experience > 0)
            {
                return GetLevelForTotalXp(save.Experience);
            }

            return Math.Max(1, save.Level);
        }

        /// <summary>Party roster line: cumulative XP toward the next level (MVP1 curve).</summary>
        public static string FormatXpProgressLine(int totalExperience, int level = 0)
        {
            int xp = Math.Max(0, totalExperience);
            int resolvedLevel = level > 0 ? Math.Max(1, level) : GetLevelForTotalXp(xp);
            if (resolvedLevel >= MaxMvp1Level)
            {
                return $"XP {xp}";
            }

            int nextThreshold = GetTotalXpRequiredForLevel(resolvedLevel + 1);
            return $"XP {xp}/{nextThreshold}";
        }

        /// <summary>XP fill within the current level band for roster meters (0 at band start).</summary>
        public static bool TryGetXpLevelBandProgress(
            int totalExperience,
            int level,
            out float value,
            out float highValue
        )
        {
            int xp = Math.Max(0, totalExperience);
            int resolvedLevel = level > 0 ? Math.Max(1, level) : GetLevelForTotalXp(xp);
            if (resolvedLevel >= MaxMvp1Level)
            {
                value = 0f;
                highValue = 1f;
                return false;
            }

            int bandStart = GetTotalXpRequiredForLevel(resolvedLevel);
            int bandEnd = GetTotalXpRequiredForLevel(resolvedLevel + 1);
            value = xp - bandStart;
            highValue = Math.Max(1, bandEnd - bandStart);
            return true;
        }

        public static CharacterBaseStats GetClassBaseStatsAtLevel(string classId, int level)
        {
            CharacterBaseStats template = Mvp1GuildRosterFactory.GetTemplateStats(classId);
            int bonusLevels = Math.Max(0, level - 1);
            CharacterBaseStats growth = GetPerLevelGrowth(classId);
            return new CharacterBaseStats
            {
                Hp = template.Hp + growth.Hp * bonusLevels,
                Mp = template.Mp + growth.Mp * bonusLevels,
                Str = template.Str + growth.Str * bonusLevels,
                Tec = template.Tec + growth.Tec * bonusLevels,
                Agi = template.Agi + growth.Agi * bonusLevels,
                Vit = template.Vit + growth.Vit * bonusLevels,
                Luc = template.Luc + growth.Luc * bonusLevels,
            };
        }

        public static CombatantStats GetCombatStatsAtLevel(string classId, int level) =>
            StatMapping.ToCombatantStats(GetClassBaseStatsAtLevel(classId, level));

        public static ProgressionApplyResult AddExperience(
            CharacterSaveData member,
            int xpGain,
            string classId,
            IEquipmentStatSource? statSource = null
        )
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            int previousLevel = Math.Max(1, member.Level);
            member.Experience = Math.Max(0, member.Experience + Math.Max(0, xpGain));
            member.Level = GetLevelForTotalXp(member.Experience);
            int levelsGained = member.Level - previousLevel;
            if (levelsGained > 0)
            {
                member.AllocatedSkillPoints += levelsGained;
                HealToMaxAfterLevelUp(member, classId, statSource);
            }

            return new ProgressionApplyResult(xpGain, levelsGained, member.Level);
        }

        public static void ApplySaveProgressionToCombatant(
            Combatant combatant,
            CharacterSaveData save,
            IEquipmentStatSource? statSource = null
        )
        {
            if (combatant == null || save == null || string.IsNullOrEmpty(save.ClassId))
            {
                return;
            }

            int level = ResolveLevel(save);
            combatant.Level = level;
            save.Level = level;
            combatant.Experience = Math.Max(0, save.Experience);
            CombatantEquipmentBuild.ApplyEffectiveStats(combatant, save, statSource);

            if (!save.VitalsSerialized)
            {
                (int hp, int mp) vitals = CharacterSaveVitals.Resolve(save, combatant.Stats);
                combatant.CurrentHp = vitals.hp;
                combatant.CurrentMp = vitals.mp;
            }
        }

        static void HealToMaxAfterLevelUp(
            CharacterSaveData member,
            string classId,
            IEquipmentStatSource? statSource = null
        )
        {
            EquipmentLoadout loadout = member.Equipment?.CloneLoadout() ?? new EquipmentLoadout();
            CombatantStats stats = CombatantEquipmentBuild.BuildEffectiveStats(
                classId,
                member.Level,
                loadout,
                statSource
            );
            member.CurrentHp = stats.Hp;
            member.CurrentMp = stats.Mp;
            member.VitalsSerialized = true;
        }

        static CharacterBaseStats GetPerLevelGrowth(string classId) =>
            classId switch
            {
                "vanguard" => new CharacterBaseStats
                {
                    Hp = 10,
                    Mp = 2,
                    Str = 2,
                    Vit = 2,
                },
                "breaker" => new CharacterBaseStats
                {
                    Hp = 8,
                    Mp = 2,
                    Str = 2,
                    Agi = 1,
                },
                "medic" => new CharacterBaseStats
                {
                    Hp = 6,
                    Mp = 4,
                    Tec = 2,
                },
                "summoner" => new CharacterBaseStats
                {
                    Hp = 6,
                    Mp = 4,
                    Tec = 2,
                },
                "marksman" => new CharacterBaseStats
                {
                    Hp = 7,
                    Mp = 2,
                    Agi = 2,
                    Str = 1,
                },
                "tactician" => new CharacterBaseStats
                {
                    Hp = 7,
                    Mp = 3,
                    Tec = 1,
                    Luc = 1,
                },
                _ => new CharacterBaseStats
                {
                    Hp = 8,
                    Mp = 2,
                    Str = 1,
                    Vit = 1,
                },
            };
    }

    public readonly struct ProgressionApplyResult
    {
        public int XpGained { get; }
        public int LevelsGained { get; }
        public int NewLevel { get; }

        public ProgressionApplyResult(int xpGained, int levelsGained, int newLevel)
        {
            XpGained = xpGained;
            LevelsGained = levelsGained;
            NewLevel = newLevel;
        }
    }
}
