// Eller's maze generator ported from mazelib Ellers.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/Ellers.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class EllersMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.Ellers;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new EllersMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                Clamp01(parameters.XSkew),
                Clamp01(parameters.YSkew)
            ).Generate();

        static float Clamp01(float value) =>
            value < 0f ? 0f
            : value > 1f ? 1f
            : value;

        sealed class EllersMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly float m_xSkew;
            readonly float m_ySkew;

            public EllersMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                float xSkew,
                float ySkew
            )
                : base(hallwayWidth, hallwayHeight, random)
            {
                m_xSkew = xSkew;
                m_ySkew = ySkew;
            }

            public override MazeGrid Generate()
            {
                int[,] sets = new int[PhysicalHeight, PhysicalWidth];
                for (int y = 0; y < PhysicalHeight; y++)
                {
                    for (int x = 0; x < PhysicalWidth; x++)
                    {
                        sets[y, x] = -1;
                    }
                }

                int maxSetNumber = 0;
                for (int y = 1; y < PhysicalHeight - 1; y += 2)
                {
                    maxSetNumber = InitRow(sets, y, maxSetNumber);
                    MergeOneRow(sets, y);
                    MergeDownARow(sets, y);
                }

                maxSetNumber = InitRow(sets, PhysicalHeight - 2, maxSetNumber);
                ProcessLastRow(sets);

                return CreateGridFromSets(sets);
            }

            int InitRow(int[,] sets, int y, int maxSetNumber)
            {
                for (int x = 1; x < PhysicalWidth; x += 2)
                {
                    if (sets[y, x] < 0)
                    {
                        sets[y, x] = maxSetNumber;
                        maxSetNumber++;
                    }
                }

                return maxSetNumber;
            }

            void MergeOneRow(int[,] sets, int y)
            {
                for (int x = 1; x < PhysicalWidth - 2; x += 2)
                {
                    if (Random.NextDouble() >= m_xSkew)
                    {
                        continue;
                    }

                    if (sets[y, x] == sets[y, x + 2])
                    {
                        continue;
                    }

                    sets[y, x + 1] = sets[y, x];
                    MergeSets(sets, sets[y, x + 2], sets[y, x], maxRow: y);
                }
            }

            void MergeDownARow(int[,] sets, int startY)
            {
                if (startY == PhysicalHeight - 2)
                {
                    return;
                }

                var setCounts = new Dictionary<int, List<int>>();
                for (int x = 1; x < PhysicalWidth; x += 2)
                {
                    int setId = sets[startY, x];
                    if (!setCounts.TryGetValue(setId, out List<int>? columns))
                    {
                        columns = new List<int>();
                        setCounts[setId] = columns;
                    }

                    columns.Add(x);
                }

                foreach (KeyValuePair<int, List<int>> entry in setCounts)
                {
                    int column = entry.Value[Random.Next(entry.Value.Count)];
                    sets[startY + 1, column] = entry.Key;
                    sets[startY + 2, column] = entry.Key;
                }

                for (int x = 1; x < PhysicalWidth - 2; x += 2)
                {
                    if (Random.NextDouble() >= m_ySkew)
                    {
                        continue;
                    }

                    int setId = sets[startY, x];
                    if (sets[startY + 1, x] != -1)
                    {
                        continue;
                    }

                    sets[startY + 1, x] = setId;
                    sets[startY + 2, x] = setId;
                }
            }

            void MergeSets(int[,] sets, int fromSet, int toSet, int maxRow = -1)
            {
                if (maxRow < 0)
                {
                    maxRow = PhysicalHeight - 1;
                }

                for (int y = 1; y <= maxRow; y++)
                {
                    for (int x = 1; x < PhysicalWidth - 1; x++)
                    {
                        if (sets[y, x] == fromSet)
                        {
                            sets[y, x] = toSet;
                        }
                    }
                }
            }

            void ProcessLastRow(int[,] sets)
            {
                int y = PhysicalHeight - 2;
                for (int x = 1; x < PhysicalWidth - 2; x += 2)
                {
                    if (sets[y, x] == sets[y, x + 2])
                    {
                        continue;
                    }

                    sets[y, x + 1] = sets[y, x];
                    MergeSets(sets, sets[y, x + 2], sets[y, x]);
                }
            }

            MazeGrid CreateGridFromSets(int[,] sets)
            {
                var grid = new MazeGrid(HallwayWidth, HallwayHeight);
                grid.Fill(MazeGrid.Open);

                for (int y = 0; y < PhysicalHeight; y++)
                {
                    for (int x = 0; x < PhysicalWidth; x++)
                    {
                        if (sets[y, x] == -1)
                        {
                            grid.SetCell(x, y, MazeGrid.Wall);
                        }
                    }
                }

                return grid;
            }
        }
    }
}
