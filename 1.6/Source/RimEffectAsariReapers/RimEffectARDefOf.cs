namespace RimEffectAR
{
    using RimWorld;
    using Verse;
    using Verse.AI;

    [DefOf]
    public static class RimEffectARDefOf
    {
        public static HediffDef 
            RE_Biotic_CharmedHediff,
            RE_Biotic_CharmHediff,
            RE_Biotic_WarpHediff,
            RE_Biotic_StasisHediff,
            RE_Biotic_SphereHediff, 
            RE_ArdatYakshi_Power,
            RE_ArdatYakshi_Partner,
            RER_DirectControl;

        public static ThingDef 
            RE_Biotic_SphereShield,
            RE_AsariShuttleIncoming,
            RE_AsariShuttleLeaving,
            RE_AsariShuttleLanded,
            RE_AbilityFlyer_Flight,
            RE_Husk_Banshee,
            RE_ReaperPowerBeam,
            RER_ReaperLongRangeMissile_Leaving,
            RER_ReaperLongRangeMissile_Arriving,
            RER_ReaperBeamGraphic,
            RE_Husk_Brute,
            RE_ReaperUnitStorage;

        public static TraitDef 
            RE_ArdatYakshi;

        public static FleckDef 
            RE_Fleck_EmbracingEternity,
            RER_ReaperGlow;

        public static ThoughtDef 
            RE_EmbracingEternity,
            RE_CharmThought;

        public static XenotypeDef 
            RE_Asari;

        public static GeneDef 
            RE_DominantGenes,
            RE_FemaleOnly;

        public static FactionDef 
            RE_AsariRepublics,
            RE_Reapers;

        public static JobDef 
            RE_EnterShuttle,
            RE_BansheeTeleport;

        public static DutyDef 
            RE_LeaveOnShuttle,
            RE_LeaveOnShuttleAndDefendSelf;

        public static SoundDef 
            RE_Pawn_Husk_Banshee_Call;

        public static FleshTypeDef 
            RE_Husk;

        public static WeatherDef 
            RER_ReaperLightningStorm;

        public static GameConditionDef 
            RER_ReaperWeather;

        public static SoundDef 
            RE_Launch_ReaperLongRangeMissile,
            RE_Incoming_ReaperLongRangeMissile,
            RE_Targeting_ReaperOrbitalBeam;

        public static WorldObjectDef RE_Reaper;

        public static GenStepDef 
            RE_Reaperify,
            RE_DragonsTeeth;

        public static RulePackDef RE_NameGenerate;
    }
}