using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Rage;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using LSPD_First_Response.Engine.Scripting.Entities;
using BarbarianCall.Extensions;
using BarbarianCall.SupportUnit;
using BarbarianCall.Types;
using BarbarianCall.API;

namespace BarbarianCall.Callouts
{
    [LSPD_First_Response.Mod.Callouts.CalloutInfo("Heart Attack Civilian", LSPD_First_Response.Mod.Callouts.CalloutProbability.High)]
    public class HeartAttackCivilian : CalloutBase
    {
        public Ped Civilian;
        private Ped Paramedic1;
        private Ped Paramedic2;
        private Vehicle Ambulance;
        private const VehicleDrivingFlags flags = VehicleDrivingFlags.DriveAroundPeds | VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.AllowWrongWay | VehicleDrivingFlags.AllowMedianCrossing;
        private Blip ambulanceBlip;
        private HeliSupport heli;
        private enum CalloutState
        {
            EnRoute,
            OnScene,
            CivilianEnterAmbulance,
            EscortingAmbulance,
        }

        private new CalloutState State;
        public override bool OnBeforeCalloutDisplayed()
        {
            Spawn = SpawnManager.GetPedSpawnPoint(PlayerPed, 300,500);
            if (Spawn == Spawnpoint.Zero)
            {
                Displayed = false;
                return false;
            }
            if (PlayerPed.Position.GetZoneName().ToLower() == "island")
            {
                "Cayo Perico Island is not supported yet".ToLog();
                Displayed = false;
                return false;
            }
            var nameRnd = PersonaHelper.GetRandomName(LSPD_First_Response.Gender.Male);
            var model = Extension.GetRandomMaleModel();
            "Creating male ped".ToLog();
            Civilian = new Ped(model, Spawn, Spawn);
            CalloutEntities.Add(Civilian);
            Civilian.MakeMissionPed();
            "Cloning ped persona".ToLog();
            Persona clone = Persona.FromExistingPed(Civilian);
            clone.Forename = nameRnd.Item1;
            clone.Surname = nameRnd.Item2;
            $"Setting persona for ped {Civilian.Model.Name}".ToLog();
            LSPDFR.SetPersonaForPed(Civilian, clone);
            CalloutPosition = Spawn;
            CalloutMessage = "Heart Attack Civilian";
            CalloutAdvisory = $"The Civilian name is {nameRnd.Item1} {nameRnd.Item2}";
            PlayScannerWithCallsign("BAR_CODE_99 BAR_CRIME_CIVILIAN_NEEDING_ASSISTANCE IN_OR_ON_POSITION", CalloutPosition);
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 30f);
            AddMinimumDistanceCheck(100f, CalloutPosition);
            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            Blip = new Blip(Spawn.Position.Around2D(20f).ToGround(), 25)
            {
                Color = Yellow,
                IsRouteEnabled = true
            };
            CalloutBlips.Add(Blip);
            "Playing fall animation".ToLog();
            Civilian.PlayScenarioAction(PedScenario.WORLD_HUMAN_STUPOR, true);
            Civilian.IsInvincible = true;
            CalloutRunning = true;
            "Getting Callout Main Fiber content".ToLog();
            MainSituation();
            "Starting Fiber".ToLog();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();
        }
        public override void End()
        {
            if (State != CalloutState.EscortingAmbulance)
            {

            }
            if (Game.IsScreenFadedOut) Game.FadeScreenIn(3000);
            if (Game.IsScreenFadingOut)
            {
                StopWatch = new();
                StopWatch.Start();
                while(true)
                {
                    GameFiber.Yield();
                    if (Game.IsScreenFadedOut || StopWatch.ElapsedMilliseconds > 5000) break;
                }
                Game.FadeScreenIn(3000);
            }
            base.End();
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }
        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        private void GetClose()
        {
            State = CalloutState.EnRoute;
            while (CalloutRunning)
            {
                GameFiber.Yield();
                var traceStart = PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition;
                Extension.DrawLine(traceStart, Civilian.Position, Color.Red);
                var raycast = World.TraceLine(traceStart, Civilian.Position, TraceFlags.None);
                if (Civilian && raycast.Hit && raycast.HitEntity && raycast.HitEntity == Civilian)
                {
                    "Raycast hit civilian".ToLog();
                    break;
                }                
                else if (Civilian && PlayerPed.DistanceToSquared(Civilian) < 900f) break;
            }
            $"Distance: {PlayerPed.DistanceTo(Civilian)}".ToLog();
            State = CalloutState.OnScene;
            if (Blip) Blip.Delete();
            Blip = Civilian.AttachBlip();
            Blip.Color = Color.Orange;
            Blip.SetBlipName("Heart Attack Civilian");
            CalloutBlips.Add(Blip);
        }
        private void MainSituation()
        {
            CalloutMainFiber = new GameFiber(() =>
            {
                try
                {
                    GetClose();
                    if (!CalloutRunning) return;
                    AnimationDictionary dictionary = "anim@amb@machinery@vertical_mill@";
                    List<Ped> ambient = new();
                    GameFiber.StartNew(() =>
                    {
                        dictionary.LoadAndWait();
                        Civilian.GetNearbyPeds(16).Where(x => x.IsAmbientOnFoot()).ToList().ForEach(x =>
                        {
                            if (x && CalloutRunning)
                            {
                                x.Tasks.Clear();
                                CalloutEntities.Add(x);
                                ambient.Add(x);
                                x.Tasks.AchieveHeading(x.GetHeadingTowards(Civilian)).WaitForCompletion(1000);
                                x.Tasks.PlayAnimation(dictionary, "look_at_object_amy_skater_01", 8.0f, AnimationFlags.Loop | AnimationFlags.SecondaryTask);
                                x.PlayAmbientSpeech(Speech.GENERIC_SHOCKED_HIGH);
                            }
                        });
                    });                 
                    Spawnpoint ambulanceSpawn = SpawnManager.GetVehicleSpawnPoint(CalloutPosition, 100, 150);
                    if (ambulanceSpawn == Spawnpoint.Zero) ambulanceSpawn = new Spawnpoint(World.GetNextPositionOnStreet(CalloutPosition.Around2D(250)), 0f);
                    if (UltimateBackupRunning)
                    {
                        var ubAmb = UltimateBackupFunc.GetUnit(EUltimateBackupUnitType.Ambulance, ambulanceSpawn, 2);
                        Paramedic1 = ubAmb.Item2.First();
                        Paramedic2 = ubAmb.Item2.Last();
                        Ambulance = ubAmb.Item1;
                    }
                    else
                    {
                        Ambulance = new Vehicle("AMBULANCE", ambulanceSpawn);
                        Paramedic1 = new Ped("s_m_m_paramedic_01", ambulanceSpawn, ambulanceSpawn);
                        Paramedic2 = new Ped("s_m_m_paramedic_01", ambulanceSpawn, ambulanceSpawn);
                        Paramedic1.RandomizeVariation();
                        Paramedic2.RandomizeVariation();
                        if (Paramedic1) Paramedic1.WarpIntoVehicle(Ambulance, -1);
                        if (Paramedic2) Paramedic2.WarpIntoVehicle(Ambulance, 0);
                    }
                    if (Ambulance) Ambulance.Heading = ambulanceSpawn;
                    if (Ambulance) Ambulance.RandomizeLicensePlate();
                    List<Entity> ambulanceEntities = new() { Ambulance, Paramedic1, Paramedic2 };
                    ambulanceEntities.ForEach(e => e.MakePersistent());
                    ambulanceEntities.ForEach(e => CalloutEntities.Add(e));
                    if (Paramedic1)
                    {
                        Paramedic1.BlockPermanentEvents = true;
                        Paramedic1.IsInvincible = true;
                    }

                    if (Paramedic2)
                    {
                        Paramedic2.BlockPermanentEvents = true;
                        Paramedic2.IsInvincible = true;
                    }
                    if (Ambulance)
                    {
                        if (Ambulance.HasSiren)
                        {
                            Ambulance.IsSirenOn = true;
                            Ambulance.IsSirenSilent = false;
                        }
                        Ambulance.Mods.ApplyAllMods();
                    }
                    var task1 = Paramedic1.VehicleMission(Civilian, MissionType.GoTo, 35f, flags, 5f, 35f, true);
                    ambulanceBlip = new Blip(Ambulance)
                    {
                        Sprite = (BlipSprite)489,
                        Name = "Ambulance",
                        IsFriendly = true,
                        Scale = 1.25f,
                        Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.RadarHealth),
                    };
                    CalloutBlips.Add(ambulanceBlip);
                    "~g~Ambulance~s~ is en route to your current location".DisplayNotifWithLogo("Heart Attack Civilian", fadeIn: true, blink: true, hudColor: RAGENativeUI.HudColor.RedDark, icon: NotificationIcon.RightJumpingArrow);
                    SendCIMessage("Test");
                    StopWatch = new();
                    StopWatch.Start();
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (!task1.IsActive || StopWatch.ElapsedMilliseconds > 100000) 
                        {
                            $"Task Status: {task1.Status}, Elapsed: {StopWatch.ElapsedMilliseconds} ms".ToLog();
                            break; 
                        }
                        if (Ambulance.DistanceToSquared(CalloutPosition) < 225f)
                        {
                            Paramedic1.VehicleMission(CalloutPosition, MissionType.Stop, 5f, VehicleDrivingFlags.None, -1.0f, -1.0f, true).WaitForCompletion(500);
                            break;
                        }
                    }
                    if (!CalloutRunning) return;
                    uint speedZone = World.AddSpeedZone(CalloutPosition, 200f, 10f);
                    Vector3 wpos = Civilian.Position + Vector3.RelativeRight;
                    if (Paramedic1) Paramedic1.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                    if (Paramedic2) Paramedic2.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(10000);
                    Task para1Task = null;
                    if (Paramedic1) para1Task = Paramedic1.Tasks.FollowNavigationMeshToPosition(Civilian.Position + Civilian.RightVector, Paramedic1.GetHeadingTowards(Civilian), 10f);
                    if (Paramedic2) Paramedic2.Tasks.FollowNavigationMeshToPosition(Civilian.Position + Civilian.RightVector * -1f, Paramedic2.GetHeadingTowards(Civilian), 10f).WaitForCompletion(10000);
                    if (para1Task != null) para1Task.WaitForCompletion(10000);
                    if (Paramedic1 && Paramedic1.DistanceTo(Civilian) > 5f) Paramedic1.SetPositionWithSnap(Civilian.Position + Civilian.RightVector);
                    if (Paramedic2 && Paramedic2.DistanceTo(Civilian) > 5f) Paramedic2.SetPositionWithSnap(Civilian.Position + Civilian.RightVector * -1f);
                    if (Paramedic1) Paramedic1.Tasks.AchieveHeading(Paramedic1.GetHeadingTowards(Civilian));
                    if (Paramedic2) Paramedic2.Tasks.AchieveHeading(Paramedic2.GetHeadingTowards(Civilian)).WaitForCompletion(1000);
                    if (Paramedic1) Paramedic1.PlayScenarioAction(PedScenario.CODE_HUMAN_MEDIC_KNEEL, true);
                    if (Paramedic2) Paramedic2.PlayScenarioAction(PedScenario.CODE_HUMAN_MEDIC_TEND_TO_DEAD, true);
                    GameFiber.Wait(MyRandom.Next(5000, 10000));
                    if (Paramedic1) Paramedic1.Tasks.Clear();
                    if (Paramedic2) Paramedic2.Tasks.Clear();
                    if (Civilian) Civilian.Tasks.Clear();
                    GameFiber.Wait(2000);
                    Task walkTask = null;
                    if (Paramedic1) Paramedic1.Tasks.EnterVehicle(Ambulance, 10000, -1, 10f, EnterVehicleFlags.None);
                    if (Civilian) walkTask = Civilian.Tasks.FollowNavigationMeshToPosition(Ambulance.RearPosition + Vector3.RelativeBack, Ambulance.Heading, 0.5f);
                    if (Paramedic2) Paramedic2.OpenVehicleDoor(Ambulance, 10000, 2, 10f).WaitForCompletion();
                    if (Paramedic2) Paramedic2.Tasks.FollowNavigationMeshToPosition(Ambulance.RearPosition + (Ambulance.ForwardVector * -2), Ambulance.Heading, 0.5f);
                    if (walkTask != null) walkTask.WaitForCompletion(18000);
                    if (Civilian) Civilian.Tasks.EnterVehicle(Ambulance, 2).WaitForCompletion(10000);
                    if (Paramedic2) Paramedic2.Tasks.EnterVehicle(Ambulance, 0, 10f).WaitForCompletion(10000);
                    if (Paramedic1 && !Paramedic1.IsInVehicle(Ambulance, false)) Paramedic1.WarpIntoVehicle(Ambulance, -1);
                    if (Paramedic2 && !Paramedic2.IsInVehicle(Ambulance, false)) Paramedic2.WarpIntoVehicle(Ambulance, 0);
                    if (Civilian && !Civilian.IsInVehicle(Ambulance, false)) Civilian.WarpIntoVehicle(Ambulance, 2);
                    if (!CalloutRunning) return;
                    ambient.ForEach(x =>
                    {
                        if (x)
                        {
                            x.Tasks.ClearSecondary();
                            x.Tasks.Clear();
                            x.Dismiss();
                        }
                    });
                    Spawnpoint hospital = Globals.Hospitals.OrderBy(x => Vector3.DistanceSquared(PlayerPed.Position, x.Position)).FirstOrDefault();
                    GenericUtils.DisplayNotifWithLogo("Please escort this ambulance to the hospital", icon: NotificationIcon.Email, hudColor: RAGENativeUI.HudColor.BlueDark);
                    if (Blip) Blip.Delete();
                    Blip = new Blip(hospital)
                    {
                        IsRouteEnabled = true,
                        Color = Yellow,
                        Name = "Hospital",
                    };
                    CalloutBlips.Add(Blip);
                    heli = new HeliSupport(Ambulance);
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (PlayerPed.IsInAnyVehicle(false)) break;
                    }
                    if (!CalloutRunning) return;
                    World.RemoveSpeedZone(speedZone);
                    if (PlayerPed.CurrentVehicle.HasSiren)
                    {
                        PlayerPed.CurrentVehicle.IsSirenOn = true;
                        PlayerPed.CurrentVehicle.IsSirenSilent = false;
                    }
                    if (Paramedic1) Paramedic1.VehicleMission(PlayerPed.CurrentVehicle, MissionType.Escort, 35f, flags, 8f, 0f, true);
                    Vehicle cv = PlayerPed.CurrentVehicle;
                    
                    while (CalloutRunning)
                    {
                        GameFiber.Yield();
                        if (!PlayerPed.IsInAnyVehicle(false))
                        {
                            Paramedic1.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                            while (CalloutRunning)
                            {
                                GameFiber.Yield();
                                if (PlayerPed.IsInAnyVehicle(false)) break;
                            }
                            if (Paramedic1) Paramedic1.VehicleMission(PlayerPed.CurrentVehicle, MissionType.Escort, 35f, flags, 8f, 0f, true);
                        }                                           
                        if (Ambulance.FrontPosition.DistanceToSquared(cv.RearPosition) < 7f || Ambulance.DistanceToSquared(cv) < 15f)
                        {
                            if (Paramedic1) Paramedic1.VehicleMission(PlayerPed, MissionType.Stop, 1f, VehicleDrivingFlags.None, -1.0f, -1.0f, true).WaitForCompletion(250);
                            if (Paramedic1) Paramedic1.VehicleMission(PlayerPed.CurrentVehicle, MissionType.Escort, 35f, flags, 8f, 0f, true);
                        }
                        if (PlayerPed.DistanceToSquared(hospital) < 625f && PlayerPed.IsInAnyVehicle(false))
                        {
                            $"Distance: {PlayerPed.DistanceToSquared(hospital)}".ToLog();
                            break;
                        }
                    }
                    if (!CalloutRunning) return;
                    Game.LocalPlayer.HasControl = false;
                    PlayerPed.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                    Paramedic1.Tasks.PerformDrivingManeuver(VehicleManeuver.Wait);
                    Game.FadeScreenOut(3000, true);
                    World.GetEntities(hospital, 8f, GetEntitiesFlags.ConsiderAllVehicles | GetEntitiesFlags.ConsiderAllPeds).Where(x => x && !x.CreatedByTheCallingPlugin && !x.IsPersistent && x.GetAttachedBlips().Length == 0).ToList().ForEach(x =>
                     {
                         if (x && x.IsPed()) x.Position = SpawnManager.GetPedSpawnPoint(x.Position, 30f, 200f);
                         else if (x && x.IsVehicle())
                         {
                             Vehicle v = x as Vehicle;
                             Spawnpoint spv = SpawnManager.GetVehicleSpawnPoint(x.Position, 50, 300);
                             if (spv == Spawnpoint.Zero) spv = SpawnManager.GetVehicleSpawnPoint(x.Position, 55f, 350f);
                             if (spv == Spawnpoint.Zero) spv = SpawnManager.GetVehicleSpawnPoint(x.Position, 60f, 360f);
                             if (spv == Spawnpoint.Zero) spv = SpawnManager.GetVehicleSpawnPoint(x.Position, 70f, 380f);
                             x.Position = spv;
                             x.Heading = spv;
                             if (v.IsEmpty) v.CreateRandomDriver().Dismiss();
                         }
                         if (x) x.Dismiss();
                     });
                    if (heli != null) heli?.CleanUp(); 
                    List<Entity> dismiss = new()
                    {
                        Ambulance, Paramedic1, Paramedic2, Civilian
                    };
                    dismiss.ForEach(x =>
                    {
                        if (x)
                        {
                            x.Position = x.IsVehicle() ? SpawnManager.GetVehicleSpawnPoint(x.Position, 150, 400) : SpawnManager.GetPedSpawnPoint(x, 80, 150);
                            x.Dismiss();
                        }
                    });
                    if (ambulanceBlip) ambulanceBlip.Delete();
                    if (Blip) Blip.Delete();
                    if (PlayerPed.IsInAnyVehicle(false))
                    {
                        PlayerPed.CurrentVehicle.Position = hospital;
                        PlayerPed.CurrentVehicle.Heading = hospital;
                    }
                    else PlayerPed.Position = hospital;
                    PlayerPed.Tasks.Clear();
                    Game.LocalPlayer.HasControl = true;
                    GameFiber.Wait(1000);
                    Game.FadeScreenIn(3000, true);
                    "~g~Congratulation~s~ you have saved the patient".DisplayNotifWithLogo(icon: NotificationIcon.Email, hudColor: RAGENativeUI.HudColor.GreenDark);
                    if (!CalloutRunning) return;
                    End();
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    End(e.Message);
                }
            });
        }
    }
}
