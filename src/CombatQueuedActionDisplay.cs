using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core
{
    /// <summary>Player-facing label for a queued command on the party roster.</summary>
    public static class CombatQueuedActionDisplay
    {
        public static string Format(
            CombatAction action,
            string? skillDisplayName = null,
            string? protocolDisplayName = null
        )
        {
            switch (action.Command)
            {
                case CombatCommand.Attack:
                    return "Attack";
                case CombatCommand.Guard:
                    return "Guard";
                case CombatCommand.Flee:
                    return "Flee";
                case CombatCommand.Item:
                    return "Item";
                case CombatCommand.Skill:
                    if (!string.IsNullOrEmpty(skillDisplayName))
                    {
                        return skillDisplayName;
                    }

                    return string.IsNullOrEmpty(action.SkillId) ? "Skill" : action.SkillId;
                case CombatCommand.Protocol:
                    if (!string.IsNullOrEmpty(protocolDisplayName))
                    {
                        return protocolDisplayName;
                    }

                    return "Protocol";
                default:
                    return action.Command.ToString();
            }
        }
    }
}
