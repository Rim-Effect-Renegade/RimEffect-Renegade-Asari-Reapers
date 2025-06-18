using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimEffectAR
{
    public class DarkSettlement : Settlement
    {
        public Faction OldFaction;

        public override IEnumerable<GenStepWithParams> ExtraGenStepDefs =>
            new[]
            {
                new GenStepWithParams(RimEffectARDefOf.RE_Reaperify, new GenStepParams()),
                new GenStepWithParams(RimEffectARDefOf.RE_DragonsTeeth, new GenStepParams())
            };

        public override string Label => base.Label + " (dark)";

        public override Material Material
        {
            get
            {
                var cachedMat = AccessTools.FieldRefAccess<Settlement, Material>(this, "cachedMat");
                if (cachedMat == null)
                    cachedMat = MaterialPool.MatFrom(OldFaction.def.settlementTexturePath,
                        ShaderDatabase.WorldOverlayTransparentLit, Faction.Color,
                        WorldMaterials.WorldObjectRenderQueue);

                return cachedMat;
            }
        }

        public override Texture2D ExpandingIcon => OldFaction.def.FactionIcon;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref OldFaction, "oldFaction");
        }
    }
}