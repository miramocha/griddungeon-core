using System;
using System.Collections.Generic;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Inventory;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.SaveData
{
    [Serializable]
    public sealed class SaveGame
    {
        public HubSaveData Hub = new();
        public PartyInventory PartyInventory = PartyInventory.CreateEmpty();
        public CharacterSaveData[] Party = Array.Empty<CharacterSaveData>();
        public string ActiveNavigatorId = string.Empty;
        public float SynchroBar;
        public CampaignSaveData Campaign = new();
        public Dictionary<string, FloorMapStateSave> Maps = new();
        public Dictionary<string, FloorFoeStateSave> FoeState = new();
        public ExplorationStateSave? Exploration;
    }

    [Serializable]
    public sealed class HubSaveData
    {
        public int Credits;
        public Dictionary<string, string> UnlockedFloors = new();
        public CharacterSaveData[] GuildRoster = Array.Empty<CharacterSaveData>();
    }

    [Serializable]
    public struct ExplorationStateSave
    {
        public string LocationId;
        public string FloorId;
        public int PartyCellX;
        public int PartyCellY;
        public FacingDirection Facing;
    }

    [Serializable]
    public sealed class CharacterSaveData
    {
        public string CharacterId = string.Empty;
        public string Name = string.Empty;
        public string ClassId = string.Empty;
        public int Level;
        public int Experience;
        public int AllocatedSkillPoints;
        public string[] AllocatedSkillIds = Array.Empty<string>();

        /// <summary>-1 = unset (pre-vitals saves); load uses max HP/MP.</summary>
        public int CurrentHp = CharacterSaveVitals.Unset;

        /// <summary>-1 = unset (pre-vitals saves); load uses max HP/MP.</summary>
        public int CurrentMp = CharacterSaveVitals.Unset;

        /// <summary>True after a snapshot write; distinguishes legacy JsonUtility 0/0 from real KO vitals.</summary>
        public bool VitalsSerialized;

        public EquipmentLoadout Equipment = new();
    }

    [Serializable]
    public sealed class FloorMapStateSave
    {
        public List<int> Visited = new();
        public List<int> Walls = new();
        public List<FeatureStateSave> Features = new();
        public List<FoeIconSave> FoeIcons = new();
    }

    [Serializable]
    public struct FeatureStateSave
    {
        public int X;
        public int Y;
        public int Type;
        public bool IsInteracted;
    }

    [Serializable]
    public struct FoeIconSave
    {
        public int X;
        public int Y;
        public string FoeId;
    }

    [Serializable]
    public sealed class FloorFoeStateSave
    {
        public int PartyStepCount;
        public FoeStateSave[] Foes = Array.Empty<FoeStateSave>();
    }

    [Serializable]
    public struct FoeStateSave
    {
        public string Id;
        public int CellX;
        public int CellY;
        public bool Alive;
        public int PatrolPathIndex;
    }
}
