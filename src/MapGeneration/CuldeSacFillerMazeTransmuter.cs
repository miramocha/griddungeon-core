// Cul-de-sac (loop) filler ported from mazelib CuldeSacFiller.py (MIT).

namespace GridDungeon.Core.MapGeneration
{
    public sealed class CuldeSacFillerMazeTransmuter : MazeTransmuteAlgoBase, IMazeTransmuter
    {
        public string TransmuterId => MazeTransmuterIds.CuldeSacFiller;

        public void Transmute(MazeGrid grid, MazeTransmutationParams parameters) =>
            base.Transmute(grid, parameters);

        protected override void TransmuteCore()
        {
            for (int y = 1; y < Grid.Height; y += 2)
            {
                for (int x = 1; x < Grid.Width; x += 2)
                {
                    if (IsStartCell(x, y) || IsEndCell(x, y))
                    {
                        continue;
                    }

                    if (Grid.IsWall(x, y))
                    {
                        continue;
                    }

                    var neighbors = FindUnblockedNeighbors(x, y);
                    if (neighbors.Count != 2)
                    {
                        continue;
                    }

                    if (
                        !TryFindNextIntersection(x, y, neighbors[0], out (int X, int Y) end1)
                        || !TryFindNextIntersection(x, y, neighbors[1], out (int X, int Y) end2)
                    )
                    {
                        continue;
                    }

                    if (end1 == end2)
                    {
                        Grid.SetCell(x, y, MazeGrid.Wall);
                    }
                }
            }
        }

        bool TryFindNextIntersection(
            int startX,
            int startY,
            (int X, int Y) firstStep,
            out (int X, int Y) intersection
        )
        {
            int previousX = startX;
            int previousY = startY;
            int currentX = firstStep.X;
            int currentY = firstStep.Y;

            int stepLimit = Grid.Width * Grid.Height;
            int steps = 0;
            var neighbors = FindUnblockedNeighbors(currentX, currentY);
            while (neighbors.Count == 2)
            {
                if (steps++ > stepLimit)
                {
                    intersection = (currentX, currentY);
                    return false;
                }

                if (neighbors[0].X == previousX && neighbors[0].Y == previousY)
                {
                    previousX = currentX;
                    previousY = currentY;
                    currentX = neighbors[1].X;
                    currentY = neighbors[1].Y;
                }
                else
                {
                    previousX = currentX;
                    previousY = currentY;
                    currentX = neighbors[0].X;
                    currentY = neighbors[0].Y;
                }

                if (currentX == startX && currentY == startY)
                {
                    intersection = (previousX, previousY);
                    return true;
                }

                neighbors = FindUnblockedNeighbors(currentX, currentY);
            }

            intersection = (currentX, currentY);
            return true;
        }
    }
}
