// Room rectangle for DungeonRooms generator (y = 0 south).

using System;

namespace GridDungeon.Core.MapGeneration
{
    /// <summary>
    /// Inclusive bounds on the physical maze grid. Corners should use odd coordinates
    /// (mazelib dungeon-room convention).
    /// </summary>
    public readonly struct MazeRoomRect : IEquatable<MazeRoomRect>
    {
        public int MinX { get; }
        public int MinY { get; }
        public int MaxX { get; }
        public int MaxY { get; }

        public MazeRoomRect(int minX, int minY, int maxX, int maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public bool Equals(MazeRoomRect other) =>
            MinX == other.MinX && MinY == other.MinY && MaxX == other.MaxX && MaxY == other.MaxY;

        public override bool Equals(object obj) => obj is MazeRoomRect other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(MinX, MinY, MaxX, MaxY);

        public static bool operator ==(MazeRoomRect left, MazeRoomRect right) => left.Equals(right);

        public static bool operator !=(MazeRoomRect left, MazeRoomRect right) =>
            !left.Equals(right);
    }
}
