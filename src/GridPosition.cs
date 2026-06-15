using System;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core
{
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static GridPosition operator +(GridPosition a, GridPosition b) =>
            new(a.X + b.X, a.Y + b.Y);

        public bool Equals(GridPosition other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is GridPosition other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(GridPosition left, GridPosition right) => left.Equals(right);

        public static bool operator !=(GridPosition left, GridPosition right) =>
            !left.Equals(right);

        public override string ToString() => $"({X},{Y})";
    }

    public readonly struct CellEdge : IEquatable<CellEdge>
    {
        public GridPosition Cell { get; }
        public FacingDirection Side { get; }

        public CellEdge(GridPosition cell, FacingDirection side)
        {
            Cell = cell;
            Side = side;
        }

        public bool Equals(CellEdge other) => Cell.Equals(other.Cell) && Side == other.Side;

        public override bool Equals(object obj) => obj is CellEdge other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Cell, Side);
    }
}
