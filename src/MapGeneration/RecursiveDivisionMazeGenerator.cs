// Recursive division maze generator ported from mazelib Division.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/Division.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class RecursiveDivisionMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.RecursiveDivision;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new RecursiveDivisionMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom()
            ).Generate();

        sealed class RecursiveDivisionMazeGeneratorAlgo : MazeGenAlgoBase
        {
            const int k_Vertical = 0;
            const int k_Horizontal = 1;

            public RecursiveDivisionMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random
            )
                : base(hallwayWidth, hallwayHeight, random) { }

            public override MazeGrid Generate()
            {
                var grid = new MazeGrid(HallwayWidth, HallwayHeight);
                grid.Fill(MazeGrid.Open);
                FillBorderWalls(grid);

                var regionStack = new Stack<((int MinY, int MinX), (int MaxY, int MaxX))>();
                regionStack.Push(((1, 1), (PhysicalHeight - 2, PhysicalWidth - 2)));

                while (regionStack.Count > 0)
                {
                    ((int minY, int minX), (int maxY, int maxX)) = regionStack.Pop();
                    int height = maxY - minY + 1;
                    int width = maxX - minX + 1;

                    if (height <= 1 || width <= 1)
                    {
                        continue;
                    }

                    int cutDirection;
                    if (width < height)
                    {
                        cutDirection = k_Horizontal;
                    }
                    else if (width > height)
                    {
                        cutDirection = k_Vertical;
                    }
                    else
                    {
                        if (width == 2)
                        {
                            continue;
                        }

                        cutDirection = Random.Next(2);
                    }

                    int cutLength = cutDirection == k_Vertical ? height : width;
                    if (cutLength < 3)
                    {
                        continue;
                    }

                    int cutPosition = RandomOddInRange(1, cutLength - 1);
                    int doorPosition = RandomEvenInRange(
                        0,
                        (cutDirection == k_Vertical ? height : width) - 1
                    );

                    if (cutDirection == k_Vertical)
                    {
                        int wallX = minX + cutPosition;
                        for (int y = minY; y <= maxY; y++)
                        {
                            grid.SetCell(wallX, y, MazeGrid.Wall);
                        }

                        grid.SetCell(wallX, minY + doorPosition, MazeGrid.Open);
                        regionStack.Push(((minY, minX), (maxY, wallX - 1)));
                        regionStack.Push(((minY, wallX + 1), (maxY, maxX)));
                    }
                    else
                    {
                        int wallY = minY + cutPosition;
                        for (int x = minX; x <= maxX; x++)
                        {
                            grid.SetCell(x, wallY, MazeGrid.Wall);
                        }

                        grid.SetCell(minX + doorPosition, wallY, MazeGrid.Open);
                        regionStack.Push(((minY, minX), (wallY - 1, maxX)));
                        regionStack.Push(((wallY + 1, minX), (maxY, maxX)));
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

            int RandomOddInRange(int minInclusive, int maxInclusive)
            {
                int span = ((maxInclusive - minInclusive) / 2) + 1;
                return minInclusive + (Random.Next(span) * 2);
            }

            int RandomEvenInRange(int minInclusive, int maxInclusive) =>
                minInclusive + (Random.Next(((maxInclusive - minInclusive) / 2) + 1) * 2);
        }
    }
}
