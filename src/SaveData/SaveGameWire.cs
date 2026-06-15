using System;
using GridDungeon.Core.Inventory;

namespace GridDungeon.Core.SaveData
{
    /// <summary>JSON-serializable snapshot; dictionaries are list-encoded for Unity JsonUtility.</summary>
    [Serializable]
    public sealed class SaveGameWire
    {
        public HubSaveWire Hub = new();
        public PartyInventory PartyInventory = PartyInventory.CreateEmpty();
        public CharacterSaveData[] Party = Array.Empty<CharacterSaveData>();
        public string ActiveNavigatorId = string.Empty;
        public float SynchroBar;
        public CampaignSaveData Campaign = new();
        public FloorMapEntry[] Maps = Array.Empty<FloorMapEntry>();
        public FoeStateEntry[] FoeState = Array.Empty<FoeStateEntry>();
        public bool HasExploration;
        public ExplorationStateSave Exploration;
    }

    [Serializable]
    public struct FloorMapEntry
    {
        public string FloorKey;
        public FloorMapStateSave State;
    }

    [Serializable]
    public struct FoeStateEntry
    {
        public string FloorKey;
        public int PartyStepCount;
        public FoeStateSave[] Foes;
    }

    [Serializable]
    public sealed class HubSaveWire
    {
        public int Credits;
        public UnlockedFloorEntry[] UnlockedFloors = Array.Empty<UnlockedFloorEntry>();
        public CharacterSaveData[] GuildRoster = Array.Empty<CharacterSaveData>();
    }

    [Serializable]
    public struct UnlockedFloorEntry
    {
        public string LocationId;
        public string FloorId;
    }
}
