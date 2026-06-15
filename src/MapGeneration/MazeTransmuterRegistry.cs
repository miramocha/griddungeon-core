// Maze transmuter registry — algorithms register on static init (issue #269 wave 2).
// Inspired by mazelib (MIT) — https://github.com/john-science/mazelib

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public static class MazeTransmuterRegistry
    {
        static readonly Dictionary<string, IMazeTransmuter> s_Transmuters = new(
            StringComparer.Ordinal
        );

        static MazeTransmuterRegistry()
        {
            Register(new CuldeSacFillerMazeTransmuter());
            Register(new DeadEndFillerMazeTransmuter());
            Register(new PerturbationMazeTransmuter());
        }

        public static void Register(IMazeTransmuter transmuter)
        {
            if (transmuter == null)
            {
                throw new ArgumentNullException(nameof(transmuter));
            }

            if (string.IsNullOrWhiteSpace(transmuter.TransmuterId))
            {
                throw new ArgumentException("TransmuterId is required.", nameof(transmuter));
            }

            s_Transmuters[transmuter.TransmuterId] = transmuter;
        }

        public static bool TryGet(string transmuterId, out IMazeTransmuter transmuter)
        {
            if (string.IsNullOrWhiteSpace(transmuterId))
            {
                transmuter = null!;
                return false;
            }

            return s_Transmuters.TryGetValue(transmuterId, out transmuter!);
        }

        public static IReadOnlyCollection<string> RegisteredTransmuterIds => s_Transmuters.Keys;

        public static void Transmute(MazeGrid grid, MazeTransmutationParams parameters)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!TryGet(parameters.TransmuterId, out IMazeTransmuter transmuter))
            {
                throw new ArgumentException(
                    $"No maze transmuter registered for id '{parameters.TransmuterId}'.",
                    nameof(parameters)
                );
            }

            transmuter.Transmute(grid, parameters);
        }
    }
}
