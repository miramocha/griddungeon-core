// Kruskal's maze generator ported from mazelib Kruskal.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/Kruskal.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class KruskalsMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.Kruskals;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new KruskalsMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom()
            ).Generate();

        sealed class KruskalsMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly int[] m_parent;
            readonly int[] m_rank;

            public KruskalsMazeGeneratorAlgo(int hallwayWidth, int hallwayHeight, Random random)
                : base(hallwayWidth, hallwayHeight, random)
            {
                int cellCount = HallwayWidth * HallwayHeight;
                m_parent = new int[cellCount];
                m_rank = new int[cellCount];
                for (int i = 0; i < cellCount; i++)
                {
                    m_parent[i] = i;
                }
            }

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);

                for (int y = 1; y < PhysicalHeight - 1; y += 2)
                {
                    for (int x = 1; x < PhysicalWidth - 1; x += 2)
                    {
                        grid.SetCell(x, y, MazeGrid.Open);
                    }
                }

                var edges = new List<(int X, int Y)>();
                for (int y = 2; y < PhysicalHeight - 1; y += 2)
                {
                    for (int x = 1; x < PhysicalWidth - 1; x += 2)
                    {
                        edges.Add((x, y));
                    }
                }

                for (int y = 1; y < PhysicalHeight - 1; y += 2)
                {
                    for (int x = 2; x < PhysicalWidth - 1; x += 2)
                    {
                        edges.Add((x, y));
                    }
                }

                Shuffle(edges);

                int disjointSets = HallwayWidth * HallwayHeight;
                foreach ((int edgeX, int edgeY) in edges)
                {
                    (int cellAX, int cellAY, int cellBX, int cellBY) = GetEdgeCells(edgeX, edgeY);
                    int rootA = Find(CellIndex(cellAX, cellAY));
                    int rootB = Find(CellIndex(cellBX, cellBY));
                    if (rootA == rootB)
                    {
                        continue;
                    }

                    Union(rootA, rootB);
                    disjointSets--;
                    grid.SetCell(edgeX, edgeY, MazeGrid.Open);
                    if (disjointSets <= 1)
                    {
                        break;
                    }
                }

                return grid;
            }

            static (int CellAX, int CellAY, int CellBX, int CellBY) GetEdgeCells(
                int edgeX,
                int edgeY
            ) =>
                edgeY % 2 == 0
                    ? (edgeX, edgeY - 1, edgeX, edgeY + 1)
                    : (edgeX - 1, edgeY, edgeX + 1, edgeY);

            int CellIndex(int x, int y) => ((y - 1) / 2) * HallwayWidth + ((x - 1) / 2);

            int Find(int index)
            {
                if (m_parent[index] != index)
                {
                    m_parent[index] = Find(m_parent[index]);
                }

                return m_parent[index];
            }

            void Union(int leftRoot, int rightRoot)
            {
                if (m_rank[leftRoot] < m_rank[rightRoot])
                {
                    m_parent[leftRoot] = rightRoot;
                    return;
                }

                if (m_rank[leftRoot] > m_rank[rightRoot])
                {
                    m_parent[rightRoot] = leftRoot;
                    return;
                }

                m_parent[rightRoot] = leftRoot;
                m_rank[leftRoot]++;
            }
        }
    }
}
