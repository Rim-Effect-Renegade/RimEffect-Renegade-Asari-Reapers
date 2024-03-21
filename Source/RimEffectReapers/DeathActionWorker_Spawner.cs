﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace RimEffectReapers
{
    public class DeathActionWorker_Spawner : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            DefModExt_DeathSpawnerProps props = corpse.InnerPawn?.def?.GetModExtension<DefModExt_DeathSpawnerProps>() ?? null;

            if(props == null)
            {
                Log.Warning("DeathActionWorker_Spawner used without required modExtension, meaning it will do nothing.");
                return;
            }

            int spawnCount = props.quantityRange.RandomInRange;
            Faction faction = FactionUtility.DefaultFactionFrom(props.kindDef.defaultFactionType) ?? Faction.OfMechanoids;

            PawnGenerationRequest request = new PawnGenerationRequest(
                kind: props.kindDef,
                faction: faction,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: false
            );

            for (int i = 0; i < spawnCount; i++)
            {
                Pawn newThing = PawnGenerator.GeneratePawn(request);
                GenSpawn.Spawn(newThing, corpse.Position, corpse.Map, WipeMode.Vanish);
            }
        }
    }
}
