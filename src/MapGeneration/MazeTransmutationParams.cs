// Maze transmutation parameters inspired by mazelib (MIT) — https://github.com/john-science/mazelib

using System;

namespace GridDungeon.Core.MapGeneration
{
    /// <summary>Inputs for Core maze transmuters (no UnityEngine).</summary>
    public sealed class MazeTransmutationParams
    {
        public int? Seed { get; set; }

        public string TransmuterId { get; set; } = string.Empty;

        /// <summary>Dead-end filler passes (mazelib <c>DeadEndFiller(iterations)</c>).</summary>
        public int DeadEndIterations { get; set; } = 1;

        /// <summary>Perturbation repeat count (mazelib <c>Perturbation(repeat=…)</c>).</summary>
        public int PerturbationRepeat { get; set; } = 1;

        /// <summary>New walls per perturbation pass (mazelib <c>new_walls</c>).</summary>
        public int PerturbationNewWalls { get; set; } = 1;

        /// <summary>Optional entrance hallway cell protected from dead-end / loop fill.</summary>
        public int? StartX { get; set; }

        public int? StartY { get; set; }

        /// <summary>Optional goal hallway cell protected from dead-end / loop fill.</summary>
        public int? EndX { get; set; }

        public int? EndY { get; set; }

        public Random CreateSeededRandom()
        {
            if (!Seed.HasValue)
            {
                throw new InvalidOperationException(
                    "MazeTransmutationParams.Seed must be set for deterministic Core transmutation."
                );
            }

            return new Random(Seed.Value);
        }
    }
}
