// Recursive backtracking maze generator ported from mazelib BacktrackingGenerator.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/BacktrackingGenerator.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class BacktrackingMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.Backtracking;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new BacktrackingMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom()
            ).Generate();

        sealed class BacktrackingMazeGeneratorAlgo : MazeGenAlgoBase
        {
            public BacktrackingMazeGeneratorAlgo(int hallwayWidth, int hallwayHeight, Random random)
                : base(hallwayWidth, hallwayHeight, random) { }

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
                (int currentX, int currentY) = RandomOddCorridorCell();
                var track = new Stack<(int X, int Y)>();
                track.Push((currentX, currentY));
                grid.SetCell(currentX, currentY, MazeGrid.Open);

                while (track.Count > 0)
                {
                    (currentX, currentY) = track.Peek();
                    var neighbors = FindNeighbors(currentX, currentY, grid, isWall: true);

                    if (neighbors.Count == 0)
                    {
                        track.Pop();
                    }
                    else
                    {
                        (int nextX, int nextY) = neighbors[0];
                        grid.SetCell(nextX, nextY, MazeGrid.Open);
                        CarvePassageBetween(currentX, currentY, nextX, nextY, grid);
                        track.Push((nextX, nextY));
                    }
                }

                return grid;
            }
        }
    }
}
