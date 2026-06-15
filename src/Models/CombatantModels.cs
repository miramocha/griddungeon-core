using System;
using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Models
{
    public sealed class Combatant
    {
        public string Id { get; set; } = string.Empty;
        public string DefinitionId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public CombatantKind Kind { get; set; }
        public FormationRow Row { get; set; }
        public int SlotIndex { get; set; }
        public int Level { get; set; } = 1;
        public int Experience { get; set; }

        public CombatantStats Stats { get; set; }
        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }

        public List<StatusInstance> Statuses { get; } = new();
        public List<BattleModifier> BattleMods { get; } = new();
        public List<string> AllocatedSkillIds { get; } = new();
        public EquipmentLoadout? Equipment { get; set; }

        /// <summary>Summon duration in combat rounds; 0 when not a timed summon.</summary>
        public int SummonTurnsRemaining { get; set; }

        public bool IsDead => CurrentHp <= 0;

        /// <summary>Core at 0 HP but still in formation until fight ends or revived.</summary>
        public bool IsDowned => Kind == CombatantKind.Core && CurrentHp <= 0;

        public bool IsAux => Kind is CombatantKind.Summon or CombatantKind.Guest;

        /// <summary>
        /// Core, player summons, and player guests. NPC guests with an action script use
        /// <see cref="GridDungeon.Core.Simulators.SummonScriptRunner"/> instead.
        /// </summary>
        public bool IsPlayerControlled =>
            Kind is CombatantKind.Core or CombatantKind.Summon or CombatantKind.Guest;
    }

    public struct CombatantStats
    {
        public int Hp;
        public int Mp;
        public int Str;
        public int Tec;
        public int Agi;
        public int Vit;
        public int Luc;
    }

    public sealed class StatusInstance
    {
        public string DefinitionId { get; set; } = string.Empty;
        public int TurnsRemaining { get; set; }
        public int AppliedRound { get; set; }
        public string SourceCombatantId { get; set; } = string.Empty;
    }

    public sealed class BattleModifier
    {
        public string ModId { get; set; } = string.Empty;
        public float Magnitude { get; set; }
        public int TurnsRemaining { get; set; } = -1;
    }

    [Serializable]
    public sealed class EquipmentLoadout
    {
        public string WeaponId { get; set; } = string.Empty;
        public string HeadId { get; set; } = string.Empty;
        public string BodyId { get; set; } = string.Empty;
        public string LegsId { get; set; } = string.Empty;
        public string AccessoryId { get; set; } = string.Empty;
    }
}
