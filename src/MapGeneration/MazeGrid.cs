// Maze grid layout inspired by mazelib (MIT) — https://github.com/john-science/mazelib

using System;

namespace GridDungeon.Core.MapGeneration
{
    /// <summary>
    /// Physical maze bitmap. <see cref="MazeGridSizing"/> maps hallway counts to <see cref="Width"/> × <see cref="Height"/>.
    /// Cell values follow mazelib: <see cref="Wall"/> = 1, <see cref="Open"/> = 0.
    /// Y = 0 is the south edge; Y increases north.
    /// </summary>
    public sealed class MazeGrid
    {
        public const int Wall = 1;
        public const int Open = 0;

        readonly int[] m_cells;

        public int HallwayWidth { get; }
        public int HallwayHeight { get; }
        public int Width { get; }
        public int Height { get; }

        public MazeGrid(int hallwayWidth, int hallwayHeight)
        {
            if (hallwayWidth < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hallwayWidth),
                    hallwayWidth,
                    "Hallway width must be at least 1."
                );
            }

            if (hallwayHeight < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hallwayHeight),
                    hallwayHeight,
                    "Hallway height must be at least 1."
                );
            }

            HallwayWidth = hallwayWidth;
            HallwayHeight = hallwayHeight;
            Width = MazeGridSizing.PhysicalWidth(hallwayWidth);
            Height = MazeGridSizing.PhysicalHeight(hallwayHeight);
            m_cells = new int[Width * Height];
        }

        public int GetCell(int x, int y) => m_cells[ToIndex(x, y)];

        public void SetCell(int x, int y, int value) => m_cells[ToIndex(x, y)] = value;

        public bool IsWall(int x, int y) => GetCell(x, y) == Wall;

        public bool IsOpen(int x, int y) => GetCell(x, y) == Open;

        public void Fill(int value)
        {
            for (int i = 0; i < m_cells.Length; i++)
            {
                m_cells[i] = value;
            }
        }

        public static MazeGrid CreateFilledWalls(int hallwayWidth, int hallwayHeight)
        {
            var grid = new MazeGrid(hallwayWidth, hallwayHeight);
            grid.Fill(Wall);
            return grid;
        }

        /// <summary>
        /// Physical bitmap for transmute / ASCII import (hallway counts are best-effort metadata).
        /// </summary>
        public static MazeGrid FromPhysicalDimensions(int physicalWidth, int physicalHeight)
        {
            if (physicalWidth < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(physicalWidth),
                    physicalWidth,
                    "Physical width must be at least 1."
                );
            }

            if (physicalHeight < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(physicalHeight),
                    physicalHeight,
                    "Physical height must be at least 1."
                );
            }

            return new MazeGrid(
                physicalWidth,
                physicalHeight,
                Math.Max(1, (physicalWidth - 1) / 2),
                Math.Max(1, (physicalHeight - 1) / 2)
            );
        }

        MazeGrid(int physicalWidth, int physicalHeight, int hallwayWidth, int hallwayHeight)
        {
            Width = physicalWidth;
            Height = physicalHeight;
            HallwayWidth = hallwayWidth;
            HallwayHeight = hallwayHeight;
            m_cells = new int[Width * Height];
        }

        int ToIndex(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new ArgumentOutOfRangeException(
                    $"({x},{y})",
                    $"Cell ({x},{y}) is outside maze bounds {Width}×{Height}."
                );
            }

            return y * Width + x;
        }
    }
}
