using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Formation
{
    public static class AuxSlotIndex
    {
        public static int FromRow(FormationRow row) => row == FormationRow.Front ? 0 : 1;

        public static int FromTargetKind(TargetKind kind) =>
            kind == TargetKind.AuxFront ? 0
            : kind == TargetKind.AuxBack ? 1
            : -1;
    }
}
