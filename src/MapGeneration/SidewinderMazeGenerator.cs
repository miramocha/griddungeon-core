// Sidewinder maze generator ported from mazelib Sidewinder.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/Sidewinder.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class SidewinderMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.Sidewinder;

        public MazeGrid Generate(MazeGenerationParams parameters) =>
            new SidewinderMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                parameters.CreateSeededRandom(),
                Clamp01(parameters.SidewinderSkew)
            ).Generate();

        static float Clamp01(float value) =>
            value < 0f ? 0f
            : value > 1f ? 1f
            : value;

        sealed class SidewinderMazeGeneratorAlgo : MazeGenAlgoBase
        {
            readonly float m_skew;

            public SidewinderMazeGeneratorAlgo(
                int hallwayWidth,
                int hallwayHeight,
                Random random,
                float skew
            )
                : base(hallwayWidth, hallwayHeight, random) => m_skew = skew;

            public override MazeGrid Generate()
            {
                MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);

                for (int x = 1; x < PhysicalWidth - 1; x++)
                {
                    grid.SetCell(x, 1, MazeGrid.Open);
                }

                for (int y = 3; y < PhysicalHeight; y += 2)
                {
                    var run = new List<(int X, int Y)>();

                    for (int x = 1; x < PhysicalWidth; x += 2)
                    {
                        grid.SetCell(x, y, MazeGrid.Open);
                        run.Add((x, y));

                        bool carveEast = Random.NextDouble() > m_skew;
                        if (carveEast && x < PhysicalWidth - 2)
                        {
                            grid.SetCell(x + 1, y, MazeGrid.Open);
                        }
                        else
                        {
                            (int northX, int northY) = run[Random.Next(run.Count)];
                            grid.SetCell(northX, northY - 1, MazeGrid.Open);
                            run.Clear();
                        }
                    }
                }

                return grid;
            }
        }
    }
}
