using System;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;
using GridDungeon.Core.SaveData;

namespace GridDungeon.Core.Hub
{
    public static class Mvp1GuildRosterFactory
    {
        public static CharacterSaveData[] EnsureDayOneRoster(CharacterSaveData[]? existing)
        {
            if (existing != null && existing.Length >= Mvp1HubConstants.DayOneClassIds.Length)
            {
                return existing;
            }

            var roster = new CharacterSaveData[Mvp1HubConstants.DayOneClassIds.Length];
            for (int i = 0; i < Mvp1HubConstants.DayOneClassIds.Length; i++)
            {
                string classId = Mvp1HubConstants.DayOneClassIds[i];
                roster[i] = CreatePremadeMember(classId, i);
            }

            return roster;
        }

        public static CharacterSaveData CreatePremadeMember(string classId, int index) =>
            new CharacterSaveData
            {
                CharacterId = $"guild_{classId}_{index}",
                Name = GetDisplayName(classId),
                ClassId = classId,
                Level = 1,
                AllocatedSkillPoints = 0,
                AllocatedSkillIds = Mvp1ClassSkillCatalog.GetDefaultSkillIds(classId),
            };

        /// <summary>Player-facing label from roster save or class id (ignores legacy guild_* id names).</summary>
        public static string ResolveMemberDisplayName(string savedName, string classId)
        {
            if (
                !string.IsNullOrWhiteSpace(savedName)
                && !savedName.StartsWith("guild_", StringComparison.Ordinal)
            )
            {
                return savedName.Trim();
            }

            return GetDisplayName(classId);
        }

        static string GetDisplayName(string classId) =>
            classId switch
            {
                "vanguard" => "Vanguard",
                "breaker" => "Breaker",
                "medic" => "Medic",
                "summoner" => "Summoner",
                "marksman" => "Marksman",
                "tactician" => "Tactician",
                _ => classId,
            };

        public static FormationRow GetPreferredRow(string classId) =>
            classId is "vanguard" or "breaker" ? FormationRow.Front : FormationRow.Back;

        public static CharacterBaseStats GetTemplateStats(string classId) =>
            classId switch
            {
                "vanguard" => new CharacterBaseStats
                {
                    Hp = 95,
                    Mp = 18,
                    Str = 12,
                    Tec = 6,
                    Agi = 8,
                    Vit = 14,
                    Luc = 5,
                },
                "breaker" => new CharacterBaseStats
                {
                    Hp = 82,
                    Mp = 20,
                    Str = 14,
                    Tec = 7,
                    Agi = 10,
                    Vit = 10,
                    Luc = 6,
                },
                "medic" => new CharacterBaseStats
                {
                    Hp = 68,
                    Mp = 32,
                    Str = 6,
                    Tec = 12,
                    Agi = 9,
                    Vit = 8,
                    Luc = 7,
                },
                "summoner" => new CharacterBaseStats
                {
                    Hp = 70,
                    Mp = 30,
                    Str = 7,
                    Tec = 13,
                    Agi = 8,
                    Vit = 8,
                    Luc = 6,
                },
                "marksman" => new CharacterBaseStats
                {
                    Hp = 72,
                    Mp = 22,
                    Str = 10,
                    Tec = 9,
                    Agi = 13,
                    Vit = 8,
                    Luc = 8,
                },
                "tactician" => new CharacterBaseStats
                {
                    Hp = 74,
                    Mp = 26,
                    Str = 7,
                    Tec = 11,
                    Agi = 10,
                    Vit = 9,
                    Luc = 9,
                },
                _ => new CharacterBaseStats
                {
                    Hp = 70,
                    Mp = 20,
                    Str = 8,
                    Tec = 8,
                    Agi = 8,
                    Vit = 8,
                    Luc = 5,
                },
            };

        /// <summary>Copies guild/party skill allocation from save onto a runtime core combatant.</summary>
        public static void ApplyAllocatedSkillsToCombatant(
            Combatant combatant,
            CharacterSaveData save
        )
        {
            if (combatant == null)
            {
                throw new ArgumentNullException(nameof(combatant));
            }

            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            combatant.AllocatedSkillIds.Clear();
            foreach (string skillId in ResolveAllocatedSkillIds(save))
            {
                if (!string.IsNullOrEmpty(skillId))
                {
                    combatant.AllocatedSkillIds.Add(skillId);
                }
            }
        }

        static string[] ResolveAllocatedSkillIds(CharacterSaveData save)
        {
            if (save.AllocatedSkillIds is { Length: > 0 })
            {
                return save.AllocatedSkillIds;
            }

            if (ShouldApplyGuildPremadeDefaultKit(save))
            {
                return Mvp1ClassSkillCatalog.GetDefaultSkillIds(save.ClassId);
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Recover legacy party saves that lost skills after assign-party bug; only guild premade IDs.
        /// </summary>
        static bool ShouldApplyGuildPremadeDefaultKit(CharacterSaveData save) =>
            save.AllocatedSkillPoints <= 0
            && !string.IsNullOrEmpty(save.ClassId)
            && !string.IsNullOrEmpty(save.CharacterId)
            && save.CharacterId.StartsWith("guild_", StringComparison.Ordinal)
            && Array.IndexOf(Mvp1HubConstants.DayOneClassIds, save.ClassId) >= 0;
    }
}
