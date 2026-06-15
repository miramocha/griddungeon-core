// Maze generation parameters inspired by mazelib (MIT) — https://github.com/john-science/mazelib

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    /// <summary>Inputs for Core maze generators (no UnityEngine).</summary>
    public sealed class MazeGenerationParams
    {
        public int? Seed { get; set; }

        /// <summary>Hallway count along X (mazelib <c>w</c>).</summary>
        public int HallwayWidth { get; set; }

        /// <summary>Hallway count along Y (mazelib <c>h</c>).</summary>
        public int HallwayHeight { get; set; }

        /// <summary>Registry lookup id; populated by follow-up generator issues.</summary>
        public string AlgorithmId { get; set; } = string.Empty;

        /// <summary>Dungeon room bounds; null or empty → 2–4 random odd-corner rooms.</summary>
        public IReadOnlyList<MazeRoomRect> Rooms { get; set; } = Array.Empty<MazeRoomRect>();

        public MazeDungeonHuntOrder HuntOrder { get; set; } = MazeDungeonHuntOrder.Random;

        public MazeBinaryTreeSkew BinaryTreeSkew { get; set; } = MazeBinaryTreeSkew.NorthWest;

        public float XSkew { get; set; } = 0.5f;

        public float YSkew { get; set; } = 0.5f;

        public float BacktrackChance { get; set; } = 1.0f;

        public float SidewinderSkew { get; set; } = 0.5f;

        public float CellularComplexity { get; set; } = 1.0f;

        public float CellularDensity { get; set; } = 1.0f;

        public Random CreateSeededRandom()
        {
            if (!Seed.HasValue)
            {
                throw new InvalidOperationException(
                    "MazeGenerationParams.Seed must be set for deterministic Core generation."
                );
            }

            return new Random(Seed.Value);
        }
    }
}
