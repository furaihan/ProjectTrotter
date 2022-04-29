using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using BarbarianCall.API;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Mass Street Fighting", CalloutProbability.High)]
    class MassStreetFighting : CalloutBase
    {
        private List<Ped> Participant;
        private List<Ped> Gang1;
        private List<Ped> Gang2;
        private List<Ped> pursuitPeds;
        public bool CanEnd = false;
        private List<Model> Gang1Model;
        private List<Model> Gang2Model;
        private Dictionary<Ped, SuspectProperty> SuspectParameters;
        private Spawnpoint spawn2;
        int gangMemberCount;
        private int deadCount = 0;
        private int arrestedCount = 0;
        private int escapedCount = 0;
        private static readonly uint[] meleeWeapon = { 0x92A27487, 0x958A4A8F, 0xF9E6AA4B, 0x84BD7BFD, 0x4E875F73, 0xF9DCBF2D, 0xD8DF3C3C, 0x99B507EA, 0xDD5DF8D9, 0xDFE37640, 0x19044EE0, 0xCD274149, 0x3813FC08 };
        private RelationshipGroup gang1Relationship;
        private RelationshipGroup gang2Relationship;
        private Checkpoint checkpoint;
        private bool endSuccessfully = true;
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            deadCount = 0;
            arrestedCount = 0;
            escapedCount = 0;
            Participant = new List<Ped>();
            SuspectParameters = new Dictionary<Ped, SuspectProperty>();
            Gang1 = new List<Ped>();
            Gang2 = new List<Ped>();
            pursuitPeds = new List<Ped>();
            gang1Relationship = new RelationshipGroup("SIJI");
            gang2Relationship = new RelationshipGroup("LORO");
            DeclareVariable();
            Spawn = SpawnManager.GetSlowRoadSpawnPoint(PlayerPed.Position, 400, 750);
            if (Spawn == Spawnpoint.Zero)
            {
                "MassStreetFighting: No location found after 600 tries".ToLog();
                return false;
            }
            Position = Spawn;
            SpawnHeading = Spawn;
            spawn2 = SpawnManager.GetSlowRoadSpawnPoint(Spawn.Position, 30, 50);
            if (spawn2 == Spawnpoint.Zero) spawn2 = SpawnManager.GetSlowRoadSpawnPoint(Spawn.Position, 50, 80);
            if (spawn2 == Spawnpoint.Zero) spawn2 = SpawnManager.GetVehicleSpawnPoint(Spawn.Position, 30, 50);
            if (spawn2 == Spawnpoint.Zero) spawn2 = SpawnManager.GetVehicleSpawnPoint(Spawn.Position, 50, 80);
            if (spawn2 == Spawnpoint.Zero)
            {
                spawn2.Position = World.GetNextPositionOnStreet(CalloutPosition.Around2D(Peralatan.Random.Next(30, 50)));
                spawn2.Heading = SpawnManager.GetRoadHeading(spawn2.Position);
            }
            try
            {
                PlayerPed.RelationshipGroup.Name.ToLog();
            }
            catch (Exception e ) { e.ToString().ToLog(); }
            Gang1Model = Globals.GangPedModels.Values.ToList().GetRandomElement();
            Gang2Model = Globals.GangPedModels.Values.ToList().GetRandomElement(m=> m != Gang1Model);
            CalloutPosition = Spawn;
            gangMemberCount = Peralatan.Random.Next(3, 8);
            CalloutAdvisory = string.Format("Total number of suspects is {0}", gangMemberCount * 2);
            CalloutMessage = "Mass Street Fighting";
            ShowCalloutAreaBlipBeforeAccepting(Spawn, 80f);
            PlayScannerWithCallsign("WE_HAVE BAR_CRIME_GANG_RELATED IN_OR_ON_POSITION", Spawn);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Gang1Model.ForEach(m => m.LoadAndWait());
            Gang2Model.ForEach(m => m.LoadAndWait());
            for (int i = 1; i <= gangMemberCount; i++)
            {
                Ped gangMember = new(Gang1Model.GetRandomElement(), Position.Around2D(3f).ToGround(), Peralatan.Random.Next() % 2 == 0 ? SpawnHeading : Position.GetHeadingTowards(PlayerPed));
                gangMember.MakeMissionPed();
                Participant.Add(gangMember);
                CalloutEntities.Add(gangMember);
                Gang1.Add(gangMember);
                gangMember.Inventory.GiveNewWeapon(meleeWeapon.GetRandomElement(), -1, true);
                gangMember.RelationshipGroup = gang1Relationship;
                gangMember.Metadata.BAR_Entity = true;
                string mugshot = "WEB_LOSSANTOSPOLICEDEPT";
                SuspectParameters.Add(gangMember, new SuspectProperty(ESuspectStates.InAction, LSPDFR.GetPersonaForPed(gangMember).FullName, null, mugshot));
            }
            Vector3 sp2 = spawn2.Position;
            if (sp2 == Vector3.Zero) SpawnManager.GetVehicleSpawnPoint2(Spawn.Position, 30, 50);
            if (sp2 == Vector3.Zero) sp2 = World.GetNextPositionOnStreet(sp2.Around(30, 50));
            for (int i = 1; i <= gangMemberCount; i++)
            {
                Ped gangMember = new(Gang2Model.GetRandomElement(), sp2.Around2D(3f).ToGround(), Peralatan.Random.Next() % 2 == 0 ? SpawnHeading : sp2.GetHeadingTowards(PlayerPed));
                gangMember.MakeMissionPed();
                Participant.Add(gangMember);
                CalloutEntities.Add(gangMember);
                Gang2.Add(gangMember);
                gangMember.Inventory.GiveNewWeapon(meleeWeapon.GetRandomElement(), -1, true);
                gangMember.RelationshipGroup = gang2Relationship;
                gangMember.Metadata.BAR_Entity = true;
                string mugshot = "WEB_LOSSANTOSPOLICEDEPT";
                SuspectParameters.Add(gangMember, new SuspectProperty(ESuspectStates.InAction, LSPDFR.GetPersonaForPed(gangMember).FullName, null, mugshot));
            }
            Participant.ForEach(p => p.SetPedAsWanted());
            gang1Relationship.SetRelationshipWith(gang2Relationship, Relationship.Hate);
            gang1Relationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            gang1Relationship.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            gang2Relationship.SetRelationshipWith(gang1Relationship, Relationship.Hate);
            gang2Relationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            gang2Relationship.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            Blip = new(Spawn, 50f)
            {
                Color = Yellow
            };
            Blip.EnableRoute(Yellow);
            SituationTawuran();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            if (CalloutRunning)
            {
                if (!Participant.All(e => CalloutEntities.Contains(e)))
                {
                    Participant.ForEach(e =>
                    {
                        if (e)
                        {
                            if (!CalloutEntities.Contains(e))
                            {
                                CalloutEntities.Add(e);
                                "Ped is not assigned to callout entities".Print();
                            }
                        }
                    });
                }
                SuspectParameters.Keys.ToList().ForEach(p =>
                {
                    if (SuspectParameters.ContainsKey(p))
                    {
                        if (SuspectParameters[p].SuspectState == ESuspectStates.InAction && p && p.IsDead)
                        {
                            SuspectParameters[p].SuspectState = ESuspectStates.Dead;
                            string name = "~b~Unknown Name";
                            if (SuspectParameters.TryGetValue(p, out var jn)) name = jn.FullName;
                            $"~b~{name}~s~ is ~r~dead".DisplayNotifWithLogo(GetType().Name, SuspectParameters[p].MugshotTexture, SuspectParameters[p].MugshotTexture);
                            List<Blip> blips = CalloutBlips.Where(b => b && b.Entity == p).ToList();
                            blips.ForEach(b => { if (b) b.Delete(); });
                            deadCount++;
                        }
                        else if (SuspectParameters[p].SuspectState == ESuspectStates.InAction && p && LSPDFR.IsPedArrested(p))
                        {
                            SuspectParameters[p].SuspectState = ESuspectStates.Arrested;
                            string name = "~b~Unknown Name";
                            if (SuspectParameters.TryGetValue(p, out var jn)) name = jn.FullName;
                            $"~b~{name}~s~ is ~g~arrested".DisplayNotifWithLogo(GetType().Name, SuspectParameters[p].MugshotTexture, SuspectParameters[p].MugshotTexture);
                            List<Blip> blips = CalloutBlips.Where(b => b && b.Entity == p).ToList();
                            blips.ForEach(b => { if (b) b.Delete(); });
                            arrestedCount++;
                        }
                        else if (SuspectParameters[p].SuspectState == ESuspectStates.InAction && !p)
                        {
                            SuspectParameters[p].SuspectState = ESuspectStates.Escaped;
                            string name = "~b~Unknown Name";
                            if (SuspectParameters.TryGetValue(p, out var jn)) name = jn.FullName;
                            $"~b~{name}~s~ is ~o~escaped".DisplayNotifWithLogo(GetType().Name, SuspectParameters[p].MugshotTexture, SuspectParameters[p].MugshotTexture);
                            List<Blip> blips = CalloutBlips.Where(b => b && b.Entity == p).ToList();
                            blips.ForEach(b => { if (b) b.Delete(); });
                            escapedCount++;
                        }
                    }
                    else if (p) Peralatan.ToLog(string.Format("Dictionary Key doesn't exist: {0} - {1} - {2}", SuspectParameters.Count, (uint)p.Handle, p.Model.Name));
                    else
                    {
                        Peralatan.ToLog("SOMETHING WENT WRONG, SEND YOUR LOG PLS!!"); //should never happen
                    }
                });
                if (SuspectParameters.Values.All(pr=> pr.SuspectState != ESuspectStates.InAction)) CanEnd = true;
                if (CanEnd) DisplaySummary();
            }
            base.Process();
        }
        public override void End()
        {
            if (endSuccessfully)
            {
                Participant.ForEach(p =>
                {
                    if (p)
                    {
                        if (p.IsAlive && p.IsPed()) p.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
                        if (p) p.Dismiss();
                    }
                });
            }
            SuspectParameters.ToList().ForEach(sp =>Peralatan.UnregisterPedHeadshot(sp.Value.MugshotHandle));
            if (checkpoint) checkpoint.Delete();
            Extension.DeleteRelationshipGroup(gang1Relationship);
            Extension.DeleteRelationshipGroup(gang2Relationship);
            base.End();
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }
        private void DisplaySummary()
        {
            if (checkpoint) checkpoint.Delete();
            $"Suspect Count~s~: {gangMemberCount * 2}~n~~g~Arrested~s~: {arrestedCount}~n~~r~Dead~s~: {deadCount}~n~~o~Escaped~s~: {escapedCount}".
                DisplayNotifWithLogo("Mass Street Fighting", hudColor: RAGENativeUI.HudColor.GreenDark);
            End();
        }
        private void GetClose()
        {
            while (CalloutRunning)
            {
                if (Participant.Any(p => p && (p.CanSee(PlayerPed) || PlayerPed.CanSee(p))) || PlayerPed.DistanceToSquared(Position) < 10000f)
                {
#if DEBUG
                    string[] log =
                    {
                        $"Distance: {PlayerPed.DistanceTo(Position)}",
                        $"Player see suspect: {Participant.Any(p=> PlayerPed.CanSee(p))}",
                        $"Suspect see player: {Participant.Any(p=> p.CanSee(PlayerPed))}",
                        $"Suspect is on screen: {Participant.Any(predicate => predicate.IsOnScreen)}"
                    };
                    log.ToList().ForEach(Peralatan.ToLog);
#endif
                    break;
                }
                Ped ran = Participant.GetRandomElement();
                if (ran) ran.Tasks.AchieveHeading(ran.GetHeadingTowards(PlayerPed));
                GameFiber.Yield();
            }
            LSPDFRFunc.RequestBackup(CalloutPosition, LSPD_First_Response.EBackupResponseType.Code2);
            LSPDFRFunc.RequestBackup(CalloutPosition, LSPD_First_Response.EBackupResponseType.Code2);
            "Backup is on the way".DisplayNotifWithLogo();
            if (Blip) Blip.Delete();
            foreach (Ped part in Participant)
            {
                if (part)
                {
                    Blip gangblp = part.AttachBlip();
                    gangblp.Color = Color.Red;
                    gangblp.Scale = 0.754585f;
                    CalloutBlips.Add(gangblp);
                }
            }
        }
        private void SituationTawuran()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (CalloutRunning)
                    {
                        GetHeadshot();
                        Participant.ForEach(p => p.Tasks.FightAgainstClosestHatedTarget(1000f));
                        Pursuit = LSPDFR.CreatePursuit();
                        try
                        {
                            Spawnpoint slowR = SpawnManager.GetSlowRoadSpawnPoint(PlayerPed.Position, 50, 100);
                            if (slowR != Spawnpoint.Zero) checkpoint = new Checkpoint(CheckpointIcon.Cylinder, slowR, 5f, 80, Color.HotPink, Color.Gold);
                        }
                        catch { }                     
                        bool pursuitCalled = false;
                        bool heliPursuitBackup = false;
                        bool isAnyGangWipedOut = false;
                        StopWatch = new();
                        StopWatch.Start();
                        while (CalloutRunning)
                        {
                            if (CanEnd) break;
                            if (StopWatch.ElapsedMilliseconds > 3000 && Participant.Any(IsPedStuck) && !isAnyGangWipedOut)
                            {
                                StopWatch.Restart();
                                var st = Participant.Where(IsPedStuck).GetRandomElement();
                                if (!st) continue;
                                if (st) st.Tasks.FightAgainstClosestHatedTarget(1000);
                            }
                            Participant.ForEach(p =>
                            {
                                if (p)
                                {
                                    if (!pursuitPeds.Contains(p) && (p.DistanceToSquared(Position) > 40000f || p.IsFleeing))
                                    {
                                        if (p)
                                        {
                                            string log = string.Format("{0} - {1} is fleeing or too far away, setting up a pursuit for {2}. {3}, {4}", LSPDFR.GetPersonaForPed(p).FullName, p.Model.Name, 
                                                p.IsMale ? "him" : "her", p.IsFleeing, p.DistanceTo(Position));
                                            log.ToLog();
                                        }
                                        LSPDFR.AddPedToPursuit(Pursuit, p);
                                        pursuitPeds.Add(p);
                                        if (!pursuitCalled)
                                        {
                                            LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                                            pursuitCalled = true;
                                            PursuitCreated = true;
                                        }
                                    }
                                }
                            });
                            if ((Gang1.All(ped => ped && ped.IsDead) || Gang2.All(ped=> ped && ped.IsDead)) && !isAnyGangWipedOut)
                            {
                                double chance = Peralatan.Random.NextDouble();
                                List<Ped> suspectRest = Participant.Where(ped => ped && ped.IsAlive).ToList();
                                if (chance > 0.6525)
                                {
                                    suspectRest.ForEach(p =>
                                    {
                                        if (p)
                                        {
                                            LSPDFR.AddPedToPursuit(Pursuit, p);
                                            string weapon = p.Inventory.EquippedWeaponObject.Model.Name;
                                            LSPDFR.AddPedContraband(p, LSPD_First_Response.Engine.Scripting.Entities.ContrabandType.Weapon, weapon);
                                            if (StopThePedRunning) StopThePedFunc.InjectPedItem(p, "~r~" + weapon);
                                            p.Inventory.GiveNewWeapon("WEAPON_UNARMED", -1, true);
                                        }
                                    });
                                    GameFiber.Wait(2500);
                                    LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                                    GameFiber.Wait(1000);
                                    LSPDFRFunc.RequestBackup(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                                    GameFiber.Sleep(500);
                                    if (GrammarPoliceRunning) GrammarPoliceFunc.SetStatus(GrammarPoliceFunc.EGrammarPoliceStatusType.InPursuit);
                                }
                                else if (chance > 0.2585)
                                {
                                    gang1Relationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Neutral);
                                    gang2Relationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Neutral);
                                    RelationshipGroup.Cop.SetRelationshipWith(gang1Relationship, Relationship.Neutral);
                                    RelationshipGroup.Cop.SetRelationshipWith(gang2Relationship, Relationship.Neutral);
                                    suspectRest.ForEach(p =>
                                    {
                                        if (p)
                                        {
                                            var equippedWeapon = p.Inventory.EquippedWeapon;
                                            if (equippedWeapon != null) equippedWeapon.Drop();
                                            p.Tasks.PutHandsUp(-1, PlayerPed);
                                            GameFiber.Wait(100);
                                        }
                                    });
                                    GameFiber.Wait(250);
                                    ("Task Done: " + suspectRest.All(p => p.Tasks.CurrentTaskStatus != TaskStatus.None).ToString()).ToLog();
                                    Game.DisplaySubtitle("~r~Gang Member~s~: We've given up, you can arrest us officer");
                                }
                                else
                                {
                                    suspectRest.ForEach(p =>
                                    {
                                        if (p)
                                        {
                                            p.Inventory.GiveNewWeapon(WeaponHashes.GetRandomElement(true), -1, true);
                                            p.CombatAgainstHatedTargetAroundPed(350f);
                                        }
                                    });
                                }
                                isAnyGangWipedOut = true;
                            }
                            if (PursuitCreated && !heliPursuitBackup)
                            {
                                heliPursuitBackup = true;
                                GameFiber.StartNew(() =>
                                {
                                    GameFiber.Wait(Peralatan.Random.Next(7500, 15000));
                                    if (CalloutRunning && LSPDFR.IsPursuitStillRunning(Pursuit))
                                    {
                                        LSPDFRFunc.RequestAirUnit(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Pursuit);
                                    }
                                });
                            }
                            GameFiber.Wait(1);
                        }
                    }
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Mass Street Fighting");
                    endSuccessfully = false;
                    End($"exception: {e.Message}");
                }              
            });          
        }
        private void SituationTrapped()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    Spawnpoint roadSide1 = SpawnManager.GetRoadSideSpawnPoint(Position, SpawnHeading);
                    if (roadSide1 != Spawnpoint.Zero) Participant.ForEach(p => p.Tasks.FollowNavigationMeshToPosition(roadSide1.Position, roadSide1.Heading, 10f, 2f));
                    gang1Relationship.SetRelationshipWith(gang2Relationship, Relationship.Companion);
                    gang2Relationship.SetRelationshipWith(gang1Relationship, Relationship.Companion);
                    GetClose();
                    if (CalloutRunning)
                    {
                        GetHeadshot();
                        Participant.ForEach(p => p.CombatAgainstHatedTargetAroundPed(250f));
                    }
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Mass Street Fighting");
                    endSuccessfully = false;
                    End();
                }
            });
        }
        private bool IsPedStuck(Ped ped)
        {
            if (!ped) return false;
            if (ped.IsInMeleeCombat) return false;
            if (ped.Tasks.CurrentTaskStatus == TaskStatus.Preparing) return false;
            if (ped.Tasks.CurrentTaskStatus == TaskStatus.InProgress) return false;
            if (ped && (LSPDFR.IsPedBeingCuffed(ped) || LSPDFR.IsPedBeingFrisked(ped) || LSPDFR.IsPedBeingGrabbed(ped) || LSPDFR.IsPedInPursuit(ped) 
                || LSPDFR.IsPedGettingArrested(ped) || pursuitPeds.Contains(ped) || LSPDFR.IsPedArrested(ped) || ped.IsDead || LSPDFR.IsPedStoppedByPlayer(ped)
                || LSPDFR.IsPedBeingCuffedByPlayer(ped) || LSPDFR.IsPedBeingFriskedByPlayer(ped) || LSPDFR.IsPedBeingGrabbedByPlayer(ped))) return false;
            if (Game.LocalPlayer.IsFreeAimingAtAnyEntity && Game.LocalPlayer.GetFreeAimingTarget() == ped) return false;
            if (StopThePedRunning && StopThePedFunc.IsPedStoppedWithSTP(ped) == true) return false;
            if (ped.IsTaskActive(PedTask.DoNothing) || !ped.IsTaskActive(PedTask.CombatClosestTargetInArea)) return true;
            return true;
        }
        private void GetHeadshot()
        {
            GameFiber.StartNew(() =>
            {
                Participant.ForEach(ped =>
                {
                    if (ped)
                    {
                        string mugshot = ped.GetPedHeadshotTexture(out uint? handle);
                        SuspectParameters[ped].MugshotTexture = mugshot;
                        SuspectParameters[ped].MugshotHandle = handle;
                    }                    
                });
            });
        }

    }
}
