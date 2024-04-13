namespace RimEffectAR
{
    using HarmonyLib;
    using RimWorld;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Verse;

    [HarmonyPatch]
    public static class GetInheritedGenes_Patch
    {
        public static MethodBase TargetMethod()
        {
            return (from x in typeof(PregnancyUtility).GetMethods()
                    where x.Name == "GetInheritedGenes"
                    select x).MaxBy((MethodInfo x) => x.GetParameters().Length);
        }

        [HarmonyPostfix]
        public static void Postfix(ref List<GeneDef> __result, Pawn father, Pawn mother)
        {
            DomGeneUtil.InheritGenes inherit = null;
            if (!(DomGeneUtil.CanInheritParentDominantGenes(father, ref inherit) & DomGeneUtil.CanInheritParentDominantGenes(mother, ref inherit)) && inherit != null)
            {
                __result.Clear();
                inherit?.Invoke(__result);
            }
        }
    }

    [HarmonyPatch(typeof(PregnancyUtility), "TryGetInheritedXenotype")]
    public static class TryGetInheritedXenotype_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn mother, Pawn father, ref XenotypeDef xenotype)
        {
            DomGeneUtil.dominantParent = null;
            DomGeneUtil.InheritXenotype inherit = null;
            if (!(DomGeneUtil.CanInheritParentDominantXenotype(mother, ref inherit) & DomGeneUtil.CanInheritParentDominantXenotype(father, ref inherit)) && inherit != null)
            {
                inherit?.Invoke(ref xenotype);
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_GeneTracker), "SetXenotypeDirect")]
    public static class SetXenotypeDirect_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_GeneTracker __instance, ref XenotypeDef xenotype)
        {
            if (DomGeneUtil.dominantParent != null)
            {
                __instance.iconDef = DomGeneUtil.dominantParent.genes.iconDef;
                __instance.xenotypeName = DomGeneUtil.dominantParent.genes.xenotypeName;
            }
        }
    }
}
