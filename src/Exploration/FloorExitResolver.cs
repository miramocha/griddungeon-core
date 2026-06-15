using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Exploration
{
    /// <summary>Resolve compiled <see cref="FloorExitLink"/> rows at interact cells (Core-only).</summary>
    public static class FloorExitResolver
    {
        public static bool TryGetExitAt(
            FloorExitLink[] exitLinks,
            GridPosition cell,
            out FloorExitLink link,
            FloorExitDirection? requiredDirection = null
        )
        {
            link = default;
            if (exitLinks == null || exitLinks.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < exitLinks.Length; i++)
            {
                FloorExitLink candidate = exitLinks[i];
                if (candidate.Cell != cell)
                {
                    continue;
                }

                if (requiredDirection.HasValue && candidate.Direction != requiredDirection.Value)
                {
                    continue;
                }

                link = candidate;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Builds floor transition target from a <see cref="FloorExitTargetKind.Floor"/> link.
        /// Hub links use <see cref="FloorExitTargetKind.Hub"/> — return false; runtime calls hub transition separately.
        /// </summary>
        public static bool TryToExplorationTarget(FloorExitLink link, out ExplorationTarget target)
        {
            target = default;
            if (link.TargetKind != FloorExitTargetKind.Floor)
            {
                return false;
            }

            if (!TryParseFloorKey(link.TargetFloorKey, out string locationId, out string floorId))
            {
                return false;
            }

            target = new ExplorationTarget(
                locationId,
                floorId,
                link.TargetFloorKey,
                link.TargetSpawnCell,
                link.TargetFacing
            );
            return true;
        }

        static bool TryParseFloorKey(string floorKey, out string locationId, out string floorId)
        {
            locationId = string.Empty;
            floorId = string.Empty;
            if (string.IsNullOrEmpty(floorKey))
            {
                return false;
            }

            int separator = floorKey.IndexOf('_');
            if (separator <= 0 || separator >= floorKey.Length - 1)
            {
                return false;
            }

            locationId = floorKey.Substring(0, separator);
            floorId = floorKey.Substring(separator + 1);
            return true;
        }
    }
}
