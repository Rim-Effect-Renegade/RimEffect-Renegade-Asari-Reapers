using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimEffectAR
{
    public class IncidentWorker_DarkSettlementReclaimed : IncidentWorker
    {
        public override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.WorldObjects.Settlements.Any(s => s is DarkSettlement) && Find.FactionManager.AllFactions.Any(
                f => !f.defeated && !f.def.isPlayer && !f.Hidden && !f.temporary &&
                     f.def != RimEffectARDefOf.RE_Reapers);
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!(Find.WorldObjects.Settlements.Where(s => s is DarkSettlement).RandomElementWithFallback() is
                DarkSettlement darkSettlement)) return false;
            var faction = Find.FactionManager.AllFactions
                .Where(f => !f.defeated && !f.def.isPlayer && !f.Hidden && !f.temporary &&
                            f.def != RimEffectARDefOf.RE_Reapers)
                .RandomElementByWeightWithFallback(f => f == darkSettlement.OldFaction ? 10 : 1);
            if (faction == null) return false;
            var settlement = (Settlement) WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            settlement.Tile = darkSettlement.Tile;
            settlement.SetFaction(faction);
            settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement);

            darkSettlement.Destroy();
            Find.WorldObjects.Add(settlement);

            SendStandardLetter(def.letterLabel + ": " + darkSettlement.Label, def.letterText.Formatted(faction),
                def.letterDef, parms, settlement);

            return true;
        }
    }
}