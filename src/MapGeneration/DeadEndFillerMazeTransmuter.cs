// Dead-end filler ported from mazelib DeadEndFiller.py (MIT).

namespace GridDungeon.Core.MapGeneration
{
    public sealed class DeadEndFillerMazeTransmuter : MazeTransmuteAlgoBase, IMazeTransmuter
    {
        public string TransmuterId => MazeTransmuterIds.DeadEndFiller;

        protected override void TransmuteCore()
        {
            int? startX = Parameters.StartX;
            int? startY = Parameters.StartY;
            int? endX = Parameters.EndX;
            int? endY = Parameters.EndY;

            int startSave = 0;
            int endSave = 0;
            if (startX.HasValue && startY.HasValue)
            {
                startSave = Grid.GetCell(startX.Value, startY.Value);
                Grid.SetCell(startX.Value, startY.Value, MazeGrid.Open);
            }

            if (endX.HasValue && endY.HasValue)
            {
                endSave = Grid.GetCell(endX.Value, endY.Value);
                Grid.SetCell(endX.Value, endY.Value, MazeGrid.Open);
            }

            int iterations = Parameters.DeadEndIterations > 0 ? Parameters.DeadEndIterations : 100;
            bool found = true;
            for (int i = 0; i < iterations && found; i++)
            {
                found = FillDeadEnds();
            }

            if (startX.HasValue && startY.HasValue)
            {
                Grid.SetCell(startX.Value, startY.Value, startSave);
            }

            if (endX.HasValue && endY.HasValue)
            {
                Grid.SetCell(endX.Value, endY.Value, endSave);
            }
        }

        bool FillDeadEnds()
        {
            if (!TryFindDeadEnd(out int deadEndX, out int deadEndY))
            {
                return false;
            }

            bool found = false;
            while (deadEndX >= 0)
            {
                found = true;
                FillDeadEnd(deadEndX, deadEndY);

                var neighbors = FindUnblockedNeighbors(deadEndX, deadEndY);
                if (neighbors.Count == 0)
                {
                    break;
                }

                if (neighbors.Count == 1 && IsDeadEnd(neighbors[0].X, neighbors[0].Y))
                {
                    deadEndX = neighbors[0].X;
                    deadEndY = neighbors[0].Y;
                    continue;
                }

                if (!TryFindDeadEnd(out deadEndX, out deadEndY))
                {
                    break;
                }
            }

            return found;
        }

        void FillDeadEnd(int x, int y)
        {
            Grid.SetCell(x, y, MazeGrid.Wall);
            Grid.SetCell(x, y - 1, MazeGrid.Wall);
            Grid.SetCell(x, y + 1, MazeGrid.Wall);
            Grid.SetCell(x - 1, y, MazeGrid.Wall);
            Grid.SetCell(x + 1, y, MazeGrid.Wall);
        }

        bool TryFindDeadEnd(out int x, out int y)
        {
            for (y = 1; y < Grid.Height; y += 2)
            {
                for (x = 1; x < Grid.Width; x += 2)
                {
                    if (IsWithinOne(x, y, Parameters.StartX, Parameters.StartY))
                    {
                        continue;
                    }

                    if (IsWithinOne(x, y, Parameters.EndX, Parameters.EndY))
                    {
                        continue;
                    }

                    if (IsDeadEnd(x, y))
                    {
                        return true;
                    }
                }
            }

            x = -1;
            y = -1;
            return false;
        }

        bool IsDeadEnd(int x, int y)
        {
            if (Grid.IsWall(x, y))
            {
                return false;
            }

            return FindUnblockedNeighbors(x, y).Count < 2;
        }
    }
}
