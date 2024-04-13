﻿using System.Linq;
using RimWorld;
using Verse;

namespace RimEffectAR
{
    using System.Text;

    public class Building_ReaperTurret : Building_TurretGun
    {
        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            foreach (var building in Map.listerThings.ThingsOfDef(RimEffectARDefOf.RE_ReaperUnitStorage)
                .Cast<Building_Reaper_UnitStorage>().Where(b => b.CanRelease))
                building.Release();
            base.PreApplyDamage(ref dinfo, out absorbed);
        }

        public override bool ClaimableBy(Faction by, StringBuilder reason = null)
        {
            return false;
        }
    }
}