using System;
using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Content
{
    [Serializable]
    public struct CharacterBaseStats
    {
        public int Hp;
        public int Mp;
        public int Str;
        public int Tec;
        public int Agi;
        public int Vit;
        public int Luc;
    }

    [Serializable]
    public struct TargetingRule
    {
        public TargetKind Kind;
        public bool CanTargetBack;
        public bool Pierce;
    }

    [Serializable]
    public struct StatusInflict
    {
        public string StatusId;
        public float Chance;
        public int DurationOverride;
    }

    [Serializable]
    public struct ElementResistances
    {
        public float Slash;
        public float Pierce;
        public float Fire;
        public float Ice;
        public float Volt;
    }

    public struct AuraModifiers
    {
        public float SynchroGainBonus;
    }

    public struct SummonAction
    {
        public string SkillId;
        public TargetKind TargetPreference;
    }

    [Serializable]
    public struct StatusResistBonuses
    {
        public float PoisonRes;
        public float SleepRes;
        public float PanicRes;
        public float BindHeadRes;
        public float BindArmRes;
        public float BindLegRes;
    }

    [Serializable]
    public struct FloorTileData
    {
        public bool IsWalkable;
        public bool IsBlockedPassage;
        public bool HasGatherNode;
        public string LootTableId;
        public string ChestItemId;
    }

    [Serializable]
    public struct PatrolWaypoint
    {
        public int X;
        public int Y;

        public GridPosition ToGrid() => new(X, Y);
    }

    [Serializable]
    public struct FoeSpawnConfig
    {
        public string FoeId;
        public int SpawnCellX;
        public int SpawnCellY;
        public PatrolWaypoint[] PatrolPath;
        public int StepsPerMove;
        public string EncounterGroupId;
        public bool TutorialFirstFoe;
        public bool NoFlee;

        public GridPosition SpawnCell => new(SpawnCellX, SpawnCellY);
    }

    [Serializable]
    public struct EncounterWeight
    {
        public string GroupId;
        public float Weight;
    }

    [Serializable]
    public struct EncounterTable
    {
        public EncounterWeight[] Entries;
    }

    [Serializable]
    public struct EnemySlotConfig
    {
        public string EnemyDefinitionId;
        public bool IsRequired;
    }

    [Serializable]
    public struct LootEntry
    {
        public string ItemId;
        public float DropChance;
        public int MinQty;
        public int MaxQty;
    }

    [Serializable]
    public struct LootTable
    {
        public LootEntry[] Entries;
    }
}
