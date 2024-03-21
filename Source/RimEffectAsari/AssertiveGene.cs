using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimEffectAsari
{
    public static class AssertiveGene
	{
		public delegate void InheritGenes(List<GeneDef> genes);

		public delegate void InheritXenotype(ref XenotypeDef xenotype);

		internal static Pawn dominantParent;

		public static List<Gene> GetEndogenes(this Pawn pawn)
		{
			return pawn.GetGenes((Pawn p) => p?.genes?.Endogenes);
		}

		public static List<Gene> GetXenogenes(this Pawn pawn)
		{
			return pawn.GetGenes((Pawn p) => p?.genes?.Xenogenes);
		}

		public static List<Gene> GetGenes(this Pawn pawn, Func<Pawn, List<Gene>> getter)
		{
			IEnumerable<Gene> enumerable = getter(pawn);
			return (enumerable ?? Enumerable.Empty<Gene>()).ToList();
		}

		public static bool HasDominantGene(this IEnumerable<GeneDef> genes)
		{
			return genes.Any((GeneDef x) => x == REA_DefOf.RE_AssertiveGenes);
		}

		public static bool HasDominantGene(this Pawn pawn, out List<Gene> endoGenes, out List<Gene> xenoGenes)
		{
			return (from x in (endoGenes = pawn.GetEndogenes()).Concat<Gene>(xenoGenes = pawn.GetXenogenes())
					select x.def).HasDominantGene();
		}

		public static bool CanInheritParentDominantGenes(Pawn parent, ref InheritGenes inherit)
		{
			if (parent == null)
			{
				return false;
			}
			List<Gene> parentEndogenes;
			List<Gene> xenoGenes;
			bool flag = parent.HasDominantGene(out parentEndogenes, out xenoGenes);
			if (flag && inherit == null)
			{
				inherit = delegate (List<GeneDef> genes)
				{
					foreach (Gene item in parentEndogenes)
					{
						genes.AddDistinct(item.def);
					}
				};
			}
			return flag;
		}

		public static bool CanInheritParentDominantXenotype(Pawn parent, ref InheritXenotype inherit)
		{
			if (parent == null)
			{
				return false;
			}
			XenotypeDef type = parent.genes?.Xenotype;
			List<Gene> endoGenes;
			List<Gene> xenoGenes;
			bool flag = parent.HasDominantGene(out endoGenes, out xenoGenes);
			if (flag)
			{
				if (inherit == null)
				{
					inherit = delegate (ref XenotypeDef xenotype)
					{
						xenotype = type;
					};
				}
				dominantParent = parent;
			}
			return flag;
		}
	}
}
