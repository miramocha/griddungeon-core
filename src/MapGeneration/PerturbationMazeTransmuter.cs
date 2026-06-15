// Perturbation transmuter ported from mazelib Perturbation.py (MIT).

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class PerturbationMazeTransmuter : MazeTransmuteAlgoBase, IMazeTransmuter
    {
        public string TransmuterId => MazeTransmuterIds.Perturbation;

        public void Transmute(MazeGrid grid, MazeTransmutationParams parameters) =>
            base.Transmute(grid, parameters);

        protected override void TransmuteCore()
        {
            int repeat = Math.Max(1, Parameters.PerturbationRepeat);
            int newWalls = Math.Max(1, Parameters.PerturbationNewWalls);

            for (int i = 0; i < repeat; i++)
            {
                for (int j = 0; j < newWalls; j++)
                {
                    AddRandomWall();
                }

                ReconnectMaze();
            }
        }

        void AddRandomWall()
        {
            int limit = 2 * Grid.Width * Grid.Height;
            for (int tries = 0; tries <= limit; tries++)
            {
                int y = Random.Next(1, Grid.Height - 1);
                int x;
                if (y % 2 == 0)
                {
                    int oddCount = (Grid.Width - 2 + 1) / 2;
                    x = (Random.Next(oddCount) * 2) + 1;
                }
                else
                {
                    int evenCount = (Grid.Width - 3) / 2;
                    x = evenCount > 0 ? (Random.Next(evenCount) * 2) + 2 : 2;
                }

                if (Grid.IsOpen(x, y))
                {
                    Grid.SetCell(x, y, MazeGrid.Wall);
                    return;
                }
            }
        }

        void ReconnectMaze()
        {
            List<HashSet<(int X, int Y)>> passages = FindAllPassages();
            FixDisjointPassages(passages);
        }

        List<HashSet<(int X, int Y)>> FindAllPassages()
        {
            var passages = new List<HashSet<(int X, int Y)>>();

            for (int y = 1; y < Grid.Height; y += 2)
            {
                for (int x = 1; x < Grid.Width; x += 2)
                {
                    var current = new HashSet<(int X, int Y)> { (x, y) };
                    foreach ((int X, int Y) neighbor in FindUnblockedNeighbors(x, y))
                    {
                        current.Add(neighbor);
                    }

                    bool found = false;
                    for (int i = 0; i < passages.Count; i++)
                    {
                        if (!Intersects(passages[i], current))
                        {
                            continue;
                        }

                        passages[i].UnionWith(current);
                        found = true;
                        break;
                    }

                    if (!found)
                    {
                        passages.Add(current);
                    }
                }
            }

            return JoinIntersectingSets(passages);
        }

        void FixDisjointPassages(List<HashSet<(int X, int Y)>> disjointPassages)
        {
            int reconnectAttempts = 0;
            int attemptLimit = Grid.Width * Grid.Height * 8;

            while (disjointPassages.Count > 1)
            {
                if (reconnectAttempts++ > attemptLimit)
                {
                    throw new InvalidOperationException(
                        "Perturbation could not reconnect maze passages; grid may not be a valid hallway maze."
                    );
                }

                bool found = false;
                var firstPassage = disjointPassages[0];
                var cells = new List<(int X, int Y)>(firstPassage);
                (int X, int Y) cell = cells[Random.Next(cells.Count)];
                var neighbors = FindLatticeNeighbors(cell.X, cell.Y, isWall: false);

                for (int passageIndex = 1; passageIndex < disjointPassages.Count; passageIndex++)
                {
                    HashSet<(int X, int Y)> passage = disjointPassages[passageIndex];
                    foreach ((int X, int Y) neighbor in neighbors)
                    {
                        if (!passage.Contains(neighbor))
                        {
                            continue;
                        }

                        (int X, int Y) mid = Midpoint(neighbor, cell);
                        Grid.SetCell(mid.X, mid.Y, MazeGrid.Open);
                        firstPassage.UnionWith(passage);
                        disjointPassages.RemoveAt(passageIndex);
                        found = true;
                        break;
                    }

                    if (found)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    // Deterministic fallback: scan for any adjacent passage pair (mazelib retries randomly).
                    if (!TryForceMergePassages(disjointPassages))
                    {
                        throw new InvalidOperationException(
                            "Perturbation could not reconnect maze passages; grid may not be a valid hallway maze."
                        );
                    }
                }
            }
        }

        bool TryForceMergePassages(List<HashSet<(int X, int Y)>> disjointPassages)
        {
            var firstPassage = disjointPassages[0];
            foreach ((int X, int Y) cell in firstPassage)
            {
                foreach (
                    (int X, int Y) neighbor in FindLatticeNeighbors(cell.X, cell.Y, isWall: false)
                )
                {
                    for (
                        int passageIndex = 1;
                        passageIndex < disjointPassages.Count;
                        passageIndex++
                    )
                    {
                        HashSet<(int X, int Y)> passage = disjointPassages[passageIndex];
                        if (!passage.Contains(neighbor))
                        {
                            continue;
                        }

                        (int X, int Y) mid = Midpoint(neighbor, cell);
                        Grid.SetCell(mid.X, mid.Y, MazeGrid.Open);
                        firstPassage.UnionWith(passage);
                        disjointPassages.RemoveAt(passageIndex);
                        return true;
                    }
                }
            }

            return false;
        }

        static bool Intersects(HashSet<(int X, int Y)> left, HashSet<(int X, int Y)> right)
        {
            foreach ((int X, int Y) cell in right)
            {
                if (left.Contains(cell))
                {
                    return true;
                }
            }

            return false;
        }

        static List<HashSet<(int X, int Y)>> JoinIntersectingSets(
            List<HashSet<(int X, int Y)>> sets
        )
        {
            bool mergedAny;
            do
            {
                mergedAny = false;
                for (int i = 0; i < sets.Count - 1; i++)
                {
                    if (sets[i] == null)
                    {
                        continue;
                    }

                    for (int j = i + 1; j < sets.Count; j++)
                    {
                        if (sets[j] == null)
                        {
                            continue;
                        }

                        if (!Intersects(sets[i], sets[j]))
                        {
                            continue;
                        }

                        sets[i].UnionWith(sets[j]);
                        sets[j] = null!;
                        mergedAny = true;
                    }
                }
            } while (mergedAny);

            var merged = new List<HashSet<(int X, int Y)>>();
            foreach (HashSet<(int X, int Y)>? set in sets)
            {
                if (set != null)
                {
                    merged.Add(set);
                }
            }

            return merged;
        }
    }
}
