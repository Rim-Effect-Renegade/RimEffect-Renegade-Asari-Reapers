namespace RimEffectAsari
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
            AssertiveGene.InheritGenes inherit = null;
            if (!(AssertiveGene.CanInheritParentDominantGenes(father, ref inherit) & AssertiveGene.CanInheritParentDominantGenes(mother, ref inherit)) && inherit != null)
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
            AssertiveGene.dominantParent = null;
            AssertiveGene.InheritXenotype inherit = null;
            if (!(AssertiveGene.CanInheritParentDominantXenotype(mother, ref inherit) & AssertiveGene.CanInheritParentDominantXenotype(father, ref inherit)) && inherit != null)
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
            if (AssertiveGene.dominantParent != null)
            {
                __instance.iconDef = AssertiveGene.dominantParent.genes.iconDef;
                __instance.xenotypeName = AssertiveGene.dominantParent.genes.xenotypeName;
            }
        }
    }
}
