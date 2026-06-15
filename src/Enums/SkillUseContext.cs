using System;

namespace GridDungeon.Core.Enums
{
    [Flags]
    public enum SkillUseContext
    {
        None = 0,
        Combat = 1 << 0,
        Field = 1 << 1,
    }
}
