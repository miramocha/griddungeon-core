using System.Collections.Generic;
using GridDungeon.Core.Content;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class CombatSimulator
    {
        public static List<Combatant> BuildTurnQueue(IEnumerable<Combatant> combatants) =>
            TurnQueueBuilder.Build(combatants);

        public static List<Combatant> BuildTurnQueue(
            IEnumerable<Combatant> combatants,
            IReadOnlyDictionary<string, StatusData>? statuses
        ) => TurnQueueBuilder.Build(combatants, statuses);

        public static float GetEffectiveAgi(Combatant combatant) =>
            TurnQueueBuilder.GetEffectiveAgi(combatant);

        public static int CalculatePhysical(
            Combatant attacker,
            Combatant defender,
            SkillData skill,
            IReadOnlyList<BattleModifier> mods
        ) => DamageCalculator.CalculatePhysical(attacker, defender, skill, mods);

        public static int CalculateElemental(
            Combatant attacker,
            Combatant defender,
            SkillData skill,
            ElementResistances resistances
        ) => DamageCalculator.CalculateElemental(attacker, defender, skill, resistances);

        public static int CalculateHeal(Combatant caster, SkillData skill) =>
            DamageCalculator.CalculateHeal(caster, skill);

        public static RoundResult SimulateRound(RoundSnapshot snapshot)
        {
            BattleState state = snapshot.State;
            state.Round++;
            List<string> log = EndOfRoundSimulator.Execute(state, snapshot.Statuses, state.Round);
            return new RoundResult { LogLines = log };
        }
    }
}
