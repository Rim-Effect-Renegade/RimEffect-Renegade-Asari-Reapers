namespace RimEffectAR
{
    using RimWorld;
    using Verse;

    public class StatWorker_ArdatYakshiPower : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req) => base.ShouldShowFor(req) && ((req.Thing as Pawn)?.story?.traits?.HasTrait(RimEffectARDefOf.RE_ArdatYakshi) ?? false);

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            float valueUnfinalized = base.GetValueUnfinalized(req, applyPostProcess);

            if((req.Thing as Pawn)?.story?.traits?.HasTrait(RimEffectARDefOf.RE_ArdatYakshi) ?? false)
                valueUnfinalized *= (req.Thing as Pawn)?.health.hediffSet.GetFirstHediffOfDef(RimEffectARDefOf.RE_ArdatYakshi_Power)?.Severity ?? 1;

            return valueUnfinalized;
        }
    }
}