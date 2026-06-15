// Cellular Automaton maze generator ported from mazelib CellularAutomaton.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/CellularAutomaton.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class CellularAutomatonMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.CellularAutomaton;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new CellularAutomatonMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                parameters.CellularComplexity,
                parameters.CellularDensity
            ).Generate();

        sealed class CellularAutomatonMazeGeneratorAlgo : MazeGenAlgoBase
        {
            float m_complexity;
            float m_density;

            public CellularAutomatonMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                float complexity,
                float density
            )
                : base(hallwayWidth, hallwayHeight, random)
            {
                m_complexity = complexity;
                m_density = density;
            }

            public override MazeGrid Generate()
            {
                var grid = new MazeGrid(HallwayWidth, HallwayHeight);
                grid.Fill(MazeGrid.Open);
                FillBorderWalls(grid);

                if (m_complexity <= 1.0f)
                {
                    m_complexity *= HallwayHeight + HallwayWidth;
                }

                if (m_density <= 1.0f)
                {
                    m_density *= HallwayWidth * HallwayHeight;
                }

                int densityIterations = (int)(2 * m_density);
                for (int i = 0; i < densityIterations; i++)
                {
                    int x;
                    int y;
                    if (i < m_density)
                    {
                        if (Random.Next(2) == 0)
                        {
                            y = Random.Next(2) == 0 ? 0 : PhysicalHeight - 1;
                            x = RandomEvenCoordinate(0, PhysicalWidth - 1);
                        }
                        else
                        {
                            x = Random.Next(2) == 0 ? 0 : PhysicalWidth - 1;
                            y = RandomEvenCoordinate(0, PhysicalHeight - 1);
                        }
                    }
                    else
                    {
                        y = RandomEvenCoordinate(0, PhysicalHeight - 1);
                        x = RandomEvenCoordinate(0, PhysicalWidth - 1);
                    }

                    grid.SetCell(x, y, MazeGrid.Wall);
                    for (int j = 0; j < (int)m_complexity; j++)
                    {
                        List<(int X, int Y)> wallNeighbors = FindWallNeighbors(grid, x, y);
                        if (wallNeighbors.Count > 0 && wallNeighbors.Count < 4)
                        {
                            List<(int X, int Y)> openNeighbors = FindOpenNeighbors(grid, x, y);
                            if (openNeighbors.Count == 0)
                            {
                                continue;
                            }

                            (int nextX, int nextY) = openNeighbors[
                                Random.Next(openNeighbors.Count)
                            ];
                            if (grid.IsOpen(nextX, nextY))
                            {
                                grid.SetCell(nextX, nextY, MazeGrid.Wall);
                                CarvePassageBetween(x, y, nextX, nextY, grid);
                                x = nextX;
                                y = nextY;
                            }
                        }
                    }
                }

                return grid;
            }

            void FillBorderWalls(MazeGrid grid)
            {
                for (int x = 0; x < PhysicalWidth; x++)
                {
                    grid.SetCell(x, 0, MazeGrid.Wall);
                    grid.SetCell(x, PhysicalHeight - 1, MazeGrid.Wall);
                }

                for (int y = 0; y < PhysicalHeight; y++)
                {
                    grid.SetCell(0, y, MazeGrid.Wall);
                    grid.SetCell(PhysicalWidth - 1, y, MazeGrid.Wall);
                }
            }

            int RandomEvenCoordinate(int minInclusive, int maxInclusive) =>
                minInclusive + (Random.Next(((maxInclusive - minInclusive) / 2) + 1) * 2);

            List<(int X, int Y)> FindWallNeighbors(MazeGrid grid, int x, int y) =>
                FindNeighbors(x, y, grid, isWall: true);

            List<(int X, int Y)> FindOpenNeighbors(MazeGrid grid, int x, int y) =>
                FindNeighbors(x, y, grid, isWall: false);
        }
    }
}
