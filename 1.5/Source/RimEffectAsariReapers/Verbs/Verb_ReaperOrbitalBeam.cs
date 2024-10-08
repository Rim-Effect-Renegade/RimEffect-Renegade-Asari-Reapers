﻿using RimWorld;
using Verse;
using Verse.AI;

namespace RimEffectAR
{
    public class Verb_ReaperOrbitalBeam : Verb_CastBase
    {
        public const int DurationTicks = 600;

        public override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map) return false;

            var powerBeam = (PowerBeam) GenSpawn.Spawn(RimEffectARDefOf.RE_ReaperPowerBeam, currentTarget.Cell,
                caster.Map);
            powerBeam.duration = DurationTicks;
            powerBeam.instigator = caster;
            powerBeam.weaponDef = EquipmentSource != null ? EquipmentSource.def : null;
            powerBeam.StartStrike();
            return true;
        }

        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = false;
            return 15f;
        }
    }
}