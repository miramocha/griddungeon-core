using System;
using System.Collections.Generic;

namespace GridDungeon.Core.Exploration
{
    /// <summary>Pure catalog matching for floor transition beats (Edit Mode testable).</summary>
    public static class FloorTransitionCatalogResolve
    {
        public readonly struct Row
        {
            public Row(
                string beatId,
                string leaveFloorKey,
                string enterFloorKey,
                float durationMaxSeconds
            )
            {
                BeatId = beatId ?? string.Empty;
                LeaveFloorKey = leaveFloorKey ?? string.Empty;
                EnterFloorKey = enterFloorKey ?? string.Empty;
                DurationMaxSeconds = durationMaxSeconds;
            }

            public string BeatId { get; }
            public string LeaveFloorKey { get; }
            public string EnterFloorKey { get; }
            public float DurationMaxSeconds { get; }
        }

        public readonly struct Resolved
        {
            public Resolved(int rowIndex, Row row)
            {
                RowIndex = rowIndex;
                Row = row;
            }

            public int RowIndex { get; }
            public Row Row { get; }
        }

        public static bool TryResolve(
            IReadOnlyList<Row> rows,
            string beatId,
            string leaveFloorKey,
            string enterFloorKey,
            out Resolved resolved
        )
        {
            resolved = default;
            if (rows == null || rows.Count == 0)
            {
                return false;
            }

            int bestScore = int.MinValue;
            int bestIndex = -1;
            Row bestRow = default;

            for (int i = 0; i < rows.Count; i++)
            {
                Row row = rows[i];
                if (!MatchesBeatId(row, beatId))
                {
                    continue;
                }

                if (!MatchesFloorKey(row.LeaveFloorKey, leaveFloorKey))
                {
                    continue;
                }

                if (!MatchesFloorKey(row.EnterFloorKey, enterFloorKey))
                {
                    continue;
                }

                int score = ScoreRow(row, beatId, leaveFloorKey, enterFloorKey);
                if (score <= bestScore)
                {
                    continue;
                }

                bestScore = score;
                bestIndex = i;
                bestRow = row;
            }

            if (bestIndex < 0)
            {
                return false;
            }

            resolved = new Resolved(bestIndex, bestRow);
            return true;
        }

        static bool MatchesBeatId(Row row, string beatId)
        {
            if (string.IsNullOrEmpty(beatId))
            {
                return string.IsNullOrEmpty(row.BeatId);
            }

            return string.IsNullOrEmpty(row.BeatId) || row.BeatId == beatId;
        }

        static bool MatchesFloorKey(string rowKey, string requestKey)
        {
            if (string.IsNullOrEmpty(rowKey))
            {
                return true;
            }

            return rowKey == requestKey;
        }

        static int ScoreRow(Row row, string beatId, string leaveFloorKey, string enterFloorKey)
        {
            int score = 0;
            if (!string.IsNullOrEmpty(row.BeatId) && row.BeatId == beatId)
            {
                score += 100;
            }

            if (!string.IsNullOrEmpty(row.LeaveFloorKey) && row.LeaveFloorKey == leaveFloorKey)
            {
                score += 10;
            }

            if (!string.IsNullOrEmpty(row.EnterFloorKey) && row.EnterFloorKey == enterFloorKey)
            {
                score += 10;
            }

            return score;
        }
    }
}
