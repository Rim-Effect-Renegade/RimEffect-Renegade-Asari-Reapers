﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace RimEffectReapers
{
    [HarmonyPatch(typeof(StunHandler), "AffectedByEMP", MethodType.Getter)]
    public class Patch_StunHandler_AffectedByEMP
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, ref StunHandler __instance, Thing ___parent)
        {
            if (___parent is Pawn)
                if (___parent.def.race.FleshType == RER_DefOf.RE_Husk)
                {
                    __result = true;
                }
        }
    }
}