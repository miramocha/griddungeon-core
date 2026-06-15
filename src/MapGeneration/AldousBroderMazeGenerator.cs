// Aldous-Broder maze generator ported from mazelib AldousBroder.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/AldousBroder.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class AldousBroderMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.AldousBroder;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new AldousBroderMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom()
            ).Generate();

        sealed class AldousBroderMazeGeneratorAlgo : MazeGenAlgoBase
        {
            public AldousBroderMazeGeneratorAlgo(int hallwayWidth, int hallwayHeight, Random random)
                : base(hallwayWidth, hallwayHeight, random) { }

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
                (int currentX, int currentY) = RandomOddCorridorCell();
                grid.SetCell(currentX, currentY, MazeGrid.Open);
                int numVisited = 1;

                while (numVisited < HallwayWidth * HallwayHeight)
                {
                    List<(int X, int Y)> neighbors = FindNeighbors(
                        currentX,
                        currentY,
                        grid,
                        isWall: true
                    );
                    if (neighbors.Count == 0)
                    {
                        List<(int X, int Y)> visitedNeighbors = FindNeighbors(
                            currentX,
                            currentY,
                            grid,
                            isWall: false
                        );
                        (currentX, currentY) = visitedNeighbors[
                            Random.Next(visitedNeighbors.Count)
                        ];
                        continue;
                    }

                    foreach ((int neighborX, int neighborY) in neighbors)
                    {
                        if (!grid.IsWall(neighborX, neighborY))
                        {
                            continue;
                        }

                        CarvePassageBetween(currentX, currentY, neighborX, neighborY, grid);
                        grid.SetCell(neighborX, neighborY, MazeGrid.Open);
                        numVisited++;
                        currentX = neighborX;
                        currentY = neighborY;
                        break;
                    }
                }

                return grid;
            }
        }
    }
}
