using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectAR.Harmony
{
    using HarmonyLib;
    using Verse;

    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.InPainShock), MethodType.Getter)]
    public static class HealthTrackerPainShock_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn ___pawn, ref bool __result)
        {
            if (___pawn.def == RimEffectARDefOf.RE_Husk_Brute)
                __result = false;
        }
    }
}
