// Hunt-and-Kill maze generator ported from mazelib HuntAndKill.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/HuntAndKill.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class HuntAndKillMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.HuntAndKill;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new HuntAndKillMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                parameters.HuntOrder
            ).Generate();

        sealed class HuntAndKillMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly MazeDungeonHuntOrder m_huntOrder;

            public HuntAndKillMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                MazeDungeonHuntOrder huntOrder
            )
                : base(hallwayWidth, hallwayHeight, random) => m_huntOrder = huntOrder;

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
                (int currentX, int currentY) = RandomOddCorridorCell();
                grid.SetCell(currentX, currentY, MazeGrid.Open);

                int numTrials = 0;
                while (currentX >= 0)
                {
                    Walk(grid, currentX, currentY);
                    (currentX, currentY) = Hunt(grid, numTrials);
                    numTrials++;
                }

                return grid;
            }

            void Walk(MazeGrid grid, int startX, int startY)
            {
                if (!grid.IsOpen(startX, startY))
                {
                    return;
                }

                int currentX = startX;
                int currentY = startY;
                List<(int X, int Y)> unvisitedNeighbors = FindNeighbors(
                    currentX,
                    currentY,
                    grid,
                    isWall: true
                );

                while (unvisitedNeighbors.Count > 0)
                {
                    (int neighborX, int neighborY) = unvisitedNeighbors[
                        Random.Next(unvisitedNeighbors.Count)
                    ];
                    grid.SetCell(neighborX, neighborY, MazeGrid.Open);
                    CarvePassageBetween(currentX, currentY, neighborX, neighborY, grid);
                    currentX = neighborX;
                    currentY = neighborY;
                    unvisitedNeighbors = FindNeighbors(currentX, currentY, grid, isWall: true);
                }
            }

            (int X, int Y) Hunt(MazeGrid grid, int trialCount) =>
                m_huntOrder == MazeDungeonHuntOrder.Serpentine
                    ? HuntSerpentine(grid)
                    : HuntRandom(trialCount);

            (int X, int Y) HuntRandom(int trialCount)
            {
                if (trialCount >= PhysicalHeight * PhysicalWidth)
                {
                    return (-1, -1);
                }

                return RandomOddCorridorCell();
            }

            (int X, int Y) HuntSerpentine(MazeGrid grid)
            {
                int x = 1;
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

                    if (grid.IsOpen(x, y) && FindNeighbors(x, y, grid, isWall: true).Count > 0)
                    {
                        return (x, y);
                    }
                }
            }
        }
    }
}
