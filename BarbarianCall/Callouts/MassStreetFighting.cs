using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using RAGENativeUI.Elements;
using BarbarianCall.Extensions;
using BarbarianCall.Types;
using BarbarianCall.API;

namespace BarbarianCall.Callouts
{
    [CalloutInfo("Mass Street Fighting", CalloutProbability.High)]
    class MassStreetFighting : CalloutBase
    {
        private List<Ped> Participant = new();
        private List<Ped> Gang1 = new();
        private List<Ped> Gang2 = new();
        private List<Ped> pursuitPeds = new();
        private List<Ped> pedTakenCare = new();
        public bool CanEnd = false;
        private List<Model> Gang1Model = new();
        private List<Model> Gang2Model = new();
        private Spawnpoint spawn2;
        int gangMemberCount;
        private int deadCount = 0;
        private int arrestedCount = 0;
        private int escapedCount = 0;
        private int stuckCount = 0;
        private static readonly uint[] meleeWeapon = { 0x92A27487, 0x958A4A8F, 0xF9E6AA4B, 0x84BD7BFD, 0x4E875F73, 0xF9DCBF2D, 0xD8DF3C3C, 0x99B507EA, 0xDD5DF8D9, 0xDFE37640, 0x19044EE0, 0xCD274149, 0x3813FC08 };
        private RelationshipGroup gang1Relationship;
        private RelationshipGroup gang2Relationship;
        private bool endSuccessfully = true;
        private int status = 0;
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutRunning = false;
            PursuitCreated = false;
            deadCount = 0;
            arrestedCount = 0;
            escapedCount = 0;
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
            spawn2 = SpawnManager.GetSlowRoadSpawnPoint(Spawn.Position, 100, 200);
            if (spawn2 == Spawnpoint.Zero) spawn2 = SpawnManager.GetSlowRoadSpawnPoint(Spawn.Position, 80, 300);
            if (spawn2 == Spawnpoint.Zero) spawn2 = SpawnManager.GetVehicleSpawnPoint(Spawn.Position, 30, 50);
            if (spawn2 == Spawnpoint.Zero) spawn2 = SpawnManager.GetVehicleSpawnPoint(Spawn.Position, 50, 80);
            if (spawn2 == Spawnpoint.Zero)
            {
                spawn2.Position = World.GetNextPositionOnStreet(CalloutPosition.Around2D(Peralatan.Random.Next(30, 50)));
                spawn2.Heading = SpawnManager.GetRoadHeading(spawn2.Position);
            }           
            var tmp = Globals.GangPedModels.Values.GetRandomNumberOfElements(3);
            Gang1Model = new List<Model>(tmp.First().Where(x => x.IsInCdImage));
            Gang2Model = new List<Model>(tmp.Last().Where(x => x.IsInCdImage));
            Gang1Model.ForEach(m => m.LoadAndWait());
            Gang2Model.ForEach(m => m.LoadAndWait());
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
            for (int i = 1; i <= gangMemberCount; i++)
            {
                Ped gangMember = new(Gang1Model.GetRandomElement(), Position.Around2D(5f).ToGround(), SpawnHeading);
                gangMember.MakeMissionPed();
                Participant.Add(gangMember);
                CalloutEntities.Add(gangMember);
                Gang1.Add(gangMember);
                gangMember.Inventory.GiveNewWeapon(meleeWeapon.GetRandomElement(), -1, true);
                gangMember.RelationshipGroup = gang1Relationship;
                gangMember.Metadata.BAR_Entity = true;
            }
            for (int i = 1; i <= gangMemberCount; i++)
            {
                Ped gangMember = new(Gang2Model.GetRandomElement(), Position.Around2D(5f).ToGround(), SpawnHeading);
                gangMember.MakeMissionPed();
                Participant.Add(gangMember);
                CalloutEntities.Add(gangMember);
                Gang2.Add(gangMember);
                gangMember.Inventory.GiveNewWeapon(meleeWeapon.GetRandomElement(), -1, true);
                gangMember.RelationshipGroup = gang2Relationship;
                gangMember.Metadata.BAR_Entity = true;
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
            TawuranRewrite();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void OnCalloutNotAccepted()
        {
            Gang1Model.ForEach(m => m.Dismiss());
            Gang2Model.ForEach(m => m.Dismiss());
            Gang1.Clear();
            Gang2.Clear();
            Gang1Model.Clear();
            Gang2Model.Clear();
            Participant.Clear();
            pursuitPeds.Clear();
            pedTakenCare.Clear();
            base.OnCalloutNotAccepted();
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
            Extension.DeleteRelationshipGroup(gang1Relationship);
            Extension.DeleteRelationshipGroup(gang2Relationship);
            Gang1.Clear();
            Gang2.Clear();
            Gang1Model.ForEach(m => m.Dismiss());
            Gang2Model.ForEach(m => m.Dismiss());
            Gang1Model.Clear();
            Gang2Model.Clear();
            Participant.Clear();
            pursuitPeds.Clear();
            pedTakenCare.Clear();
            base.End();
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }
        private void DisplaySummary()
        {
            $"Suspect Count~s~: {gangMemberCount * 2}~n~~g~Arrested~s~: {arrestedCount}~n~~r~Dead~s~: {deadCount}~n~~o~Escaped~s~: {escapedCount}".
                DisplayNotifWithLogo("Mass Street Fighting", hudColor: RAGENativeUI.HudColor.GreenDark);
            End();
        }
        private void TawuranRewrite()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    TimerBarPool pool = new();
                    TextTimerBar deadBar = new("Dead: ", "");
                    TextTimerBar arrestedBar = new("Arrested: ", "");
                    TextTimerBar escapedBar = new("Escaped: ", "");
                    TextTimerBar stuckBar = new("Stuck:", "");
                    BarTimerBar complete = new("Code 4")
                    {
                        ForegroundColor = Green,
                        BackgroundColor = Color.FromArgb(120, Green),
                        Accent = Color.Green,
                        Highlight = Red,
                    };
                    deadBar.LabelStyle = TimerBarBase.DefaultLabelStyle.With(color: Red);
                    arrestedBar.LabelStyle = TimerBarBase.DefaultLabelStyle.With(color: Green);
                    escapedBar.LabelStyle = TimerBarBase.DefaultLabelStyle.With(color: Orange);
                    pool.AddRange(new TimerBarBase[] {complete, arrestedBar, escapedBar, deadBar, stuckBar});
                    while (CalloutRunning)
                    {
                        switch (status)
                        {
                            case 0:
                                $"Started callout logic".ToLog();
                                SendCIMessage("2 Gang members are on a melee war");
                                SendCIMessage($"Number of ivolved: {Participant.Count}");
                                status = 1;
                                break;
                            case 1:                                
                                if (PlayerPed.DistanceToSquared(Position) < 10000f)
                                {                                
                                    status = 2;
                                    if (Blip) Blip.Delete();
                                }
                                break;
                            case 2:
                                LSPDFR.RequestBackup(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Code2, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                LSPDFR.RequestBackup(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Code2, LSPD_First_Response.EBackupUnitType.LocalUnit);
                                foreach (Ped ped in Participant)
                                {
                                    if (ped)
                                    {
                                        ped.GetCombatProperty().Aggressive = true;
                                        ped.GetCombatProperty().AlwaysFight = true;
                                        ped.GetCombatProperty().CanChaseTargetOnFoot = true;
                                        ped.GetCombatProperty().DisableFleeFromCombat = true;
                                        ped.GetCombatProperty().CanUseVehicles = false;
                                        ped.GetCombatProperty().CanCommandeerVehicles = false;
                                        ped.GetCombatProperty().DisableBulletReactions = true;
                                        ped.GetCombatProperty().DisableReactToBuddyShot = true;
                                        ped.GetCombatProperty().MinimumDistanceToTarget = 10f;
                                        ped.GetCombatProperty().MaintainMinDistanceToTarget = true;
                                        ped.Tasks.FightAgainstClosestHatedTarget(650f);
                                        Blip blip = new Blip(ped)
                                        {
                                            Color = Red,
                                            Scale = 0.7445659999f,
                                            Alpha = 0.654487f,
                                        };
                                        CalloutBlips.Add(blip);
                                        ped.Metadata.BAR_AttachedBlip = blip;
                                    }
                                }
                                status = 3;
                                break;
                            case 3:
                                Monitor();
                                TaskMonitor();
                                if (pedTakenCare.Count == Participant.Count) status = 99;
                                if (Gang1.All(x => pedTakenCare.Contains(x)) || Gang2.All(x => pedTakenCare.Contains(x)))
                                {
                                    status = 4;
                                }
                                break;
                            case 4:
                                Pursuit = LSPDFR.CreatePursuit();
                                foreach (Ped ped in Participant.Where(x => x && x.IsAlive && !LSPDFR.IsPedArrested(x) && !LSPDFR.IsPedGettingArrested(x)))
                                {
                                    ped.Tasks.Clear();
                                    LSPDFR.AddPedToPursuit(Pursuit, ped);
                                    LSPDFR.GetPedPursuitAttributes(ped).ExhaustionDuration = 15000;
                                    LSPDFR.GetPedPursuitAttributes(ped).ExhaustionInterval = 40000;
                                    LSPDFR.GetPedPursuitAttributes(ped).CanUseCars = false;
                                    LSPDFR.GetPedPursuitAttributes(ped).MaxRunningSpeed = (float)Math.Round(1.75f + (MyRandom.NextDouble() * 2 * 0.16), 3, MidpointRounding.ToEven);
                                }
                                LSPDFR.SetPursuitAsCalledIn(Pursuit);
                                LSPDFR.SetPursuitCopsCanJoin(Pursuit, true);
                                LSPDFR.SetPursuitIsActiveForPlayer(Pursuit, true);
                                status = 5;
                                break;
                            case 5:
                                Monitor();
                                if (pedTakenCare.Count == Participant.Count) status = 99;
                                break;
                            case 99:
                                DisplaySummary();
                                status = -1;
                                break;
                        }
                        if (status == -1) break;
                        deadBar.Text = deadCount.ToString();
                        arrestedBar.Text = arrestedCount.ToString();
                        escapedBar.Text = escapedCount.ToString();   
                        stuckBar.Text = stuckCount.ToString();
                        complete.Percentage = pedTakenCare.Count / (gangMemberCount * 2);
                        pool.Draw();
                        GameFiber.Yield();
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
        void Monitor()
        {
            foreach (Ped ped in Participant)
            {
                if (ped)
                {
                    if (pedTakenCare.Contains(ped))
                    {
                        continue;
                    }
                    if (ped.IsDead)
                    {
                        deadCount++;
                        if (ped.Metadata.BAR_AttachedBlip != null)
                        {
                            Blip b = ped.Metadata.BAR_AttachedBlip as Blip;
                            if (b) b.Delete();
                        }
                        pedTakenCare.Add(ped);
                    }
                    if (LSPDFR.IsPedArrested(ped))
                    {
                        arrestedCount++;
                        if (ped.Metadata.BAR_AttachedBlip != null)
                        {
                            Blip b = ped.Metadata.BAR_AttachedBlip as Blip;
                            if (b) b.Delete();
                        }
                        pedTakenCare.Add(ped);
                    }

                    if (!ped.IsValid())
                    {
                        escapedCount++;
                        pedTakenCare.Add(ped);
                    }
                }
            }
        }
        void TaskMonitor()
        {
            foreach (Ped ped in Participant)
            {  
                if (ped)
                {
                    if (ped.IsAlive && !ped.IsTaskActive(PedTask.Combat) && !LSPDFR.IsPedGettingArrested(ped) && !ped.IsRagdoll)
                    {
                        stuckCount++;
                        ped.Tasks.Clear();
                        ped.Tasks.FightAgainstClosestHatedTarget(650f);
                    }
                }            
            }
        }     
    }
}
