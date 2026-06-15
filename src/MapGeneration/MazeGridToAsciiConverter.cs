// ASCII export for Floor Editor / ExplorationFloor rows.
// Coordinate flip matches north-up authoring (y = 0 south).

namespace GridDungeon.Core.MapGeneration
{
    public static class MazeGridToAsciiConverter
    {
        public const char DefaultWallChar = '#';
        public const char DefaultOpenChar = '.';

        public static string ToAscii(
            MazeGrid grid,
            char wallChar = DefaultWallChar,
            char openChar = DefaultOpenChar
        )
        {
            string[] rows = ToAsciiRows(grid, wallChar, openChar);
            return string.Join("\n", rows);
        }

        /// <summary>North-up rows (index 0 = north edge).</summary>
        public static string[] ToAsciiRows(
            MazeGrid grid,
            char wallChar = DefaultWallChar,
            char openChar = DefaultOpenChar
        )
        {
            var rows = new string[grid.Height];
            for (int rowFromNorth = 0; rowFromNorth < grid.Height; rowFromNorth++)
            {
                int y = grid.Height - 1 - rowFromNorth;
                var chars = new char[grid.Width];
                for (int x = 0; x < grid.Width; x++)
                {
                    chars[x] = grid.IsWall(x, y) ? wallChar : openChar;
                }

                rows[rowFromNorth] = new string(chars);
            }

            return rows;
        }
    }
}
