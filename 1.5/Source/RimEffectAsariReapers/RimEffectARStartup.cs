using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace RimEffectAR
{
    [StaticConstructorOnStartup]
    public static class RimEffectARStartup
    {
        static RimEffectARStartup()
        {
            ReaperRotTweak();
            Setup();
            DoDefsAlter();
        }

        public static void Setup()
        {
            RimEffectARMod.settings.incidents = DefDatabase<IncidentDef>.AllDefsListForReading.Where(x => x.defName.StartsWith("RE_ReaperLand")).ToList();

            if (RimEffectARMod.settings.reaperShipIncidentChances is null)
            {
                RimEffectARMod.settings.reaperShipIncidentChances = new Dictionary<string, float>();
            }
            RimEffectARMod.settings.reaperShipIncidentChances.RemoveAll(x => DefDatabase<IncidentDef>.GetNamed(x.Key, false) is null);

            if (RimEffectARMod.settings.reaperShipPresences is null)
            {
                RimEffectARMod.settings.reaperShipPresences = new Dictionary<string, int>();
            }

            if (RimEffectARMod.settings.reaperShipColonistCount is null)
            {
                RimEffectARMod.settings.reaperShipColonistCount = new Dictionary<string, int>();
            }

            if (RimEffectARMod.settings.reaperShipWealth is null)
            {
                RimEffectARMod.settings.reaperShipWealth = new Dictionary<string, int>();
            }

            if (RimEffectARMod.settings.reaperShipDistances is null)
            {
                RimEffectARMod.settings.reaperShipDistances = new Dictionary<string, int>();
            }

            foreach (var reaperShip in RimEffectARMod.settings.incidents)
            {
                if (!RimEffectARMod.settings.reaperShipIncidentChances.ContainsKey(reaperShip.defName))
                {
                    RimEffectARMod.settings.reaperShipIncidentChances[reaperShip.defName] = reaperShip.baseChance;
                }
                reaperShip.baseChance = 0;

                ReaperIncidentExtension incidentExtension = reaperShip.GetModExtension<ReaperIncidentExtension>();
                if (!RimEffectARMod.settings.reaperShipPresences.ContainsKey(incidentExtension.baseToPlace.defName))
                {
                    int presence = incidentExtension.baseToPlace.GetModExtension<ReaperBaseExtension>().raisesPresence;
                    RimEffectARMod.settings.reaperShipPresences[incidentExtension.baseToPlace.defName] = presence;
                }

                if (!RimEffectARMod.settings.reaperShipColonistCount.ContainsKey(reaperShip.defName))
                {
                    RimEffectARMod.settings.reaperShipColonistCount[reaperShip.defName] = incidentExtension.minimumColonistCount;
                }

                if (!RimEffectARMod.settings.reaperShipWealth.ContainsKey(reaperShip.defName))
                {
                    RimEffectARMod.settings.reaperShipWealth[reaperShip.defName] = incidentExtension.minimumWealth;
                }

                if (!RimEffectARMod.settings.reaperShipDistances.ContainsKey(reaperShip.defName))
                {
                    RimEffectARMod.settings.reaperShipDistances[reaperShip.defName] = incidentExtension.maxDistance;
                }
            }
        }

        public static void DoDefsAlter()
        {
            foreach (IncidentDef incidentDef in RimEffectARMod.settings.incidents)
            {
                ReaperIncidentExtension incidentExtension = incidentDef.GetModExtension<ReaperIncidentExtension>();

                incidentExtension.minimumColonistCount = RimEffectARMod.settings.reaperShipColonistCount[incidentDef.defName];
                incidentExtension.maxDistance = RimEffectARMod.settings.reaperShipDistances[incidentDef.defName];


            }

            foreach (KeyValuePair<string, int> reaperShipPresence in RimEffectARMod.settings.reaperShipPresences)
            {
                var defToAlter = DefDatabase<WorldObjectDef>.GetNamedSilentFail(reaperShipPresence.Key);
                if (defToAlter != null)
                {
                    defToAlter.GetModExtension<ReaperBaseExtension>().raisesPresence = reaperShipPresence.Value;
                }
            }
        }

        public static void ReaperRotTweak()
        {
            foreach(ThingDef thingdef in DefDatabase<ThingDef>.AllDefs)
            {
                ThingDef corpseDef = thingdef?.race?.corpseDef;
                if(corpseDef != null)
                {
                    if (thingdef?.race?.FleshType == RimEffectARDefOf.RE_Husk)
                    {
                        corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
                        corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
                    }
}
            }
        }
    }
}
