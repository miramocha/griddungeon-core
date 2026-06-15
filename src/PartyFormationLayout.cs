using System;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core
{
    public enum PartyFormationSlotKind
    {
        Core,
        Aux,
    }

    public readonly struct PartyFormationSlotRef
    {
        public PartyFormationSlotRef(
            PartyFormationSlotKind kind,
            int coreIndex,
            int auxIndex,
            bool isFrontRow
        )
        {
            Kind = kind;
            CoreIndex = coreIndex;
            AuxIndex = auxIndex;
            IsFrontRow = isFrontRow;
        }

        public PartyFormationSlotKind Kind { get; }

        /// <summary>0..5 when <see cref="Kind"/> is Core; otherwise -1.</summary>
        public int CoreIndex { get; }

        /// <summary>0..1 when <see cref="Kind"/> is Aux; otherwise -1.</summary>
        public int AuxIndex { get; }

        public bool IsFrontRow { get; }
    }

    /// <summary>Maps 2×4 party grid indices to core/aux formation slots.</summary>
    public static class PartyFormationLayout
    {
        public const int SlotCount = 8;
        public const int GridColumns = 4;

        public static PartyFormationSlotRef DescribeGridSlot(int gridIndex)
        {
            return gridIndex switch
            {
                0 => new PartyFormationSlotRef(PartyFormationSlotKind.Core, 0, -1, true),
                1 => new PartyFormationSlotRef(PartyFormationSlotKind.Core, 1, -1, true),
                2 => new PartyFormationSlotRef(PartyFormationSlotKind.Core, 2, -1, true),
                3 => new PartyFormationSlotRef(PartyFormationSlotKind.Aux, -1, 0, true),
                4 => new PartyFormationSlotRef(PartyFormationSlotKind.Core, 3, -1, false),
                5 => new PartyFormationSlotRef(PartyFormationSlotKind.Core, 4, -1, false),
                6 => new PartyFormationSlotRef(PartyFormationSlotKind.Core, 5, -1, false),
                7 => new PartyFormationSlotRef(PartyFormationSlotKind.Aux, -1, 1, false),
                _ => throw new ArgumentOutOfRangeException(nameof(gridIndex)),
            };
        }

        public static int GridIndexForCore(int coreIndex)
        {
            if (coreIndex < 0 || coreIndex >= BattleFormation.MaxCoreSlots)
            {
                throw new ArgumentOutOfRangeException(nameof(coreIndex));
            }

            return coreIndex < BattleFormation.MaxEnemyFront ? coreIndex : coreIndex + 1;
        }

        public static int GridIndexForAux(int auxIndex)
        {
            if (auxIndex < 0 || auxIndex >= BattleFormation.MaxAuxSlots)
            {
                throw new ArgumentOutOfRangeException(nameof(auxIndex));
            }

            return auxIndex == 0 ? 3 : 7;
        }

        public static FormationRow RowForCoreIndex(int coreIndex) =>
            coreIndex < BattleFormation.MaxEnemyFront ? FormationRow.Front : FormationRow.Back;

        public static bool IsCoreGridSlot(int gridIndex) =>
            DescribeGridSlot(gridIndex).Kind == PartyFormationSlotKind.Core;

        const int FrontRowEndExclusive = 3;
        const int MaxCoreColumnInclusive = 2;

        /// <summary>Row for core grid slots only — aux indices 3 and 7 are not row/column navigable.</summary>
        public static int RowForGridIndex(int gridIndex)
        {
            if (!IsCoreGridSlot(gridIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(gridIndex));
            }

            return gridIndex < FrontRowEndExclusive ? 0 : 1;
        }

        /// <summary>Column for core grid slots only — aux indices 3 and 7 are not row/column navigable.</summary>
        public static int ColumnForGridIndex(int gridIndex)
        {
            if (!IsCoreGridSlot(gridIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(gridIndex));
            }

            return gridIndex switch
            {
                0 or 4 => 0,
                1 or 5 => 1,
                2 or 6 => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(gridIndex)),
            };
        }

        /// <summary>Core grid index for a 2×3 row/column pair (rows 0–1, columns 0–2).</summary>
        public static int GridIndexForRowColumn(int row, int column)
        {
            if (row < 0 || row > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (column < 0 || column > MaxCoreColumnInclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }

            return row == 0 ? column : column + FrontRowEndExclusive + 1;
        }
    }
}
