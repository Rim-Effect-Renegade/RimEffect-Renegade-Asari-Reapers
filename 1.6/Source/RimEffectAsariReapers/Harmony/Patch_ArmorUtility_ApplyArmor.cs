namespace RimEffectAR
{
    using HarmonyLib;
    using RimWorld;
    using Verse;

    [HarmonyPatch(typeof(ArmorUtility), "ApplyArmor")]
    public static class Patch_ArmorUtility_ApplyArmor
    {
        [HarmonyPrefix]
        public static void Prefix(ref float armorRating, Pawn pawn)
        {
            if (pawn.health.hediffSet.HasHediff(RimEffectARDefOf.RE_Biotic_WarpHediff))
                armorRating = 0f;
        }
    }
}
