// Wilson's maze generator ported from mazelib Wilsons.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/Wilsons.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class WilsonsMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.Wilsons;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new WilsonsMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                parameters.HuntOrder
            ).Generate();

        sealed class WilsonsMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly MazeDungeonHuntOrder m_huntOrder;

            public WilsonsMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                MazeDungeonHuntOrder huntOrder
            )
                : base(hallwayWidth, hallwayHeight, random) => m_huntOrder = huntOrder;

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
                (int startX, int startY) = RandomOddCorridorCell();
                grid.SetCell(startX, startY, MazeGrid.Open);
                int numVisited = 1;
                (int currentX, int currentY) = Hunt(grid, numVisited);

                while (currentX >= 0)
                {
                    Dictionary<(int X, int Y), (int DeltaX, int DeltaY)> walk = GenerateRandomWalk(
                        grid,
                        (currentX, currentY)
                    );
                    numVisited += SolveRandomWalk(grid, walk, (currentX, currentY));
                    (currentX, currentY) = Hunt(grid, numVisited);
                }

                return grid;
            }

            (int X, int Y) Hunt(MazeGrid grid, int count) =>
                m_huntOrder == MazeDungeonHuntOrder.Serpentine
                    ? HuntSerpentine(grid)
                    : HuntRandom(count);

            (int X, int Y) HuntRandom(int count)
            {
                if (count >= HallwayWidth * HallwayHeight)
                {
                    return (-1, -1);
                }

                return RandomOddCorridorCell();
            }

            (int X, int Y) HuntSerpentine(MazeGrid grid)
            {
                int x = -1;
                int y = 1;

                while (true)
                {
                    x += 2;
                    if (x > PhysicalWidth - 2)
                    {
                        x = 1;
                        y += 2;
                    }

                    if (y > PhysicalHeight - 2)
                    {
                        return (-1, -1);
                    }

                    if (grid.IsWall(x, y))
                    {
                        return (x, y);
                    }
                }
            }

            Dictionary<(int X, int Y), (int DeltaX, int DeltaY)> GenerateRandomWalk(
                MazeGrid grid,
                (int X, int Y) start
            )
            {
                var walk = new Dictionary<(int X, int Y), (int DeltaX, int DeltaY)>();
                (int deltaX, int deltaY) = RandomDirection(start);
                walk[start] = (deltaX, deltaY);
                (int currentX, int currentY) = Move(start.X, start.Y, deltaX, deltaY);

                while (grid.IsWall(currentX, currentY))
                {
                    (deltaX, deltaY) = RandomDirection((currentX, currentY));
                    walk[(currentX, currentY)] = (deltaX, deltaY);
                    (currentX, currentY) = Move(currentX, currentY, deltaX, deltaY);
                }

                return walk;
            }

            (int DeltaX, int DeltaY) RandomDirection((int X, int Y) current)
            {
                var options = new List<(int DeltaX, int DeltaY)>(4);
                if (current.Y > 1)
                {
                    options.Add((0, -2));
                }

                if (current.Y < PhysicalHeight - 2)
                {
                    options.Add((0, 2));
                }

                if (current.X > 1)
                {
                    options.Add((-2, 0));
                }

                if (current.X < PhysicalWidth - 2)
                {
                    options.Add((2, 0));
                }

                return options[Random.Next(options.Count)];
            }

            static (int X, int Y) Move(int x, int y, int deltaX, int deltaY) =>
                (x + deltaX, y + deltaY);

            int SolveRandomWalk(
                MazeGrid grid,
                Dictionary<(int X, int Y), (int DeltaX, int DeltaY)> walk,
                (int X, int Y) start
            )
            {
                int visits = 0;
                (int currentX, int currentY) = start;

                while (!grid.IsOpen(currentX, currentY))
                {
                    grid.SetCell(currentX, currentY, MazeGrid.Open);
                    (int deltaX, int deltaY) = walk[(currentX, currentY)];
                    (int nextX, int nextY) = Move(currentX, currentY, deltaX, deltaY);
                    CarvePassageBetween(currentX, currentY, nextX, nextY, grid);
                    visits++;
                    currentX = nextX;
                    currentY = nextY;
                }

                return visits;
            }
        }
    }
}
