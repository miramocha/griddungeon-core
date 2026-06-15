using System;
using System.Collections.Generic;
using GridDungeon.Core.Inventory;

namespace GridDungeon.Core.SaveData
{
    public static class SaveGameMapper
    {
        public static SaveGameWire ToWire(SaveGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            var wire = new SaveGameWire
            {
                Hub = ToWireHub(game.Hub ?? new HubSaveData()),
                PartyInventory = PartyInventoryNormalizer.Normalize(game.PartyInventory).Clone(),
                Party = game.Party ?? Array.Empty<CharacterSaveData>(),
                ActiveNavigatorId = game.ActiveNavigatorId ?? string.Empty,
                SynchroBar = game.SynchroBar,
                Campaign = game.Campaign ?? new CampaignSaveData(),
                HasExploration = game.Exploration.HasValue,
                Exploration = game.Exploration ?? default,
                Maps = ToFloorMapEntries(game.Maps),
                FoeState = ToFoeStateEntries(game.FoeState),
            };
            return wire;
        }

        public static SaveGame FromWire(SaveGameWire wire)
        {
            if (wire == null)
            {
                throw new ArgumentNullException(nameof(wire));
            }

            CharacterSaveData[] party = wire.Party ?? Array.Empty<CharacterSaveData>();
            NormalizePartyVitals(party);

            CampaignSaveData campaign = wire.Campaign ?? new CampaignSaveData();
            CampaignSaveData.NormalizeLegacyJsonFields(campaign);

            var game = new SaveGame
            {
                Hub = FromWireHub(wire.Hub),
                PartyInventory = PartyInventoryNormalizer.Normalize(wire.PartyInventory),
                Party = party,
                ActiveNavigatorId = wire.ActiveNavigatorId ?? string.Empty,
                SynchroBar = wire.SynchroBar,
                Campaign = campaign,
                Exploration = wire.HasExploration ? wire.Exploration : null,
            };

            game.Maps = FromFloorMapEntries(wire.Maps);
            game.FoeState = FromFoeStateEntries(wire.FoeState);
            return game;
        }

        static HubSaveWire ToWireHub(HubSaveData hub)
        {
            var entries = new List<UnlockedFloorEntry>(hub.UnlockedFloors.Count);
            foreach (KeyValuePair<string, string> entry in hub.UnlockedFloors)
            {
                entries.Add(
                    new UnlockedFloorEntry { LocationId = entry.Key, FloorId = entry.Value }
                );
            }

            entries.Sort(
                (a, b) => string.Compare(a.LocationId, b.LocationId, StringComparison.Ordinal)
            );
            return new HubSaveWire
            {
                Credits = hub.Credits,
                UnlockedFloors = entries.ToArray(),
                GuildRoster = hub.GuildRoster ?? Array.Empty<CharacterSaveData>(),
            };
        }

        static HubSaveData FromWireHub(HubSaveWire? hub)
        {
            var runtime = new HubSaveData();
            if (hub == null)
            {
                return runtime;
            }

            runtime.Credits = hub.Credits;
            CharacterSaveData[] roster = hub.GuildRoster ?? Array.Empty<CharacterSaveData>();
            NormalizePartyVitals(roster);
            runtime.GuildRoster = roster;
            if (hub.UnlockedFloors == null)
            {
                return runtime;
            }

            foreach (UnlockedFloorEntry entry in hub.UnlockedFloors)
            {
                if (string.IsNullOrEmpty(entry.LocationId))
                {
                    continue;
                }

                runtime.UnlockedFloors[entry.LocationId] = entry.FloorId ?? string.Empty;
            }

            return runtime;
        }

        static FloorMapEntry[] ToFloorMapEntries(Dictionary<string, FloorMapStateSave> maps)
        {
            if (maps == null || maps.Count == 0)
            {
                return Array.Empty<FloorMapEntry>();
            }

            var entries = new FloorMapEntry[maps.Count];
            int i = 0;
            foreach (KeyValuePair<string, FloorMapStateSave> kv in maps)
            {
                entries[i++] = new FloorMapEntry { FloorKey = kv.Key, State = kv.Value };
            }

            Array.Sort(
                entries,
                (a, b) => string.Compare(a.FloorKey, b.FloorKey, StringComparison.Ordinal)
            );
            return entries;
        }

        static Dictionary<string, FloorMapStateSave> FromFloorMapEntries(FloorMapEntry[]? entries)
        {
            var maps = new Dictionary<string, FloorMapStateSave>();
            if (entries == null)
            {
                return maps;
            }

            foreach (FloorMapEntry entry in entries)
            {
                if (string.IsNullOrEmpty(entry.FloorKey))
                {
                    continue;
                }

                maps[entry.FloorKey] = entry.State ?? new FloorMapStateSave();
            }

            return maps;
        }

        static FoeStateEntry[] ToFoeStateEntries(Dictionary<string, FloorFoeStateSave> foes)
        {
            if (foes == null || foes.Count == 0)
            {
                return Array.Empty<FoeStateEntry>();
            }

            var entries = new FoeStateEntry[foes.Count];
            int i = 0;
            foreach (KeyValuePair<string, FloorFoeStateSave> kv in foes)
            {
                FloorFoeStateSave floorState = kv.Value ?? new FloorFoeStateSave();
                entries[i++] = new FoeStateEntry
                {
                    FloorKey = kv.Key,
                    PartyStepCount = floorState.PartyStepCount,
                    Foes = floorState.Foes ?? Array.Empty<FoeStateSave>(),
                };
            }

            Array.Sort(
                entries,
                (a, b) => string.Compare(a.FloorKey, b.FloorKey, StringComparison.Ordinal)
            );
            return entries;
        }

        static Dictionary<string, FloorFoeStateSave> FromFoeStateEntries(FoeStateEntry[]? entries)
        {
            var foes = new Dictionary<string, FloorFoeStateSave>();
            if (entries == null)
            {
                return foes;
            }

            foreach (FoeStateEntry entry in entries)
            {
                if (string.IsNullOrEmpty(entry.FloorKey))
                {
                    continue;
                }

                foes[entry.FloorKey] = new FloorFoeStateSave
                {
                    PartyStepCount = entry.PartyStepCount,
                    Foes = entry.Foes ?? Array.Empty<FoeStateSave>(),
                };
            }

            return foes;
        }

        static void NormalizePartyVitals(CharacterSaveData[] members)
        {
            if (members == null)
            {
                return;
            }

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i] != null)
                {
                    CharacterSaveVitals.NormalizeLegacyJsonFields(members[i]);
                }
            }
        }
    }
}
