// Dungeon rooms maze generator ported from mazelib DungeonRooms.py (MIT) —
// https://github.com/john-science/mazelib/blob/main/mazelib/generate/DungeonRooms.py

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public sealed class DungeonRoomsMazeGenerator : IMazeGenerator
    {
        public string AlgorithmId => MazeGeneratorIds.DungeonRooms;

        public MazeGrid Generate(MazeGenerationParams parameters)
        {
            Random random = parameters.CreateSeededRandom();
            IReadOnlyList<MazeRoomRect> rooms = ResolveRooms(parameters, random);
            return new DungeonRoomsMazeGeneratorAlgo(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                random,
                rooms,
                parameters.HuntOrder
            ).Generate();
        }

        static IReadOnlyList<MazeRoomRect> ResolveRooms(
            MazeGenerationParams parameters,
            Random random
        )
        {
            if (parameters.Rooms != null && parameters.Rooms.Count > 0)
            {
                return parameters.Rooms;
            }

            return MazeRoomLayout.CreateDefaultRooms(
                parameters.HallwayWidth,
                parameters.HallwayHeight,
                random
            );
        }
    }

    static class MazeRoomLayout
    {
        public static List<MazeRoomRect> CreateDefaultRooms(
            int hallwayWidth,
            int hallwayHeight,
            Random random
        )
        {
            int physicalWidth = MazeGridSizing.PhysicalWidth(hallwayWidth);
            int physicalHeight = MazeGridSizing.PhysicalHeight(hallwayHeight);
            int roomCount = random.Next(2, 5);
            var rooms = new List<MazeRoomRect>(roomCount);
            int attempts = 0;

            while (rooms.Count < roomCount && attempts < 200)
            {
                attempts++;
                if (
                    !TryCreateRandomRoom(
                        physicalWidth,
                        physicalHeight,
                        hallwayWidth,
                        hallwayHeight,
                        random,
                        out MazeRoomRect room
                    )
                )
                {
                    continue;
                }

                rooms.Add(room);
            }

            return rooms;
        }

        static bool TryCreateRandomRoom(
            int physicalWidth,
            int physicalHeight,
            int hallwayWidth,
            int hallwayHeight,
            Random random,
            out MazeRoomRect room
        )
        {
            room = default;
            int maxWidthCells = Math.Min(7, (hallwayWidth * 2) - 1);
            int maxHeightCells = Math.Min(7, (hallwayHeight * 2) - 1);
            if (maxWidthCells < 3 || maxHeightCells < 3)
            {
                return false;
            }

            int widthCells = (random.Next((maxWidthCells - 1) / 2 + 1) * 2) + 1;
            int heightCells = (random.Next((maxHeightCells - 1) / 2 + 1) * 2) + 1;
            int maxMinX = physicalWidth - widthCells - 1;
            int maxMinY = physicalHeight - heightCells - 1;
            if (maxMinX < 1 || maxMinY < 1)
            {
                return false;
            }

            int minX = (random.Next(maxMinX / 2 + 1) * 2) + 1;
            int minY = (random.Next(maxMinY / 2 + 1) * 2) + 1;
            int maxX = minX + widthCells - 1;
            int maxY = minY + heightCells - 1;
            if (maxX > physicalWidth - 2 || maxY > physicalHeight - 2)
            {
                return false;
            }

            room = new MazeRoomRect(minX, minY, maxX, maxY);
            return true;
        }

        public static bool IsInsideGrid(MazeRoomRect room, MazeGrid grid) =>
            room.MinX >= 0
            && room.MinY >= 0
            && room.MaxX < grid.Width
            && room.MaxY < grid.Height
            && room.MinX <= room.MaxX
            && room.MinY <= room.MaxY;

        public static void CarveRoom(MazeRoomRect room, MazeGrid grid)
        {
            for (int y = room.MinY; y <= room.MaxY; y++)
            {
                for (int x = room.MinX; x <= room.MaxX; x++)
                {
                    grid.SetCell(x, y, MazeGrid.Open);
                }
            }
        }

        public static void CarveDoor(MazeRoomRect room, MazeGrid grid, Random random)
        {
            if (HasEvenCorner(room))
            {
                return;
            }

            var doors = new List<(int X, int Y)>();
            List<int> oddXs = CollectOddValues(room.MinX - 1, room.MaxX + 1);
            List<int> oddYs = CollectOddValues(room.MinY - 1, room.MaxY + 1);

            if (room.MaxY < grid.Height - 2)
            {
                foreach (int x in oddXs)
                {
                    doors.Add((x, room.MaxY + 1));
                }
            }

            if (room.MinY > 1)
            {
                foreach (int x in oddXs)
                {
                    doors.Add((x, room.MinY - 1));
                }
            }

            if (room.MinX > 1)
            {
                foreach (int y in oddYs)
                {
                    doors.Add((room.MinX - 1, y));
                }
            }

            if (room.MaxX < grid.Width - 2)
            {
                foreach (int y in oddYs)
                {
                    doors.Add((room.MaxX + 1, y));
                }
            }

            if (doors.Count == 0)
            {
                return;
            }

            (int doorX, int doorY) = doors[random.Next(doors.Count)];
            grid.SetCell(doorX, doorY, MazeGrid.Open);
        }

        static bool HasEvenCorner(MazeRoomRect room) =>
            room.MinX % 2 == 0 || room.MinY % 2 == 0 || room.MaxX % 2 == 0 || room.MaxY % 2 == 0;

        static List<int> CollectOddValues(int min, int max)
        {
            var values = new List<int>();
            for (int value = min; value <= max; value++)
            {
                if (value % 2 == 1)
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }

    sealed class DungeonRoomsMazeGeneratorAlgo : MazeGenAlgoBase
    {
        readonly IReadOnlyList<MazeRoomRect> m_rooms;
        readonly MazeDungeonHuntOrder m_huntOrder;

        public DungeonRoomsMazeGeneratorAlgo(
            int hallwayWidth,
            int hallwayHeight,
            Random random,
            IReadOnlyList<MazeRoomRect> rooms,
            MazeDungeonHuntOrder huntOrder
        )
            : base(hallwayWidth, hallwayHeight, random)
        {
            m_rooms = rooms ?? Array.Empty<MazeRoomRect>();
            m_huntOrder = huntOrder;
        }

        public override MazeGrid Generate()
        {
            MazeGrid grid = MazeGrid.CreateFilledWalls(HallwayWidth, HallwayHeight);
            CarveRooms(grid);
            (int currentX, int currentY) = ChooseStart(grid);
            grid.SetCell(currentX, currentY, MazeGrid.Open);

            int huntTrials = 0;
            while (currentX >= 0)
            {
                Walk(grid, currentX, currentY);
                (currentX, currentY) = Hunt(grid, huntTrials);
                huntTrials++;
            }

            ReconnectMaze(grid);
            return grid;
        }

        void CarveRooms(MazeGrid grid)
        {
            foreach (MazeRoomRect room in m_rooms)
            {
                if (!MazeRoomLayout.IsInsideGrid(room, grid))
                {
                    continue;
                }

                MazeRoomLayout.CarveRoom(room, grid);
                MazeRoomLayout.CarveDoor(room, grid, Random);
            }
        }

        (int X, int Y) ChooseStart(MazeGrid grid)
        {
            int limit = PhysicalHeight * PhysicalWidth * 2;
            for (int attempt = 0; attempt < limit; attempt++)
            {
                (int x, int y) = RandomOddCorridorCell();
                if (grid.IsWall(x, y))
                {
                    return (x, y);
                }
            }

            throw new InvalidOperationException(
                "DungeonRooms could not find a wall cell outside carved rooms."
            );
        }

        void Walk(MazeGrid grid, int startX, int startY)
        {
            if (!grid.IsOpen(startX, startY))
            {
                return;
            }

            int currentX = startX;
            int currentY = startY;
            List<(int X, int Y)> unvisitedNeighbors = FindNeighbors(
                currentX,
                currentY,
                grid,
                isWall: true
            );

            while (unvisitedNeighbors.Count > 0)
            {
                (int neighborX, int neighborY) = unvisitedNeighbors[0];
                grid.SetCell(neighborX, neighborY, MazeGrid.Open);
                CarvePassageBetween(currentX, currentY, neighborX, neighborY, grid);
                currentX = neighborX;
                currentY = neighborY;
                unvisitedNeighbors = FindNeighbors(currentX, currentY, grid, isWall: true);
            }
        }

        (int X, int Y) Hunt(MazeGrid grid, int trialCount)
        {
            return m_huntOrder == MazeDungeonHuntOrder.Serpentine
                ? HuntSerpentine(grid)
                : HuntRandom(trialCount);
        }

        (int X, int Y) HuntRandom(int trialCount)
        {
            if (trialCount >= PhysicalHeight * PhysicalWidth)
            {
                return (-1, -1);
            }

            return RandomOddCorridorCell();
        }

        (int X, int Y) HuntSerpentine(MazeGrid grid)
        {
            int x = 1;
            int y = 1;
            while (true)
            {
                x += 2;
                if (x > PhysicalWidth - 2)
                {
                    x = 1;
                    y += 2;
                }

                if (y > PhysicalHeight - 2)
                {
                    return (-1, -1);
                }

                if (grid.IsOpen(x, y) && FindNeighbors(x, y, grid, isWall: true).Count > 0)
                {
                    return (x, y);
                }
            }
        }

        void ReconnectMaze(MazeGrid grid)
        {
            FixDisjointPassages(grid, FindAllPassages(grid));
        }

        void FixDisjointPassages(MazeGrid grid, List<HashSet<(int X, int Y)>> disjointPassages)
        {
            while (disjointPassages.Count > 1)
            {
                HashSet<(int X, int Y)> first = disjointPassages[0];
                int attemptLimit = first.Count * disjointPassages.Count;
                int attempts = 0;
                bool found = false;
                while (!found)
                {
                    if (attempts++ >= attemptLimit)
                    {
                        throw new InvalidOperationException(
                            $"DungeonRooms reconnect exhausted after {attemptLimit} attempts "
                                + $"({PhysicalWidth}x{PhysicalHeight}, {disjointPassages.Count} disjoint passages)."
                        );
                    }

                    (int cellX, int cellY) = PickRandomCell(first);
                    List<(int X, int Y)> neighbors = FindNeighbors(
                        cellX,
                        cellY,
                        grid,
                        isWall: false
                    );

                    for (
                        int passageIndex = 1;
                        passageIndex < disjointPassages.Count;
                        passageIndex++
                    )
                    {
                        HashSet<(int X, int Y)> passage = disjointPassages[passageIndex];
                        foreach ((int neighborX, int neighborY) in neighbors)
                        {
                            if (!passage.Contains((neighborX, neighborY)))
                            {
                                continue;
                            }

                            (int midX, int midY) = Midpoint(neighborX, neighborY, cellX, cellY);
                            grid.SetCell(midX, midY, MazeGrid.Open);
                            first.UnionWith(passage);
                            disjointPassages.RemoveAt(passageIndex);
                            found = true;
                            break;
                        }

                        if (found)
                        {
                            break;
                        }
                    }
                }
            }
        }

        List<HashSet<(int X, int Y)>> FindAllPassages(MazeGrid grid)
        {
            var passages = new List<HashSet<(int X, int Y)>>();

            for (int y = 1; y < PhysicalHeight; y += 2)
            {
                for (int x = 1; x < PhysicalWidth; x += 2)
                {
                    List<(int X, int Y)> neighbors = FindUnblockedNeighbors(grid, x, y);
                    var current = new HashSet<(int X, int Y)> { (x, y) };
                    foreach ((int neighborX, int neighborY) in neighbors)
                    {
                        current.Add((neighborX, neighborY));
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

        List<(int X, int Y)> FindUnblockedNeighbors(MazeGrid grid, int x, int y)
        {
            var neighbors = new List<(int X, int Y)>(4);

            if (y > 1 && grid.IsOpen(x, y - 1) && grid.IsOpen(x, y - 2))
            {
                neighbors.Add((x, y - 2));
            }

            if (y < PhysicalHeight - 2 && grid.IsOpen(x, y + 1) && grid.IsOpen(x, y + 2))
            {
                neighbors.Add((x, y + 2));
            }

            if (x > 1 && grid.IsOpen(x - 1, y) && grid.IsOpen(x - 2, y))
            {
                neighbors.Add((x - 2, y));
            }

            if (x < PhysicalWidth - 2 && grid.IsOpen(x + 1, y) && grid.IsOpen(x + 2, y))
            {
                neighbors.Add((x + 2, y));
            }

            Shuffle(neighbors);
            return neighbors;
        }

        static List<HashSet<(int X, int Y)>> JoinIntersectingSets(
            List<HashSet<(int X, int Y)>> sets
        )
        {
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
                }
            }

            var merged = new List<HashSet<(int X, int Y)>>();
            foreach (HashSet<(int X, int Y)> set in sets)
            {
                if (set != null)
                {
                    merged.Add(set);
                }
            }

            return merged;
        }

        static bool Intersects(HashSet<(int X, int Y)> left, HashSet<(int X, int Y)> right)
        {
            foreach ((int x, int y) in right)
            {
                if (left.Contains((x, y)))
                {
                    return true;
                }
            }

            return false;
        }

        (int X, int Y) PickRandomCell(HashSet<(int X, int Y)> cells)
        {
            int pick = Random.Next(cells.Count);
            foreach ((int x, int y) in cells)
            {
                if (pick-- == 0)
                {
                    return (x, y);
                }
            }

            throw new InvalidOperationException("Passage set was empty.");
        }

        static (int X, int Y) Midpoint(int ax, int ay, int bx, int by) =>
            ((ax + bx) / 2, (ay + by) / 2);
    }
}
