using System;
using System.Collections.Generic;
using System.Diagnostics;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;
using GridDungeon.Core.Simulators;

namespace GridDungeon.Core.Combat
{
    /// <summary>Tab grouping and row enable rules for the skill use picker (ADR 035).</summary>
    public static class SkillPickerCatalog
    {
        public const string TabIdAll = "all";

        static readonly (string TabId, string Label, SkillType Type)[] s_typeTabs =
        {
            ("physical", "Physical", SkillType.Physical),
            ("elemental", "Elemental", SkillType.Elemental),
            ("heal", "Heal / Recovery", SkillType.Heal),
            ("buff", "Buff", SkillType.Buff),
            ("debuff", "Debuff", SkillType.Debuff),
            ("deploy", "Deploy", SkillType.Deploy),
            ("passive", "Passive", SkillType.Passive),
        };

        public static int CategoryTabCount => 1 + s_typeTabs.Length;

        public static SkillPickerPresentationModel Build(SkillPickerBuildRequest request)
        {
            List<SkillPickerRowModel> rows = BuildRows(request);
            rows.Sort(CompareRows);

            var tabs = new List<SkillPickerTabModel> { new(TabIdAll, "All", rows) };

            foreach ((string tabId, string label, SkillType type) in s_typeTabs)
            {
                tabs.Add(new SkillPickerTabModel(tabId, label, FilterRowsByType(rows, type)));
            }

            return new SkillPickerPresentationModel(tabs);
        }

        static List<SkillPickerRowModel> BuildRows(SkillPickerBuildRequest request)
        {
            var rows = new List<SkillPickerRowModel>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (string skillId in request.SkillIds)
            {
                if (string.IsNullOrEmpty(skillId) || !seen.Add(skillId))
                {
                    continue;
                }

                if (!request.SkillsById.TryGetValue(skillId, out SkillData skill))
                {
#if DEBUG
                    Debug.WriteLine(
                        $"[SkillPickerCatalog] Skill '{skillId}' missing from skillsById — row omitted."
                    );
#endif
                    continue;
                }

                string displayName = string.IsNullOrEmpty(skill.DisplayName)
                    ? skillId
                    : skill.DisplayName;
                bool contextOk = (skill.UseContexts & request.Context) != 0;
                bool mpOk = request.Actor.CurrentMp >= skill.MpCost;
                bool bindOk = !StatusSystem.IsBlocked(request.Actor, skill);

                string disabledReason = string.Empty;
                if (!contextOk)
                {
                    disabledReason = "Cannot use here";
                }
                else if (!mpOk)
                {
                    disabledReason = "Not enough MP";
                }
                else if (!bindOk)
                {
                    disabledReason = "Limb bound";
                }

                bool isEnabled = contextOk && mpOk && bindOk;
                rows.Add(
                    new SkillPickerRowModel(
                        skillId,
                        displayName,
                        skill.Type,
                        isEnabled,
                        disabledReason,
                        skill.DescriptionEn,
                        skill.MpCost
                    )
                );
            }

            return rows;
        }

        static List<SkillPickerRowModel> FilterRowsByType(
            IReadOnlyList<SkillPickerRowModel> rows,
            SkillType type
        )
        {
            var filtered = new List<SkillPickerRowModel>();
            foreach (SkillPickerRowModel row in rows)
            {
                if (row.SkillType == type)
                {
                    filtered.Add(row);
                }
            }

            return filtered;
        }

        static int CompareRows(SkillPickerRowModel a, SkillPickerRowModel b)
        {
            int byName = string.Compare(
                a.DisplayName,
                b.DisplayName,
                StringComparison.OrdinalIgnoreCase
            );
            if (byName != 0)
            {
                return byName;
            }

            return string.Compare(a.SkillId, b.SkillId, StringComparison.Ordinal);
        }
    }
}
