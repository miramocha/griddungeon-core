using GridDungeon.Core.Content;
using GridDungeon.Core.Enums;
using GridDungeon.Core.Formation;
using GridDungeon.Core.Models;

namespace GridDungeon.Core.Simulators
{
    public static class SummonDeployResolver
    {
        public static bool IsAuxSlotOccupied(BattleState state, FormationRow auxRow)
        {
            int index = AuxSlotIndex.FromRow(auxRow);
            if (index < 0 || index >= state.AuxSlots.Length)
            {
                return true;
            }

            Combatant? occupant = state.AuxSlots[index];
            return occupant != null && !occupant.IsDead;
        }

        public static bool CanDeploy(BattleState state, SkillData skill)
        {
            if (skill.Type != SkillType.Deploy || string.IsNullOrEmpty(skill.SummonDefinitionId))
            {
                return false;
            }

            if (
                !Mvp1SummonCatalog.TryGetSummonData(skill.SummonDefinitionId, out SummonData summon)
            )
            {
                return false;
            }

            return !IsAuxSlotOccupied(state, summon.AuxRow);
        }
    }
}
