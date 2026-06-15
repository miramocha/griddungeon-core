using System;
using System.Collections.Generic;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Simulators
{
    public static class MapRevealCalculator
    {
        public const int k_ConeDepth = 1;

        public static IEnumerable<GridPosition> EnumerateConeRevealCells(
            GridPosition origin,
            FacingDirection facing,
            int depth,
            Func<GridPosition, bool> isWalkable,
            int gridWidth,
            int gridHeight
        )
        {
            for (int lateral = -depth; lateral <= depth; lateral++)
            {
                int forwardStart = lateral == 0 ? 0 : Math.Abs(lateral);
                for (int forward = forwardStart; forward <= depth; forward++)
                {
                    GridPosition pos = GridMovement.Offset(origin, facing, forward, lateral);
                    if (pos.X < 0 || pos.Y < 0 || pos.X >= gridWidth || pos.Y >= gridHeight)
                    {
                        break;
                    }

                    yield return pos;
                    if (!isWalkable(pos))
                    {
                        break;
                    }
                }
            }
        }

        public static void AddPerimeterEdges(GridPosition cell, ICollection<CellEdge> edges)
        {
            edges.Add(new CellEdge(cell, FacingDirection.North));
            edges.Add(new CellEdge(cell, FacingDirection.East));
            edges.Add(new CellEdge(cell, FacingDirection.South));
            edges.Add(new CellEdge(cell, FacingDirection.West));
        }

        public static IEnumerable<CellEdge> RevealOnBump(
            GridPosition fromCell,
            FacingDirection bumpSide
        ) => new[] { new CellEdge(fromCell, bumpSide) };
    }
}
