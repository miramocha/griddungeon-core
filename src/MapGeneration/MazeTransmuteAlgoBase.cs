// Shared maze transmuter helpers ported from mazelib MazeTransmuteAlgo.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/transmute/MazeTransmuteAlgo.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public abstract class MazeTransmuteAlgoBase
    {
        protected MazeGrid Grid { get; private set; } = null!;
        protected MazeTransmutationParams Parameters { get; private set; } = null!;
        protected Random Random { get; private set; } = null!;

        public void Transmute(MazeGrid grid, MazeTransmutationParams parameters)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Grid = grid;
            Parameters = parameters;
            Random = parameters.CreateSeededRandom();
            TransmuteCore();
        }

        protected abstract void TransmuteCore();

        protected bool IsStartCell(int x, int y) =>
            Parameters.StartX == x && Parameters.StartY == y;

        protected bool IsEndCell(int x, int y) => Parameters.EndX == x && Parameters.EndY == y;

        protected bool IsWithinOne(int x, int y, int? targetX, int? targetY)
        {
            if (!targetX.HasValue || !targetY.HasValue)
            {
                return false;
            }

            int desireX = targetX.Value;
            int desireY = targetY.Value;
            if (x == desireX)
            {
                return Math.Abs(y - desireY) < 2;
            }

            if (y == desireY)
            {
                return Math.Abs(x - desireX) < 2;
            }

            return false;
        }

        protected List<(int X, int Y)> FindUnblockedNeighbors(int x, int y)
        {
            var neighbors = new List<(int X, int Y)>(4);

            if (y > 1 && Grid.IsOpen(x, y - 1) && Grid.IsOpen(x, y - 2))
            {
                neighbors.Add((x, y - 2));
            }

            if (y < Grid.Height - 2 && Grid.IsOpen(x, y + 1) && Grid.IsOpen(x, y + 2))
            {
                neighbors.Add((x, y + 2));
            }

            if (x > 1 && Grid.IsOpen(x - 1, y) && Grid.IsOpen(x - 2, y))
            {
                neighbors.Add((x - 2, y));
            }

            if (x < Grid.Width - 2 && Grid.IsOpen(x + 1, y) && Grid.IsOpen(x + 2, y))
            {
                neighbors.Add((x + 2, y));
            }

            Shuffle(neighbors);
            return neighbors;
        }

        /// <summary>
        /// Hallway-lattice neighbors two steps away (mazelib <c>_find_neighbors</c>).
        /// Does not require an open mid-cell — unlike <see cref="FindUnblockedNeighbors"/>.
        /// </summary>
        protected List<(int X, int Y)> FindLatticeNeighbors(int x, int y, bool isWall)
        {
            var neighbors = new List<(int X, int Y)>(4);

            if (y > 1 && Grid.IsWall(x, y - 2) == isWall)
            {
                neighbors.Add((x, y - 2));
            }

            if (y < Grid.Height - 2 && Grid.IsWall(x, y + 2) == isWall)
            {
                neighbors.Add((x, y + 2));
            }

            if (x > 1 && Grid.IsWall(x - 2, y) == isWall)
            {
                neighbors.Add((x - 2, y));
            }

            if (x < Grid.Width - 2 && Grid.IsWall(x + 2, y) == isWall)
            {
                neighbors.Add((x + 2, y));
            }

            Shuffle(neighbors);
            return neighbors;
        }

        protected static (int X, int Y) Midpoint((int X, int Y) a, (int X, int Y) b) =>
            ((a.X + b.X) / 2, (a.Y + b.Y) / 2);

        protected void Shuffle<T>(IList<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Next(i + 1);
                (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
            }
        }
    }
}
