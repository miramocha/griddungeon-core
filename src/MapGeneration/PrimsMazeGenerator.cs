// Prim's maze generator ported from mazelib Prims.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/Prims.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class PrimsMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.Prims;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new PrimsMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom()
            ).Generate();

        sealed class PrimsMazeGeneratorAlgo : MazeGenAlgoBase
        {
            public PrimsMazeGeneratorAlgo(int hallwayWidth, int hallwayHeight, Random random)
                : base(hallwayWidth, hallwayHeight, random) { }

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
                (int currentX, int currentY) = RandomOddCorridorCell();
                grid.SetCell(currentX, currentY, MazeGrid.Open);

                var neighbors = FindNeighbors(currentX, currentY, grid, isWall: true);
                int visited = 1;
                int targetVisited = HallwayWidth * HallwayHeight;

                while (visited < targetVisited)
                {
                    if (neighbors.Count == 0)
                    {
                        throw new InvalidOperationException(
                            $"Prims frontier exhausted at visited={visited}/{targetVisited}."
                        );
                    }

                    int pickIndex = Random.Next(neighbors.Count);
                    (currentX, currentY) = neighbors[pickIndex];
                    neighbors.RemoveAt(pickIndex);
                    visited++;
                    grid.SetCell(currentX, currentY, MazeGrid.Open);

                    List<(int X, int Y)> openNeighbors = FindNeighbors(
                        currentX,
                        currentY,
                        grid,
                        isWall: false
                    );
                    if (openNeighbors.Count == 0)
                    {
                        throw new InvalidOperationException(
                            $"Prims cell ({currentX},{currentY}) has no open neighbor to carve toward."
                        );
                    }

                    (int openX, int openY) = openNeighbors[0];
                    CarvePassageBetween(currentX, currentY, openX, openY, grid);

                    List<(int X, int Y)> unvisited = FindNeighbors(
                        currentX,
                        currentY,
                        grid,
                        isWall: true
                    );
                    MergeUniqueNeighbors(neighbors, unvisited);
                }

                return grid;
            }

            static void MergeUniqueNeighbors(
                List<(int X, int Y)> neighbors,
                List<(int X, int Y)> unvisited
            )
            {
                foreach ((int x, int y) in unvisited)
                {
                    if (!neighbors.Contains((x, y)))
                    {
                        neighbors.Add((x, y));
                    }
                }
            }
        }
    }
}
