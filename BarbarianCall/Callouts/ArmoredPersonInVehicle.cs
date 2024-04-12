using System;
using System.Collections.Generic;
using System.Linq;
using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using BarbarianCall.Types;
using BarbarianCall.Freemode;
using BarbarianCall.Extensions;
using System.Diagnostics;
using BarbarianCall.API;
using RAGENativeUI.Elements;

namespace BarbarianCall.Callouts
{
    [LSPD_First_Response.Mod.Callouts.CalloutInfo("Armored Person In Vehicle", LSPD_First_Response.Mod.Callouts.CalloutProbability.Medium)]
    class ArmoredPersonInVehicle : CalloutBase
    {
        private new FreemodePed Suspect;
        private FreemodePed Passenger;
        private RelationshipGroup Criminal;
        public override bool OnBeforeCalloutDisplayed()
        {
            DeclareVariable();
            CalloutRunning = false;
            PursuitCreated = false;
            Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed.Position, 425, 725, true);
            if (Spawn == Spawnpoint.Zero)
            {
                $"{GetType().Name} | Spawnpoint is not found, cancelling the callout".ToLog();
                return false;
            }          
            Position = Spawn;
            CalloutPosition = Spawn;
            CalloutAdvisory = "Please be careful! suspect is very dangerous";
            CalloutMessage = "Armored Person In Vehicle";
            FriendlyName = "Armored Person In Vehicle";
            PlayScannerWithCallsign("CITIZENS_REPORT CRIME_BRANDISHING_WEAPON IN_OR_ON_POSITION", CalloutPosition);
            AddMinimumDistanceCheck(100f, CalloutPosition);
            ShowCalloutAreaBlipBeforeAccepting(CalloutPosition, 25f);
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            CarModel = Globals.ScannerVehicleModel.GetRandomElement(m => m.IsInCdImage && m.IsValid && (m.IsCar || m.IsBike)
           && !m.IsEmergencyVehicle && !m.IsLawEnforcementVehicle && !m.IsBigVehicle && m.NumberOfSeats >= 2, true);
            CarModel.LoadAndWait();
            VehicleSkin skin = new(CarModel);
            skin.LicensePlate = GenericUtils.GetRandomPlateNumber();
            SuspectCar = VehicleSkin.CreateVehicle(skin, CalloutPosition, Spawn.Heading);
            CalloutEntities.Add(SuspectCar);
            SuspectCar.IsPersistent = true;
            SuspectCar.Mods.ApplyAllMods();
            SuspectCar.IsStolen = MyRandom.Next() % 4 == 1;
            Suspect = new FreemodePed(Position, MyRandom.Next() % 2 == 1);
            CalloutEntities.Add(Suspect);
            Passenger = new FreemodePed(Position, MyRandom.Next() % 2 == 1);
            CalloutEntities.Add(Passenger);
            Suspect.MakeMissionPed();
            Passenger.MakeMissionPed();
            Suspect.MaxHealth = MyRandom.Next(2750, 3750);
            Suspect.Health = Suspect.MaxHealth; 
            Passenger.MaxHealth = MyRandom.Next(2550, 3200);
            Passenger.Health = Suspect.MaxHealth;
            Suspect.Armor = 1000;
            Passenger.Armor = 1000;
            Suspect.SetJuggernautComponent();
            Passenger.SetJuggernautComponent();
            Suspect.SufferCriticalHit(false);
            Passenger.SufferCriticalHit(false);
            Suspect.CanRagdoll = false;
            Passenger.CanRagdoll = false;
            LSPDFR.SetPedCantBeArrestedByPlayer(Suspect, true);
            LSPDFR.SetPedCantBeArrestedByPlayer(Passenger, true);
            Suspect.SetPedAsWanted();
            Passenger.SetPedAsWanted();
            Criminal = new RelationshipGroup("CRIMINAL");
            Suspect.RelationshipGroup = Criminal;
            Passenger.RelationshipGroup = Criminal;
            Suspect.WarpIntoVehicle(SuspectCar, -1);
            Passenger.WarpIntoVehicle(SuspectCar, 0);
            Suspect.VehicleMission(PlayerPed, MissionType.Flee, 35f, Globals.Sheeesh, -1.0f, -1.0f, true);
            Blip = new Blip(SuspectCar.Position, 80f)
            {
                Color = Yellow,
                IsRouteEnabled = true,
                RouteColor = Yellow,
            };
            SendCIMessage("Suspect is highly armored");
            Logical();
            CalloutMainFiber.Start();
            GameFiber.StartNew(() =>
            {
                Ped wanted = (new[] { Suspect, Passenger}).GetRandomElement();
                Manusia = new Manusia(wanted, LSPDFRFunc.GetPedPersona(wanted), SuspectCar);
                LSPDFRFunc.WaitAudioScannerCompletion();
                GameFiber.Sleep(1000);
                Manusia.DisplayNotif();
                GameFiber.Wait(1500);
                LSPDFRFunc.WaitAudioScannerCompletion();
                LSPDFRFunc.PlayScannerAudio($"SUSPECT_IS DRIVING_A {SuspectCar.GetColorAudio()} {GenericUtils.GetPoliceScannerAudio(SuspectCar)} BAR_TARGET_PLATE {GenericUtils.GetLicensePlateAudio(SuspectCar)}", true);
                GameFiber.Wait(2500);
                DisplayGPNotif();
            });
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void End()
        {
            if (Blip) Blip.Delete();
            RelationshipGroup? group = Criminal;
            if (group.HasValue) Extension.DeleteRelationshipGroup(group.Value);
            base.End();
        }
        protected override void CleanUp()
        {
            // TODO: implement CleanUp()
        }
        public override void Process()
        {
            base.Process();
        }
        void GetClose()
        {
            var curPos = SuspectCar.Position;
            StopWatch = Stopwatch.StartNew();
            ResText resText = new("SUSPECT", new(0, 0), 0.75f, Yellow, RAGENativeUI.Common.EFont.ChaletLondon, ResText.Alignment.Centered);
            while (CalloutRunning)
            {
                GameFiber.Yield();
                Rage.Debug.DrawLine(PlayerPed.IsInAnyVehicle(false) ? PlayerPed.CurrentVehicle.FrontPosition : PlayerPed.FrontPosition, SuspectCar.Position, Yellow);
                Vector3 pos = SuspectCar.AbovePosition + new Vector3(0f, 0f, 1f);
                if (Rage.Native.NativeFunction.Natives.GET_SCREEN_COORD_FROM_WORLD_COORD<bool>(pos.X, pos.Y, pos.Z, out float xs, out float ys))
                {
                    resText.Position = new(Convert.ToInt32(xs), Convert.ToInt32(ys));
                    resText.Draw();
                }
                if (StopWatch.ElapsedMilliseconds > 20000 && SuspectCar.DistanceToSquared(curPos) > 2500f)
                {
                    curPos = SuspectCar.Position;
                    if (Blip) Blip.Delete();
                    Blip = new Blip(SuspectCar.Position.Around2D(20f), 80f)
                    {
                        Color = Yellow,
                        IsRouteEnabled = true,
                        RouteColor = Yellow,
                        Name = "Suspect Last Seen",
                    };
                    Blip.EnableRoute(Yellow);
                    LSPDFRFunc.PlayScannerAudioUsingPosition(string.Format("SUSPECT_HEADING {0} IN_OR_ON_POSITION", SuspectCar.GetCardinalDirectionLowDetailedAudio()), SuspectCar.Position);
                    SendCIMessage($"Suspect last seen in {LSPDFR.GetZoneAtPosition(SuspectCar.Position).RealAreaName} driving with {MathHelper.ConvertMetersPerSecondToKilometersPerHourRounded(SuspectCar.Speed)} KM/H");
                    StopWatch.Restart();
                }
                if (PlayerPed.DistanceToSquared(SuspectCar) < 625f)
                {
                    if (Blip) Blip.Delete();
                    break;
                }
            }
        }
        void SetRelationship()
        {
            Criminal.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Medic, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Fireman, Relationship.Hate);
            Criminal.SetRelationshipWith(RelationshipGroup.Player, Relationship.Hate);
            Criminal.SetRelationshipWith(PlayerPed.RelationshipGroup, Relationship.Hate);
            RelationshipGroup.Cop.SetRelationshipWith(Criminal, Relationship.Hate);
            RelationshipGroup.Fireman.SetRelationshipWith(Criminal, Relationship.Hate);
            RelationshipGroup.Medic.SetRelationshipWith(Criminal, Relationship.Hate);
        }
        void DisplayCasualities()
        {
            int civilianDead = 0;
            int copsDead = 0;
            int vehicleExplodes = 0;
            foreach(Ped ped in World.GetAllPeds())
            {
                if (ped)
                {
                    if (ped.IsHuman && ped.IsDead)
                    {
                        if (LSPDFR.IsPedACop(ped)) copsDead++;
                        else if (ped.RelationshipGroup != Criminal) civilianDead++;
                    }
                }
            }
            foreach (Vehicle vehicle in World.GetAllVehicles())
            {
                if (vehicle && (vehicle.IsCar || vehicle.IsBike || vehicle.IsBicycle))
                {
                    if (vehicle.IsDead) vehicleExplodes++;
                }
            }
        }
        void Logical()
        {
            CalloutMainFiber = new(() =>
            {
                try
                {
                    List<FreemodePed> Suspects = new() { Suspect, Passenger, };
                    SetRelationship();
                    GetClose();
                    if (!CalloutRunning) return;
                    Blip = new(Suspect)
                    {
                        Scale = 0.75f,
                        Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Red),
                        Name = "Armored Person",
                    };
                    CalloutBlips.Add(Blip);
                    Blip passBlip = new(Passenger)
                    {
                        Scale = 0.75f,
                        Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Red),
                        Name = "Armored Person",
                    };
                    CalloutBlips.Add(passBlip);
                    Suspects.ForEach(s =>
                    {
                        if (s)
                        {
                            s.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen).WaitForCompletion(3000);
                            WeaponAsset weapon = new(Globals.WeaponMKIIAndItsComponents.GetRandomElement().Key);
                            s.Inventory.GiveNewWeapon(weapon, -1, true);
                            var components = Globals.WeaponMKIIAndItsComponents[weapon];
                            components.ForEach(x => s.Inventory.AddComponentToWeapon(weapon, x));
                            s.FiringPattern = FiringPattern.BurstFireShortBursts;
                            s.Tasks.FightAgainstClosestHatedTarget(1000f);
                        }
                    });
                    SendCIMessage("Shots Fired!!");
                    LSPDFR.RequestBackup(PlayerPed.Position, LSPD_First_Response.EBackupResponseType.Code3, LSPD_First_Response.EBackupUnitType.SwatTeam);
                    SendCIMessage("SWAT unit dispatced");
                    bool explode = false;
                    while(CalloutRunning)
                    {
                        GameFiber.Yield();
                        Suspects.ForEach(s =>
                        {
                            if (s && (s.IsTaskActive(PedTask.DoNothing) || !s.IsTaskActive(PedTask.CombatClosestTargetInArea)) && s.IsAlive) s.Tasks.FightAgainstClosestHatedTarget(1000f);
                        });
                        if (Suspects.All(s => (s && s.IsDead) || !s)) break;
                        if (!explode && SuspectCar && PlayerPed.DistanceToSquared(SuspectCar) < 8f && Suspects.All(x => x && x.DistanceToSquared(SuspectCar) > 625f))
                        {
                            SuspectCar.Explode(true);
                            explode = true;
                        }
                    }
                    if (!CalloutRunning) return;
                    End();
                }
                catch (Exception e)
                {
                    $"{GetType().Name} callout crashes".ToLog();
                    e.ToString().ToLog();
                    NetExtension.SendError(e);
                    $"{GetType().Name} callout crashed, please send your log".DisplayNotifWithLogo("Officer Stabbed");
                    End();
                }
            });
        }
        void Logical2()
        {

        }
    }
}
