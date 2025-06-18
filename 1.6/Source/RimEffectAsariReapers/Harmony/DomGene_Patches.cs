using HarmonyLib;
using RimWorld;
using System;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
using static RimEffectAR.DomGeneUtil;

namespace RimEffectAR
{
    public static class DomGene_Patches
    {
        private static int ranNum;

        [HarmonyPatch(typeof(PregnancyUtility), "GetInheritedGenes", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
        public static class RE_GetInheritedGenes_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn father, Pawn mother, ref List<GeneDef> __result)
            {
                InheritGenes inherit = null;
                System.Random random = new System.Random();
                if (CanInheritParentDominantGenes(father, ref inherit) & CanInheritParentDominantGenes(mother, ref inherit))
                {
                    inherit = null;
                    ranNum = random.Next(0, 2); ;
                    if (ranNum == 0)
                    {
                        CanInheritParentDominantGenes(mother, ref inherit);
                    }
                    else
                    {
                        CanInheritParentDominantGenes(father, ref inherit);
                    }
                }
                if (inherit is null) { return; }
                inherit?.Invoke(__result);
            }
        }

        [HarmonyPatch(typeof(PregnancyUtility), nameof(TryGetInheritedXenotype))]
        [HarmonyPostfix]
        public static void TryGetInheritedXenotype(ref bool __result, Pawn mother, Pawn father, ref XenotypeDef xenotype)
        {
            domParent = null;
            InheritXenotype inherit = null;
            if (CanInheritParentDominantXenotype(mother, ref inherit) & CanInheritParentDominantXenotype(father, ref inherit))
            {
                inherit = null;
                if (ranNum == 0)
                {

                    CanInheritParentDominantXenotype(mother, ref inherit);
                }
                else
                {
                    CanInheritParentDominantXenotype(father, ref inherit);
                }
            }
            if (inherit is null) { return; }
            inherit?.Invoke(ref xenotype);
            __result = true;
        }

        [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(SetXenotypeDirect))]
        [HarmonyPostfix]
        public static void SetXenotypeDirect(Pawn_GeneTracker __instance, ref XenotypeDef xenotype)
        {
            if (domParent is null) { return; }
            __instance.iconDef = domParent.genes.iconDef;
            __instance.xenotypeName = domParent.genes.xenotypeName;
        }
    }
}
