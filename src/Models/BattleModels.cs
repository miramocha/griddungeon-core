using System.Collections.Generic;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Models
{
    public sealed class BattleState
    {
        public Combatant[] CoreSlots { get; set; } = new Combatant[BattleFormation.MaxCoreSlots];
        public Combatant?[] AuxSlots { get; set; } = new Combatant?[BattleFormation.MaxAuxSlots];
        public Combatant[] EnemySlots { get; set; } = new Combatant[BattleFormation.MaxEnemySlots];
        public TurnQueue Queue { get; set; } = new();
        public int Round { get; set; }
        public CombatEntryContext Entry { get; set; } = new();
        public bool FleeEnabled { get; set; }

        /// <summary>Combat copy of party Synchro Charge (0..1).</summary>
        public float SynchroBar { get; set; }
    }

    public sealed class TurnQueue
    {
        List<Combatant> m_ordered = new();
        int m_index;

        public IReadOnlyList<Combatant> Ordered => m_ordered;

        public Combatant? Current =>
            m_ordered.Count == 0 || m_index >= m_ordered.Count ? null : m_ordered[m_index];

        public bool IsEmpty => m_ordered.Count == 0 || m_index >= m_ordered.Count;

        public void SetOrder(IReadOnlyList<Combatant> combatants)
        {
            m_ordered = new List<Combatant>(combatants);
            m_index = 0;
        }

        public void Advance()
        {
            m_index++;
        }

        public void Rebuild(IReadOnlyList<Combatant> combatants) => SetOrder(combatants);
    }

    public sealed class CombatEntryContext
    {
        public FoeInstance? Foe { get; set; }
        public string EncounterGroupId { get; set; } = string.Empty;
        public string BattleBackgroundId { get; set; } = string.Empty;
        public GridPosition FightAnchor { get; set; }
        public FacingDirection PartyFacing { get; set; }
        public bool NoFlee { get; set; }

        /// <summary>Grid contact fight (FOE on map), not a random ambush.</summary>
        public bool IsFoeContactFight => Foe != null;

        /// <summary>FOE contact fights push the party back one cell on flee (ADR 011; random encounters stay put).</summary>
        public bool ShouldMovePartyToRetreatCell(BattleResult result) =>
            result == BattleResult.Flee && IsFoeContactFight;
    }

    public sealed class CombatAction
    {
        public CombatCommand Command { get; set; }
        public string? SkillId { get; set; }
        public string? ItemId { get; set; }
        public string? TargetId { get; set; }
    }

    public sealed class CombatActionResult
    {
        public Combatant Actor { get; set; } = null!;
        public Combatant? Target { get; set; }
        public bool Hit { get; set; }
        public int DamageDealt { get; set; }
        public int HealingDone { get; set; }
        public string? StatusApplied { get; set; }
        public string? StatusCleansed { get; set; }
        public bool TargetDied { get; set; }

        /// <summary>Delta to Synchro Charge after this action (core only).</summary>
        public float SynchroBarDelta { get; set; }

        /// <summary>True when a flee attempt succeeded on this actor's AGI turn.</summary>
        public bool FleeSucceeded { get; set; }

        /// <summary>Set when a deploy skill successfully spawns a summon (ADR 016).</summary>
        public string? DeploySummonId { get; set; }
    }

    public sealed class RoundSnapshot
    {
        public BattleState State { get; set; } = null!;
        public IReadOnlyDictionary<string, Content.SkillData> Skills { get; set; } =
            new Dictionary<string, Content.SkillData>();

        public IReadOnlyDictionary<string, Content.StatusData> Statuses { get; set; } =
            new Dictionary<string, Content.StatusData>();
    }

    public sealed class RoundResult
    {
        public IReadOnlyList<string> LogLines { get; set; } = new List<string>();
    }
}
