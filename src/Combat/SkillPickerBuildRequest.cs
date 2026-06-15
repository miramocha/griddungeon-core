using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Combat
{
    public readonly struct SkillPickerBuildRequest
    {
        public SkillPickerBuildRequest(
            SkillUseContext context,
            Combatant actor,
            IReadOnlyList<string> skillIds,
            IReadOnlyDictionary<string, SkillData> skillsById
        )
        {
            Context = context;
            Actor = actor;
            SkillIds = skillIds;
            SkillsById = skillsById;
        }

        public SkillUseContext Context { get; }
        public Combatant Actor { get; }
        public IReadOnlyList<string> SkillIds { get; }
        public IReadOnlyDictionary<string, SkillData> SkillsById { get; }
    }
}
