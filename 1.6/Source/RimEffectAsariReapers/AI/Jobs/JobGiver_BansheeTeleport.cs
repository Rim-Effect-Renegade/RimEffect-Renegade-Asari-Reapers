using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectAR
{
    using RimWorld;
    using Verse;
    using Verse.AI;

    public class JobGiver_BansheeTeleport : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            Thing target = (Thing)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable, maxDist: 20f);

            if (target == null)
                return null;

            return new Job(RimEffectARDefOf.RE_BansheeTeleport, target);
        }
    }
}
