using HarmonyLib;
using Verse;
using Verse.AI.Group;

namespace RimEffectAR
{
    [HarmonyPatch(typeof(Trigger_UrgentlyHungry), "ActivateOn")]
    public class Trigger_UrgentlyHungry_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Lord lord, ref bool __result)
        {
            if (lord.faction.def != RimEffectARDefOf.RE_Reapers) return true;
            __result = false;
            return false;
        }
    }
}