using GridDungeon.Core.Enums;

namespace GridDungeon.Core.Content
{
    public readonly struct SkillData
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string DescriptionEn { get; }
        public SkillType Type { get; }
        public DamageElement Element { get; }
        public BodyPart BodyPart { get; }
        public int MpCost { get; }
        public float Power { get; }
        public TargetingRule Targeting { get; }
        public StatusInflict? Inflict { get; }
        public SkillUseContext UseContexts { get; }

        /// <summary>Deploy skills — summon to spawn (e.g. <c>scout_drone</c>).</summary>
        public string SummonDefinitionId { get; }

        public SkillData(
            string id,
            SkillType type,
            DamageElement element,
            BodyPart bodyPart,
            int mpCost,
            float power,
            TargetingRule targeting,
            StatusInflict? inflict,
            string displayName = "",
            SkillUseContext useContexts = SkillUseContext.Combat | SkillUseContext.Field,
            string summonDefinitionId = "",
            string descriptionEn = ""
        )
        {
            Id = id;
            DisplayName = displayName;
            DescriptionEn = descriptionEn ?? string.Empty;
            Type = type;
            Element = element;
            BodyPart = bodyPart;
            MpCost = mpCost;
            Power = power;
            Targeting = targeting;
            Inflict = inflict;
            UseContexts = useContexts;
            SummonDefinitionId = summonDefinitionId;
        }
    }

    public readonly struct StatusData
    {
        public string Id { get; }
        public StatusCategory Category { get; }
        public int DefaultDurationTurns { get; }
        public float Magnitude { get; }
        public BodyPart BindPart { get; }
        public bool RemovedOnDamage { get; }

        public StatusData(
            string id,
            StatusCategory category,
            int defaultDurationTurns,
            float magnitude,
            BodyPart bindPart,
            bool removedOnDamage
        )
        {
            Id = id;
            Category = category;
            DefaultDurationTurns = defaultDurationTurns;
            Magnitude = magnitude;
            BindPart = bindPart;
            RemovedOnDamage = removedOnDamage;
        }
    }

    public readonly struct NavigatorData
    {
        public string Id { get; }
        public AuraModifiers Aura { get; }
        public string[] ProtocolSkillIds { get; }

        public NavigatorData(string id, AuraModifiers aura, string[] protocolSkillIds)
        {
            Id = id;
            Aura = aura;
            ProtocolSkillIds = protocolSkillIds;
        }
    }

    public readonly struct ProtocolSkillData
    {
        public string Id { get; }
        public ProtocolEffectType EffectType { get; }
        public float Power { get; }
        public int ParticipantCount { get; }

        public ProtocolSkillData(
            string id,
            ProtocolEffectType effectType,
            float power,
            int participantCount
        )
        {
            Id = id;
            EffectType = effectType;
            Power = power;
            ParticipantCount = participantCount;
        }
    }

    public readonly struct EnemyData
    {
        public string Id { get; }
        public string DisplayName { get; }
        public CharacterBaseStats Stats { get; }
        public ElementResistances Resistances { get; }
        public string[] SkillIds { get; }
        public bool NoFlee { get; }
        public string[] StatusImmuneTags { get; }
        public int XpReward { get; }
        public LootTable LootTable { get; }

        public EnemyData(
            string id,
            CharacterBaseStats stats,
            ElementResistances resistances,
            string[] skillIds,
            bool noFlee,
            string[] statusImmuneTags,
            string displayName = "",
            int xpReward = 0,
            LootTable lootTable = default
        )
        {
            Id = id;
            DisplayName = displayName;
            Stats = stats;
            Resistances = resistances;
            SkillIds = skillIds;
            NoFlee = noFlee;
            StatusImmuneTags = statusImmuneTags;
            XpReward = xpReward;
            LootTable = lootTable;
        }
    }

    public readonly struct SummonData
    {
        public string Id { get; }
        public string DisplayName { get; }
        public CharacterBaseStats Stats { get; }
        public int DurationRounds { get; }
        public FormationRow AuxRow { get; }
        public SummonAction[] ActionScript { get; }
        public string[] SkillIds { get; }

        public SummonData(
            string id,
            CharacterBaseStats stats,
            int durationRounds,
            FormationRow auxRow,
            SummonAction[] actionScript,
            string displayName = "",
            string[]? skillIds = null
        )
        {
            Id = id;
            DisplayName = displayName;
            Stats = stats;
            DurationRounds = durationRounds;
            AuxRow = auxRow;
            ActionScript = actionScript;
            SkillIds = skillIds ?? System.Array.Empty<string>();
        }
    }
}
