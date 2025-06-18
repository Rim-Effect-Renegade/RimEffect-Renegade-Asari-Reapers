using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RimEffectAR
{
    public class GenStep_DragonsTeeth : GenStep_Scatterer
    {
        public override int SeedPart => 98762345;

        public override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
        {
            GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.MineableGold), loc, map, ThingPlaceMode.Direct);
        }

        public override bool CanScatterAt(IntVec3 loc, Map map)
        {
            return base.CanScatterAt(loc, map);
        }
    }
}