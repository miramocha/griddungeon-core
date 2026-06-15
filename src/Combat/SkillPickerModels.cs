using System.Collections.Generic;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Combat
{
    public sealed class SkillPickerPresentationModel
    {
        public SkillPickerPresentationModel(IReadOnlyList<SkillPickerTabModel> tabs)
        {
            Tabs = tabs;
        }

        public IReadOnlyList<SkillPickerTabModel> Tabs { get; }
    }

    public sealed class SkillPickerTabModel
    {
        public SkillPickerTabModel(
            string tabId,
            string label,
            IReadOnlyList<SkillPickerRowModel> rows
        )
        {
            TabId = tabId;
            Label = label;
            Rows = rows;
        }

        public string TabId { get; }
        public string Label { get; }
        public IReadOnlyList<SkillPickerRowModel> Rows { get; }
    }

    public sealed class SkillPickerRowModel
    {
        public SkillPickerRowModel(
            string skillId,
            string displayName,
            SkillType skillType,
            bool isEnabled,
            string disabledReason,
            string descriptionEn,
            int mpCost
        )
        {
            SkillId = skillId;
            DisplayName = displayName;
            SkillType = skillType;
            IsEnabled = isEnabled;
            DisabledReason = disabledReason ?? string.Empty;
            DescriptionEn = descriptionEn ?? string.Empty;
            CostLabel = $"{mpCost} MP";
        }

        public string SkillId { get; }
        public string DisplayName { get; }
        public string DescriptionEn { get; }
        public string CostLabel { get; }
        public SkillType SkillType { get; }
        public bool IsEnabled { get; }
        public string DisabledReason { get; }
    }
}
