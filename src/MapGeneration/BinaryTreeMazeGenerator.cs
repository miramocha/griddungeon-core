// Binary Tree maze generator ported from mazelib BinaryTree.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/BinaryTree.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class BinaryTreeMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.BinaryTree;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new BinaryTreeMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                parameters.BinaryTreeSkew
            ).Generate();

        sealed class BinaryTreeMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly (int DeltaX, int DeltaY)[] m_skewOffsets;

            public BinaryTreeMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                MazeBinaryTreeSkew skew
            )
                : base(hallwayWidth, hallwayHeight, random)
            {
                m_skewOffsets = ResolveSkewOffsets(skew);
            }

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);

                for (int y = 1; y < PhysicalHeight; y += 2)
                {
                    for (int x = 1; x < PhysicalWidth; x += 2)
                    {
                        grid.SetCell(x, y, MazeGrid.Open);
                        (int neighborX, int neighborY) = FindNeighbor(x, y);
                        grid.SetCell(neighborX, neighborY, MazeGrid.Open);
                    }
                }

                return grid;
            }

            (int X, int Y) FindNeighbor(int x, int y)
            {
                var neighbors = new List<(int X, int Y)>(2);
                foreach ((int deltaX, int deltaY) in m_skewOffsets)
                {
                    int neighborX = x + deltaX;
                    int neighborY = y + deltaY;
                    if (
                        neighborX > 0
                        && neighborX < PhysicalWidth - 1
                        && neighborY > 0
                        && neighborY < PhysicalHeight - 1
                    )
                    {
                        neighbors.Add((neighborX, neighborY));
                    }
                }

                if (neighbors.Count == 0)
                {
                    return (x, y);
                }

                return neighbors[Random.Next(neighbors.Count)];
            }

            static (int DeltaX, int DeltaY)[] ResolveSkewOffsets(MazeBinaryTreeSkew skew) =>
                skew switch
                {
                    MazeBinaryTreeSkew.NorthEast => new[] { (1, 0), (0, 1) },
                    MazeBinaryTreeSkew.SouthWest => new[] { (-1, 0), (0, -1) },
                    MazeBinaryTreeSkew.SouthEast => new[] { (1, 0), (0, -1) },
                    _ => new[] { (-1, 0), (0, 1) },
                };
        }
    }
}
