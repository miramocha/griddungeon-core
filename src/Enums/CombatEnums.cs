namespace GridDungeon.Core.Enums
{
    public enum CombatantKind
    {
        Core,
        Summon,
        Guest,
        Enemy,
    }

    public enum FormationRow
    {
        Front,
        Back,
    }

    public enum SkillType
    {
        Physical,
        Elemental,
        Heal,
        Buff,
        Debuff,
        Deploy,
        Passive,
    }

    public enum DamageElement
    {
        None,
        Slash,
        Pierce,
        Fire,
        Ice,
        Volt,
    }

    public enum BodyPart
    {
        None,
        Head,
        Arm,
        Leg,
    }

    public enum BattleResult
    {
        Victory,
        Wipe,
        Flee,
    }

    public enum StatusCategory
    {
        Control,
        BindLimb,
        DoT,
        StatBuff,
        StatDebuff,
        BattleMod,
    }

    public enum SkillPresentation
    {
        Fixed,
        Cinematic,
        CinematicQTE,
    }

    public enum CombatPhase
    {
        Idle,
        CommandPlanning,
        TurnPhase,
        EndOfRound,
    }

    public enum CombatCommand
    {
        Attack,
        Guard,
        Skill,
        Item,
        Protocol,
        Flee,
    }

    public enum TargetKind
    {
        SingleEnemy,
        AllEnemies,
        SingleAlly,
        AllAllies,
        Self,
        AuxFront,
        AuxBack,
    }

    public enum ProtocolEffectType
    {
        DamageAllEnemies,
        HealAllAllies,
    }

    public enum EquipSlot
    {
        Weapon,
        Head,
        Body,
        Legs,
        Accessory,
    }

    public enum ItemEffectType
    {
        HealHp,
        HealMp,
        CureAilment,
        ReviveAlly,
        Identify,
    }

    public enum FeatureType
    {
        StairsDown,
        StairsUp,
        Door,
        Chest,
        GatherNode,
    }

    [System.Flags]
    public enum WallMask
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
    }
}
