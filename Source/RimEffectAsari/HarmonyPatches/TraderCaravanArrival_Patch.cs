﻿namespace RimEffectAsari
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using HarmonyLib;
    using RimWorld;
    using Verse;
    using Verse.AI;
    using Verse.AI.Group;

    [HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival), "TryExecuteWorker")]
    public static class TraderCaravanArrival_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var list   = instructions.ToList();
            var info1  = AccessTools.Method(typeof(IncidentWorker_TraderCaravanArrival), "SendLetter");
            var idx1   = list.FindIndex(ins => ins.Calls(info1)) + 1;
            var label1 = generator.DefineLabel();
            var label2 = generator.DefineLabel();
            list[idx1].labels.Add(label2);
            list.InsertRange(idx1, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.faction))),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Faction), nameof(Faction.def))),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(REA_DefOf), nameof(REA_DefOf.RE_AsariRepublics))),
                new CodeInstruction(OpCodes.Bne_Un, label2),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(list[idx1 - 3]),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TraderCaravanArrival_Patch), nameof(TraderArrivalAsari))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ret)
            });
            return list;
        }

        public static void TraderArrivalAsari(IncidentParms parms, List<Pawn> pawns, TraderKindDef traderKind)
        {
            var shuttle = (AsariShuttleLanded) ThingMaker.MakeThing(REA_DefOf.RE_AsariShuttleLanded);
            shuttle.SetFactionDirect(parms.faction);
            foreach (var pawn in pawns) pawn.DeSpawn();

            shuttle.GetDirectlyHeldThings().TryAddRangeOrTransfer(pawns);
            shuttle.State    = AsariShuttleLanded.ShuttleState.Unloading;
            shuttle.Rotation = Rot4.East;

            var map  = parms.target as Map;
            var cell = DropCellFinder.TryFindSafeLandingSpotCloseToColony(map, shuttle.RotatedSize, parms.faction);
            if (!cell.IsValid && !DropCellFinder.FindSafeLandingSpot(out cell, parms.faction, map, size: shuttle.RotatedSize + new IntVec2(2, 2)))
            {
                cell = DropCellFinder.RandomDropSpot(map);
                if (!cell.IsValid) cell                                                                                            = DropCellFinder.RandomDropSpot(map, false);
                if (DropCellFinder.TryFindDropSpotNear(cell, map, out var newCell, false, false, false, shuttle.RotatedSize)) cell = newCell;
            }

            SkyfallerMaker.SpawnSkyfaller(REA_DefOf.RE_AsariShuttleIncoming, shuttle, cell, map).Rotation = Rot4.North;
        }
    }

    public class AsariShuttleLanded : Building, IThingHolder
    {
        public enum ShuttleState
        {
            Unloading,
            Loading,
            Waiting
        }

        private ThingOwner   innerContainer;
        private Lord         lord;
        public  ShuttleState State;
        public  List<Thing>  ToLoad;

        public AsariShuttleLanded() => innerContainer = new ThingOwner<Thing>(this);

        public ThingOwner GetDirectlyHeldThings() => innerContainer;

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public override void Tick()
        {
            base.Tick();
            if (!Spawned) return;
            switch (State)
            {
                case ShuttleState.Unloading when this.IsHashIntervalTick(Rand.Range(15, 25)):
                    if (lord == null)
                        lord = LordMaker.MakeNewLord(Faction,
                            new LordJob_TradeWithColonyFromShuttle(Faction, Position + new IntVec3(Rot4.Random.AsVector2) * Rand.Range(4, 7), this), Map);
                    if (innerContainer.OfType<Pawn>().TryRandomElement(out var pawn))
                    {
                        innerContainer.TryDrop(pawn, Position + IntVec3.South * 2, Map, ThingPlaceMode.Near, 1, out _);
                        if (pawn.GetLord() != null) pawn.GetLord().Notify_PawnLost(pawn, PawnLostCondition.ForcedToJoinOtherLord);
                        lord.AddPawn(pawn);
                    }
                    else
                        innerContainer.TryDropAll(Position + IntVec3.South * 2, Map, ThingPlaceMode.Near);

                    if (!innerContainer.Any()) State = ShuttleState.Waiting;
                    return;
                case ShuttleState.Loading when ToLoad != null && !ToLoad.Any():
                    var map  = Map;
                    var cell = Position;
                    DeSpawn();

                    foreach (var thing in innerContainer)
                        if (thing is Pawn p && p.GetLord() is Lord lord)
                        {
                            lord.RemovePawn(p);
                            if (!lord.ownedPawns.Any()) lord.Cleanup();
                        }

                    SkyfallerMaker.SpawnSkyfaller(REA_DefOf.RE_AsariShuttleLeaving, this, cell, map).Rotation = Rot4.North;
                    return;
                case ShuttleState.Waiting:
                    return;
                default: return;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Values.Look(ref State, "state");
            Scribe_References.Look(ref lord, "lord");
            Scribe_Collections.Look(ref ToLoad, "toLoad", LookMode.Reference);
        }
    }

    public class AsariShuttleFlying : Skyfaller
    {
        public override void LeaveMap()
        {
            foreach (var thing in innerContainer)
                if (thing is Pawn p)
                    p.DestroyOrPassToWorld();

            base.LeaveMap();
        }
    }

    public class JobGiver_EnterAsariShuttle : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var shuttle = pawn.Map.listerThings.ThingsOfDef(REA_DefOf.RE_AsariShuttleLanded).OfType<AsariShuttleLanded>().FirstOrDefault(t =>
                t.Faction == pawn.Faction && t.State == AsariShuttleLanded.ShuttleState.Loading && t.ToLoad != null && t.ToLoad.Contains(pawn));
            return shuttle != null ? JobMaker.MakeJob(REA_DefOf.RE_EnterShuttle, shuttle) : null;
        }
    }

    public class JobDriver_EnterShuttle : JobDriver
    {
        public AsariShuttleLanded Shuttle => job?.GetTarget(TargetIndex.A).Thing as AsariShuttleLanded;
        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;

        public override IEnumerable<Toil> MakeNewToils()
        {
            Log.Message($"Shuttle={Shuttle},ToLoad={Shuttle?.ToLoad},State={Shuttle?.State}");
            this.FailOn(() => Shuttle?.ToLoad is null || !Shuttle.ToLoad.Contains(pawn) || Shuttle.State != AsariShuttleLanded.ShuttleState.Loading);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Do(() =>
            {
                Shuttle?.ToLoad?.Remove(pawn);
                pawn.DeSpawn();
                Shuttle?.GetDirectlyHeldThings().TryAddOrTransfer(pawn, false);
            });
        }
    }

    public class LordJob_TradeWithColonyFromShuttle : LordJob
    {
        private IntVec3 chillSpot;

        private Faction faction;

        public AsariShuttleLanded Shuttle;

        public LordJob_TradeWithColonyFromShuttle()
        {
        }

        public LordJob_TradeWithColonyFromShuttle(Faction faction, IntVec3 chillSpot, AsariShuttleLanded shuttle)
        {
            this.faction   = faction;
            this.chillSpot = chillSpot;
            Shuttle        = shuttle;
        }

        public override StateGraph CreateGraph()
        {
            var graph      = new StateGraph();
            var travelToil = new LordToil_Travel(chillSpot);
            graph.StartingToil = travelToil;
            var chill = new LordToil_DefendTrader(chillSpot);
            graph.AddToil(chill);
            var defendTrader = new LordToil_DefendTrader();
            graph.AddToil(defendTrader);
            var leaveNormal = new LordToil_LeaveOnShuttle();
            graph.AddToil(leaveNormal);
            var leaveDefend = new LordToil_LeaveOnShuttleAndDefendSelf();
            graph.AddToil(leaveDefend);
            var leaveMining = new LordToil_LeaveOnShuttle(LocomotionUrgency.Walk, true);
            graph.AddToil(leaveMining);
            var dangerousTemp = new Transition(travelToil, leaveNormal);
            dangerousTemp.AddSource(chill);
            dangerousTemp.AddPreAction(new TransitionAction_Message("MessageVisitorsDangerousTemperature".Translate(faction.def.pawnsPlural.CapitalizeFirst(), faction.Name)));
            dangerousTemp.AddPostAction(new TransitionAction_EndAllJobs());
            dangerousTemp.AddTrigger(new Trigger_PawnExperiencingDangerousTemperatures());
            graph.AddTransition(dangerousTemp);
            var trapped = new Transition(travelToil, leaveMining);
            trapped.AddSources(chill, leaveNormal, leaveDefend);
            trapped.AddPostAction(new TransitionAction_Message("MessageVisitorsTrappedLeaving".Translate(faction.def.pawnsPlural.CapitalizeFirst(), faction.Name)));
            trapped.AddPostAction(new TransitionAction_WakeAll());
            trapped.AddPostAction(new TransitionAction_EndAllJobs());
            trapped.AddTrigger(new Trigger_CannotReachShuttle());
            graph.AddTransition(trapped);
            var notTrapped = new Transition(leaveMining, leaveDefend);
            notTrapped.AddTrigger(new Trigger_AllCanReachShuttle());
            notTrapped.AddPostAction(new TransitionAction_EndAllJobs());
            graph.AddTransition(notTrapped);
            var defendOnHarmed = new Transition(travelToil, defendTrader);
            defendOnHarmed.AddTrigger(new Trigger_PawnHarmed());
            defendOnHarmed.AddPreAction(new TransitionAction_SetDefendTrader());
            defendOnHarmed.AddPostAction(new TransitionAction_WakeAll());
            defendOnHarmed.AddPostAction(new TransitionAction_EndAllJobs());
            graph.AddTransition(defendOnHarmed);
            var notHarmed = new Transition(defendTrader, leaveDefend);
            notHarmed.AddTrigger(new Trigger_TicksPassedWithoutHarm(1200));
            graph.AddTransition(notHarmed);
            var arrive = new Transition(travelToil, chill);
            arrive.AddTrigger(new Trigger_Memo("TravelArrived"));
            graph.AddTransition(arrive);
            var leave = new Transition(chill, leaveNormal);
            leave.AddTrigger(new Trigger_TicksPassed(DebugSettings.instantVisitorsGift ? 2500 : Rand.Range(27000, 45000)));
            leave.AddPreAction(new TransitionAction_CheckGiveGift());
            leave.AddPreAction(new TransitionAction_Message("MessageTraderCaravanLeaving".Translate(faction.Name)));
            leave.AddPostAction(new TransitionAction_WakeAll());
            graph.AddTransition(leave);
            var backup = new Transition(leaveDefend, leaveNormal);
            backup.AddSources(leaveMining, chill, defendTrader, travelToil);
            backup.AddTrigger(new Trigger_TicksPassed(60000));
            backup.AddPostAction(new TransitionAction_WakeAll());
            graph.AddTransition(backup);
            var retreat = new Transition(defendTrader, leaveDefend);
            retreat.AddSources(chill, travelToil);
            retreat.AddTrigger(new Trigger_ImportantTraderCaravanPeopleLost());
            retreat.AddTrigger(new Trigger_BecamePlayerEnemy());
            retreat.AddPostAction(new TransitionAction_WakeAll());
            retreat.AddPostAction(new TransitionAction_EndAllJobs());
            graph.AddTransition(retreat);
            var defendShuttle   = new LordToil_DefendPoint(Shuttle.Position, 7f);
            var onShuttleDamage = new Transition(travelToil, defendShuttle);
            onShuttleDamage.AddSources(chill, defendTrader, leaveNormal, leaveMining, leaveDefend);
            onShuttleDamage.AddPostAction(new TransitionAction_WakeAll());
            onShuttleDamage.AddPostAction(new TransitionAction_EndAllJobs());
            onShuttleDamage.AddTrigger(new Trigger_ThingDamageTaken(Shuttle, 0.8f));
            graph.AddToil(defendShuttle);
            graph.AddTransition(onShuttleDamage);
            var shuttleSafe = new Transition(defendShuttle, leaveDefend);
            shuttleSafe.AddTrigger(new Trigger_TicksPassedWithoutHarm(1200));
            shuttleSafe.AddPostAction(new TransitionAction_WakeAll());
            shuttleSafe.AddPostAction(new TransitionAction_EndAllJobs());
            graph.AddTransition(shuttleSafe);
            return graph;
        }

        public override bool CanOpenAnyDoor(Pawn p) => p.RaceProps.FenceBlocked;

        public override void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref chillSpot, "chillSpot");
            Scribe_References.Look(ref Shuttle, "shuttle");
        }
    }

    public class LordToil_DefendTrader : LordToil_DefendPoint
    {
        public LordToil_DefendTrader()
        {
        }

        public LordToil_DefendTrader(IntVec3 defendPoint) : base(defendPoint)
        {
        }

        public override bool AllowSatisfyLongNeeds => false;

        public override float? CustomWakeThreshold => 0.5f;

        public override void UpdateAllDuties()
        {
            var pawn2 = lord.ownedPawns.Where(p => p.RaceProps.Humanlike).RandomElement();
            foreach (var pawn in lord.ownedPawns)
            {
                Log.Message($"TraderCaravanRole of {pawn} is {pawn.GetTraderCaravanRole()}");
                switch (pawn.GetTraderCaravanRole())
                {
                    case TraderCaravanRole.Carrier:
                        pawn.mindState.duty            = new PawnDuty(DutyDefOf.Follow, pawn2, 5f);
                        pawn.mindState.duty.locomotion = LocomotionUrgency.Walk;
                        break;
                    case TraderCaravanRole.Chattel:
                        pawn.mindState.duty            = new PawnDuty(DutyDefOf.Escort, pawn2, 5f);
                        pawn.mindState.duty.locomotion = LocomotionUrgency.Walk;
                        break;
                    case TraderCaravanRole.Trader:
                    case TraderCaravanRole.Guard:
                    case TraderCaravanRole.None:
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.Defend, Data.defendPoint, Data.defendRadius);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class Trigger_CannotReachShuttle : Trigger
    {
        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 127 == 1)
            {
                var shuttle = ((LordJob_TradeWithColonyFromShuttle) lord.LordJob).Shuttle;
                if (lord.ownedPawns.Any(pawn => pawn.Spawned && !pawn.Downed && !pawn.Dead && !pawn.CanReach(shuttle, PathEndMode.Touch, Danger.Deadly))) return true;
            }

            return false;
        }
    }

    public class Trigger_AllCanReachShuttle : Trigger
    {
        public override bool ActivateOn(Lord lord, TriggerSignal signal)
        {
            if (signal.type == TriggerSignalType.Tick && Find.TickManager.TicksGame % 127 == 1)
            {
                var shuttle = ((LordJob_TradeWithColonyFromShuttle) lord.LordJob).Shuttle;
                if (lord.ownedPawns.All(pawn => !pawn.Spawned || pawn.Downed || pawn.Dead && pawn.CanReach(shuttle, PathEndMode.Touch, Danger.Deadly))) return true;
            }

            return false;
        }
    }

    public class LordToil_LeaveOnShuttle : LordToil_ExitMap
    {
        public LordToil_LeaveOnShuttle(LocomotionUrgency locomotion = LocomotionUrgency.None, bool canDig = false, bool interruptCurrentJob = false) : base(locomotion, canDig,
            interruptCurrentJob)
        {
        }

        public override DutyDef ExitDuty => REA_DefOf.RE_LeaveOnShuttle;

        public override void Init()
        {
            base.Init();
            var shuttle = ((LordJob_TradeWithColonyFromShuttle) lord.LordJob).Shuttle;
            shuttle.ToLoad = lord.ownedPawns.ListFullCopy().Cast<Thing>().ToList();
            shuttle.State  = AsariShuttleLanded.ShuttleState.Loading;
        }
    }

    public class LordToil_LeaveOnShuttleAndDefendSelf : LordToil_LeaveOnShuttle
    {
        public override DutyDef ExitDuty => REA_DefOf.RE_LeaveOnShuttleAndDefendSelf;
    }
}