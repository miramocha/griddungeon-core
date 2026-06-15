// Shared maze generator helpers ported from mazelib MazeGenAlgo.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/MazeGenAlgo.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    /// <summary>
    /// Base class for hallway maze generators (issue #266+).
    /// Implement <see cref="IMazeGenerator"/> in follow-ups; construct from
    /// <see cref="MazeGenerationParams"/> then call <see cref="Generate()"/>.
    /// </summary>
    public abstract class MazeGenAlgoBase
    {
        protected int HallwayHeight { get; }
        protected int HallwayWidth { get; }
        protected int PhysicalHeight { get; }
        protected int PhysicalWidth { get; }
        protected Random Random { get; }

        protected MazeGenAlgoBase(int hallwayWidth, int hallwayHeight, Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            MazeGridSizing.ValidateHallwayDimensions(hallwayWidth, hallwayHeight);
            HallwayWidth = hallwayWidth;
            HallwayHeight = hallwayHeight;
            PhysicalWidth = MazeGridSizing.PhysicalWidth(hallwayWidth);
            PhysicalHeight = MazeGridSizing.PhysicalHeight(hallwayHeight);
            Random = random;
        }

        public abstract MazeGrid Generate();

        /// <summary>
        /// Neighbors two cells away on the odd corridor lattice (mazelib <c>_find_neighbors</c>).
        /// </summary>
        protected List<(int X, int Y)> FindNeighbors(int x, int y, MazeGrid grid, bool isWall)
        {
            int targetValue = isWall ? MazeGrid.Wall : MazeGrid.Open;
            var neighbors = new List<(int X, int Y)>(4);

            if (y > 1 && grid.GetCell(x, y - 2) == targetValue)
            {
                neighbors.Add((x, y - 2));
            }

            if (y < PhysicalHeight - 2 && grid.GetCell(x, y + 2) == targetValue)
            {
                neighbors.Add((x, y + 2));
            }

            if (x > 1 && grid.GetCell(x - 2, y) == targetValue)
            {
                neighbors.Add((x - 2, y));
            }

            if (x < PhysicalWidth - 2 && grid.GetCell(x + 2, y) == targetValue)
            {
                neighbors.Add((x + 2, y));
            }

            Shuffle(neighbors);
            return neighbors;
        }

        protected void Shuffle<T>(IList<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Next(i + 1);
                (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
            }
        }

        protected (int X, int Y) RandomOddCorridorCell() =>
            ((Random.Next(HallwayWidth) * 2) + 1, (Random.Next(HallwayHeight) * 2) + 1);

        protected static void CarvePassageBetween(
            int fromX,
            int fromY,
            int toX,
            int toY,
            MazeGrid grid
        ) => grid.SetCell((fromX + toX) / 2, (fromY + toY) / 2, MazeGrid.Open);
    }
}
