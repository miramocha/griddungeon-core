// ASCII import for Floor Editor transmute / tests.

using System;
using System.Collections.Generic;

namespace GridDungeon.Core.MapGeneration
{
    public static class MazeAsciiGridConverter
    {
        public static MazeGrid FromNorthUpRows(
            IReadOnlyList<string> rowsNorthUp,
            char wallChar = MazeGridToAsciiConverter.DefaultWallChar,
            char openChar = MazeGridToAsciiConverter.DefaultOpenChar
        )
        {
            if (rowsNorthUp == null)
            {
                throw new ArgumentNullException(nameof(rowsNorthUp));
            }

            if (rowsNorthUp.Count == 0)
            {
                throw new ArgumentException("At least one row is required.", nameof(rowsNorthUp));
            }

            int height = rowsNorthUp.Count;
            int width = rowsNorthUp[0].Length;
            if (width == 0)
            {
                throw new ArgumentException("Rows must not be empty.", nameof(rowsNorthUp));
            }

            var grid = MazeGrid.FromPhysicalDimensions(width, height);
            for (int row = 0; row < height; row++)
            {
                string line = rowsNorthUp[row];
                if (line.Length != width)
                {
                    throw new InvalidOperationException(
                        $"Row {row} must be {width} characters (got {line.Length})."
                    );
                }

                int y = height - 1 - row;
                for (int x = 0; x < width; x++)
                {
                    char cell = line[x];
                    if (cell != wallChar && cell != openChar)
                    {
                        throw new InvalidOperationException(
                            $"Unexpected character '{cell}' at row {row}, column {x}."
                        );
                    }

                    grid.SetCell(x, y, cell == wallChar ? MazeGrid.Wall : MazeGrid.Open);
                }
            }

            return grid;
        }
    }
}
