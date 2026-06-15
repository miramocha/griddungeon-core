// Maze generator registry — algorithms register on static init (issue #266+).
// Inspired by mazelib (MIT) — https://github.com/john-science/mazelib

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public static class MazeGeneratorRegistry
    {
        // Follow-up Runtime/Editor callers must re-register after domain reload
        // ([RuntimeInitializeOnLoadMethod(SubsystemRegistration)]).
        static readonly Dictionary<string, IMazeGenerator> s_Generators = new(
            StringComparer.Ordinal
        );

        static MazeGeneratorRegistry()
        {
            Register(new PrimsMazeGenerator());
            Register(new BacktrackingMazeGenerator());
            Register(new DungeonRoomsMazeGenerator());
            Register(new BinaryTreeMazeGenerator());
            Register(new SidewinderMazeGenerator());
            Register(new GrowingTreeMazeGenerator());
            Register(new KruskalsMazeGenerator());
            Register(new EllersMazeGenerator());
            Register(new HuntAndKillMazeGenerator());
            Register(new WilsonsMazeGenerator());
            Register(new AldousBroderMazeGenerator());
            Register(new RecursiveDivisionMazeGenerator());
            Register(new CellularAutomatonMazeGenerator());
        }

        public static void Register(IMazeGenerator generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (string.IsNullOrWhiteSpace(generator.AlgorithmId))
            {
                throw new ArgumentException("AlgorithmId is required.", nameof(generator));
            }

            s_Generators[generator.AlgorithmId] = generator;
        }

        public static bool TryGet(string algorithmId, out IMazeGenerator generator)
        {
            if (string.IsNullOrWhiteSpace(algorithmId))
            {
                generator = null!;
                return false;
            }

            return s_Generators.TryGetValue(algorithmId, out generator!);
        }

        public static IReadOnlyCollection<string> RegisteredAlgorithmIds => s_Generators.Keys;

        public static MazeGrid Generate(MazeGenerationParams parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!TryGet(parameters.AlgorithmId, out IMazeGenerator generator))
            {
                throw new ArgumentException(
                    $"No maze generator registered for algorithm id '{parameters.AlgorithmId}'.",
                    nameof(parameters)
                );
            }

            return generator.Generate(parameters);
        }
    }
}
