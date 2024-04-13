using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectAR
{
    using Verse;
    using Verse.AI;

    public class ThinkNode_ConditionalIsBanshee : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn) => 
            pawn.def == RimEffectARDefOf.RE_Husk_Banshee;
    }
}
