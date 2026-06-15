using System;
using System.Collections.Generic;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    /// <summary>Queued player commands for living cores and player aux before AGI execution.</summary>
    public sealed class PartyCommandBatch
    {
        readonly Dictionary<string, CombatAction> m_actions = new();

        public int Count => m_actions.Count;

        public void Clear() => m_actions.Clear();

        public bool TryGet(string combatantId, out CombatAction action) =>
            m_actions.TryGetValue(combatantId, out action);

        public void Assign(Combatant combatant, CombatAction action) =>
            m_actions[combatant.Id] = action;

        public bool Remove(string combatantId) => m_actions.Remove(combatantId);

        public bool HasCommand(Combatant combatant) => m_actions.ContainsKey(combatant.Id);

        public bool HasCommand(string combatantId) => m_actions.ContainsKey(combatantId);

        public static IEnumerable<Combatant> LivingCores(Combatant?[] coreSlots)
        {
            foreach (Combatant? core in coreSlots)
            {
                if (core != null && !core.IsDead)
                {
                    yield return core;
                }
            }
        }

        public static IEnumerable<Combatant> LivingPlanningAux(
            Combatant?[] auxSlots,
            Func<Combatant, bool> usesPlanning
        )
        {
            foreach (Combatant? aux in auxSlots)
            {
                if (aux != null && !aux.IsDead && usesPlanning(aux))
                {
                    yield return aux;
                }
            }
        }

        public static bool AllLivingCoresAssigned(Combatant?[] coreSlots, PartyCommandBatch batch)
        {
            foreach (Combatant core in LivingCores(coreSlots))
            {
                if (!batch.HasCommand(core))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AllLivingPlanningAuxAssigned(
            Combatant?[] auxSlots,
            PartyCommandBatch batch,
            Func<Combatant, bool> usesPlanning
        )
        {
            foreach (Combatant aux in LivingPlanningAux(auxSlots, usesPlanning))
            {
                if (!batch.HasCommand(aux))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AllPlanningActorsAssigned(
            Combatant?[] coreSlots,
            Combatant?[] auxSlots,
            PartyCommandBatch batch,
            Func<Combatant, bool> usesAuxPlanning
        ) =>
            AllLivingCoresAssigned(coreSlots, batch)
            && AllLivingPlanningAuxAssigned(auxSlots, batch, usesAuxPlanning);

        public static Combatant? FirstUnassignedCore(
            Combatant?[] coreSlots,
            PartyCommandBatch batch
        )
        {
            foreach (Combatant core in LivingCores(coreSlots))
            {
                if (!batch.HasCommand(core))
                {
                    return core;
                }
            }

            return null;
        }

        public static Combatant? FirstUnassignedPlanningAux(
            Combatant?[] auxSlots,
            PartyCommandBatch batch,
            Func<Combatant, bool> usesPlanning
        )
        {
            foreach (Combatant aux in LivingPlanningAux(auxSlots, usesPlanning))
            {
                if (!batch.HasCommand(aux))
                {
                    return aux;
                }
            }

            return null;
        }

        /// <summary>Living cores first, then planning aux (front slot index before back).</summary>
        public static Combatant? FirstUnassignedPlanningActor(
            Combatant?[] coreSlots,
            Combatant?[] auxSlots,
            PartyCommandBatch batch,
            Func<Combatant, bool> usesAuxPlanning
        ) =>
            FirstUnassignedCore(coreSlots, batch)
            ?? FirstUnassignedPlanningAux(auxSlots, batch, usesAuxPlanning);

        /// <summary>True if any living core other than <paramref name="exceptCombatantId"/> queued Protocol.</summary>
        public bool AnyQueuedProtocolExcept(Combatant?[] coreSlots, string? exceptCombatantId)
        {
            foreach (Combatant core in LivingCores(coreSlots))
            {
                if (exceptCombatantId != null && core.Id == exceptCombatantId)
                {
                    continue;
                }

                if (
                    TryGet(core.Id, out CombatAction action)
                    && action.Command == CombatCommand.Protocol
                )
                {
                    return true;
                }
            }

            return false;
        }
    }
}
