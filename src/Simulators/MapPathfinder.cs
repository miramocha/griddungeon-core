using System;
using System.Collections.Generic;

namespace GridDungeon.Core.Simulators
{
    public static class MapPathfinder
    {
        static readonly GridPosition[] s_CardinalOffsets =
        {
            new(0, 1),
            new(0, -1),
            new(1, 0),
            new(-1, 0),
        };

        public static bool TryFindPath(
            GridPosition start,
            GridPosition goal,
            Func<GridPosition, bool> isNodePassable,
            Func<GridPosition, GridPosition, bool> canTraverseEdge,
            out IReadOnlyList<GridPosition> path
        )
        {
            path = Array.Empty<GridPosition>();
            if (!isNodePassable(start) || !isNodePassable(goal))
            {
                return false;
            }

            if (start == goal)
            {
                path = new[] { start };
                return true;
            }

            var openHeap = new BinaryMinHeap();
            var cameFrom = new Dictionary<GridPosition, GridPosition>();
            var gScore = new Dictionary<GridPosition, int> { [start] = 0 };
            int startHeuristic = Manhattan(start, goal);
            openHeap.Push(start, startHeuristic, startHeuristic);

            var closed = new HashSet<GridPosition>();

            while (openHeap.Count > 0)
            {
                GridPosition current = openHeap.Pop();
                if (closed.Contains(current))
                {
                    continue;
                }

                if (current == goal)
                {
                    path = Reconstruct(cameFrom, start, goal);
                    return true;
                }

                closed.Add(current);
                int currentG = gScore[current];

                foreach (GridPosition offset in s_CardinalOffsets)
                {
                    GridPosition neighbor = current + offset;
                    if (closed.Contains(neighbor))
                    {
                        continue;
                    }

                    if (!isNodePassable(neighbor) || !canTraverseEdge(current, neighbor))
                    {
                        continue;
                    }

                    int tentativeG = currentG + 1;
                    if (gScore.TryGetValue(neighbor, out int knownG) && tentativeG >= knownG)
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    int fScore = tentativeG + Manhattan(neighbor, goal);
                    openHeap.Push(neighbor, fScore, Manhattan(neighbor, goal));
                }
            }

            return false;
        }

        static int Manhattan(GridPosition a, GridPosition b) =>
            Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        static List<GridPosition> Reconstruct(
            Dictionary<GridPosition, GridPosition> cameFrom,
            GridPosition start,
            GridPosition goal
        )
        {
            var path = new List<GridPosition> { goal };
            GridPosition current = goal;
            while (current != start)
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }

        sealed class BinaryMinHeap
        {
            readonly List<HeapEntry> m_entries = new();

            public int Count => m_entries.Count;

            public void Push(GridPosition cell, int fScore, int tieBreak)
            {
                m_entries.Add(new HeapEntry(cell, fScore, tieBreak));
                SiftUp(m_entries.Count - 1);
            }

            public GridPosition Pop()
            {
                HeapEntry root = m_entries[0];
                int lastIndex = m_entries.Count - 1;
                m_entries[0] = m_entries[lastIndex];
                m_entries.RemoveAt(lastIndex);
                if (m_entries.Count > 0)
                {
                    SiftDown(0);
                }

                return root.Cell;
            }

            void SiftUp(int index)
            {
                while (index > 0)
                {
                    int parent = (index - 1) / 2;
                    if (Compare(m_entries[index], m_entries[parent]) >= 0)
                    {
                        break;
                    }

                    Swap(index, parent);
                    index = parent;
                }
            }

            void SiftDown(int index)
            {
                int count = m_entries.Count;
                while (true)
                {
                    int left = index * 2 + 1;
                    if (left >= count)
                    {
                        break;
                    }

                    int right = left + 1;
                    int smallest = left;
                    if (right < count && Compare(m_entries[right], m_entries[left]) < 0)
                    {
                        smallest = right;
                    }

                    if (Compare(m_entries[index], m_entries[smallest]) <= 0)
                    {
                        break;
                    }

                    Swap(index, smallest);
                    index = smallest;
                }
            }

            static int Compare(HeapEntry a, HeapEntry b)
            {
                int byF = a.FScore.CompareTo(b.FScore);
                if (byF != 0)
                {
                    return byF;
                }

                int byH = a.TieBreak.CompareTo(b.TieBreak);
                if (byH != 0)
                {
                    return byH;
                }

                int byX = a.Cell.X.CompareTo(b.Cell.X);
                return byX != 0 ? byX : a.Cell.Y.CompareTo(b.Cell.Y);
            }

            void Swap(int a, int b)
            {
                HeapEntry temp = m_entries[a];
                m_entries[a] = m_entries[b];
                m_entries[b] = temp;
            }

            readonly struct HeapEntry
            {
                public HeapEntry(GridPosition cell, int fScore, int tieBreak)
                {
                    Cell = cell;
                    FScore = fScore;
                    TieBreak = tieBreak;
                }

                public GridPosition Cell { get; }
                public int FScore { get; }
                public int TieBreak { get; }
            }
        }
    }
}
