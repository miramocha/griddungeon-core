// Growing Tree maze generator ported from mazelib GrowingTree.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/GrowingTree.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class GrowingTreeMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.GrowingTree;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new GrowingTreeMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                Clamp01(parameters.BacktrackChance)
            ).Generate();

        static float Clamp01(float value) =>
            value < 0f ? 0f
            : value > 1f ? 1f
            : value;

        sealed class GrowingTreeMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly float m_backtrackChance;

            public GrowingTreeMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                float backtrackChance
            )
                : base(hallwayWidth, hallwayHeight, random) => m_backtrackChance = backtrackChance;

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
                (int currentX, int currentY) = RandomOddCorridorCell();
                grid.SetCell(currentX, currentY, MazeGrid.Open);
                var active = new List<(int X, int Y)> { (currentX, currentY) };

                while (active.Count > 0)
                {
                    if (Random.NextDouble() < m_backtrackChance)
                    {
                        (currentX, currentY) = active[active.Count - 1];
                    }
                    else
                    {
                        (currentX, currentY) = active[Random.Next(active.Count)];
                    }

                    List<(int X, int Y)> nextNeighbors = FindNeighbors(
                        currentX,
                        currentY,
                        grid,
                        isWall: true
                    );
                    if (nextNeighbors.Count == 0)
                    {
                        RemoveActiveCell(active, currentX, currentY);
                        continue;
                    }

                    (int nextX, int nextY) = nextNeighbors[Random.Next(nextNeighbors.Count)];
                    active.Add((nextX, nextY));
                    grid.SetCell(nextX, nextY, MazeGrid.Open);
                    CarvePassageBetween(currentX, currentY, nextX, nextY, grid);
                }

                return grid;
            }

            static void RemoveActiveCell(List<(int X, int Y)> active, int x, int y)
            {
                for (int i = active.Count - 1; i >= 0; i--)
                {
                    if (active[i].X == x && active[i].Y == y)
                    {
                        active.RemoveAt(i);
                    }
                }
            }
        }
    }
}
