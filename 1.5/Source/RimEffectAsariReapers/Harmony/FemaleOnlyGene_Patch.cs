using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace RimEffectAR
{
	[HarmonyPatch(typeof(PawnGenerator))]
	[HarmonyPatch("GeneratePawnRelations")]
	public static class PawnGenerator_GeneratePawnRelations_Patch
	{
		[HarmonyPrefix]
		public static bool DisableRelations(Pawn pawn)
		{
			Pawn_GeneTracker genes2 = pawn.genes;
			if (genes2 == null || !genes2.HasGene(RimEffectARDefOf.RE_FemaleOnly))
			{
				return true;
			}
			return false;
		}
	}
}
