// Maze sizing helpers inspired by mazelib (MIT) — https://github.com/john-science/mazelib

using System;

namespace GridDungeon.Core.MapGeneration
{
    /// <summary>
    /// Maps hallway counts to physical maze dimensions (<c>H = 2h + 1</c>, <c>W = 2w + 1</c>)
    /// and resolves hallway counts for target exploration floor sizes.
    /// </summary>
    public static class MazeGridSizing
    {
        public const int MinHallwayDimension = 3;

        public static int PhysicalWidth(int hallwayWidth) => (2 * hallwayWidth) + 1;

        public static int PhysicalHeight(int hallwayHeight) => (2 * hallwayHeight) + 1;

        public static int MinPhysicalDimension => PhysicalWidth(MinHallwayDimension);

        public static void ValidateHallwayDimensions(int hallwayWidth, int hallwayHeight)
        {
            if (hallwayWidth < MinHallwayDimension)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hallwayWidth),
                    hallwayWidth,
                    $"Hallway width must be at least {MinHallwayDimension}."
                );
            }

            if (hallwayHeight < MinHallwayDimension)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hallwayHeight),
                    hallwayHeight,
                    $"Hallway height must be at least {MinHallwayDimension}."
                );
            }
        }

        public static bool TryHallwayWidthFromPhysical(int physicalWidth, out int hallwayWidth)
        {
            hallwayWidth = 0;
            if (!IsValidPhysicalDimension(physicalWidth))
            {
                return false;
            }

            hallwayWidth = (physicalWidth - 1) / 2;
            return hallwayWidth >= MinHallwayDimension;
        }

        public static bool TryHallwayHeightFromPhysical(int physicalHeight, out int hallwayHeight)
        {
            hallwayHeight = 0;
            if (!IsValidPhysicalDimension(physicalHeight))
            {
                return false;
            }

            hallwayHeight = (physicalHeight - 1) / 2;
            return hallwayHeight >= MinHallwayDimension;
        }

        /// <summary>
        /// Largest hallway counts whose physical maze fits inside <paramref name="targetWidth"/> ×
        /// <paramref name="targetHeight"/> (e.g. MVP1 floor grid with optional border pad).
        /// </summary>
        public static bool TryGetLargestHallwayDimensionsForTargetPhysical(
            int targetWidth,
            int targetHeight,
            out int hallwayWidth,
            out int hallwayHeight
        )
        {
            hallwayWidth = 0;
            hallwayHeight = 0;
            if (targetWidth < MinPhysicalDimension || targetHeight < MinPhysicalDimension)
            {
                return false;
            }

            int physicalWidth = LargestOddNotExceeding(targetWidth);
            int physicalHeight = LargestOddNotExceeding(targetHeight);
            if (
                !TryHallwayWidthFromPhysical(physicalWidth, out hallwayWidth)
                || !TryHallwayHeightFromPhysical(physicalHeight, out hallwayHeight)
            )
            {
                hallwayWidth = 0;
                hallwayHeight = 0;
                return false;
            }

            return true;
        }

        public static int PadCellsToTarget(int physicalDimension, int targetDimension) =>
            Math.Max(0, targetDimension - physicalDimension);

        static bool IsValidPhysicalDimension(int physicalDimension) =>
            physicalDimension >= MinPhysicalDimension && physicalDimension % 2 == 1;

        static int LargestOddNotExceeding(int value) => value % 2 == 0 ? value - 1 : value;
    }
}
