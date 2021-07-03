using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPDFR = LSPD_First_Response.Mod.API.Functions;
using BarbarianCall.Types;
using BarbarianCall.Freemode;
using BarbarianCall.Extensions;
using System.Diagnostics;
using BarbarianCall.API;

namespace BarbarianCall.Callouts
{
    [LSPD_First_Response.Mod.Callouts.CalloutInfo("Armored Person In Vehicle", LSPD_First_Response.Mod.Callouts.CalloutProbability.Medium)]
    class ArmoredPersonInVehicle : CalloutBase
    {
        private new FreemodePed Suspect;
        private FreemodePed Passenger;
        public override bool OnBeforeCalloutDisplayed()
        {
            DeclareVariable();
            CalloutRunning = false;
            PursuitCreated = false;
            Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed.Position, 425, 725, true);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint2(PlayerPed.Position, 425, 725);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint3(PlayerPed.Position, 425, 725);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint(PlayerPed.Position, 350, 800);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint2(PlayerPed.Position, 350, 800);
            if (Spawn == Spawnpoint.Zero) Spawn = SpawnManager.GetVehicleSpawnPoint3(PlayerPed.Position, 350, 800);
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
            CarModel = Globals.AudibleCarModel.GetRandomElement(m => m.IsInCdImage && m.IsValid && (m.IsCar || m.IsBike)
           && !m.IsEmergencyVehicle && !m.IsLawEnforcementVehicle && !m.IsBigVehicle && m.NumberOfSeats >= 2, true);
            CarModel.LoadAndWait();
            VehicleSkin skin = new(CarModel);
            skin.LicensePlate = Peralatan.GetRandomPlateNumber();
            SuspectCar = VehicleSkin.CreateVehicle(skin, CalloutPosition, Spawn.Heading);
            SuspectCar.IsPersistent = true;
            SuspectCar.Mods.ApplyAllMods();
            SuspectCar.IsStolen = Peralatan.Random.Next() % 4 == 1;
            Suspect = new FreemodePed(Position, Peralatan.Random.Next() % 2 == 1);
            Passenger = new FreemodePed(Position, Peralatan.Random.Next() % 2 == 1);
            Suspect.MakeMissionPed();
            Passenger.MakeMissionPed();
            Suspect.MaxHealth = Peralatan.Random.Next(2750, 3750);
            Suspect.Health = Suspect.MaxHealth; 
            Passenger.MaxHealth = Peralatan.Random.Next(2550, 3200);
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
            Suspect.WarpIntoVehicle(SuspectCar, -1);
            Passenger.WarpIntoVehicle(SuspectCar, 0);
            Suspect.Tasks.CruiseWithVehicle(35f, (VehicleDrivingFlags)20);
            Blip = new Blip(SuspectCar.Position, 80f)
            {
                Color = Yellow,
                IsRouteEnabled = true,
                RouteColor = Yellow,
            };
            Logical();
            CalloutMainFiber.Start();
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }
        public override void End()
        {
            if (Blip) Blip.Delete();
            base.End();
        }

        public override void Process()
        {
            base.Process();
        }
        void GetClose()
        {
            var curPos = SuspectCar.Position;
            StopWatch = Stopwatch.StartNew();
            while (CalloutRunning)
            {
                GameFiber.Yield();
                if (StopWatch.ElapsedMilliseconds > 20000 && SuspectCar.DistanceToSquared(curPos) > 2500f)
                {
                    curPos = SuspectCar.Position;
                    if (Blip) Blip.Delete();
                    Blip = new Blip(SuspectCar.Position.Around2D(20f), 150f)
                    {
                        Color = Yellow,
                        IsRouteEnabled = true,
                        RouteColor = Yellow,
                        Name = "Suspect Last Seen",
                    };
                    Blip.EnableRoute(Yellow);
                    LSPDFRFunc.PlayScannerAudioUsingPosition(string.Format("SUSPECT_HEADING {0} IN_OR_ON_POSITION", SuspectCar.GetCardinalDirectionLowDetailedAudio()), SuspectCar.Position);
                    StopWatch.Restart();
                }
                if (PlayerPed.DistanceToSquared(SuspectCar) < 625f)
                {
                    if (Blip) Blip.Delete();
                    break;
                }
            }
        }
        void Logical()
        {
            CalloutMainFiber = GameFiber.StartNew(() =>
            {
                try
                {
                    List<FreemodePed> Suspects = new List<FreemodePed>() { Suspect, Passenger, };                 
                    GetClose();
                    if (!CalloutRunning) return;
                    Blip = new(Suspect)
                    {
                        Scale = 0.75f,
                        Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Red),
                        Name = "Armored Person",
                    };
                    CalloutBlips.Add(Blip);
                    Blip passBlip = new Blip(Passenger)
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
                            s.FiringPattern = FiringPattern.SingleShot;
                            s.Tasks.FightAgainstClosestHatedTarget(1000f);
                        }
                    });
                    while(CalloutRunning)
                    {
                        GameFiber.Yield();
                        Suspects.ForEach(s =>
                        {
                            if (s && (s.IsTaskActive(PedTask.DoNothing) || !s.IsTaskActive(PedTask.CombatClosestTargetInArea)) && s.IsAlive) s.Tasks.FightAgainstClosestHatedTarget(1000f);
                        });
                        if (Suspects.All(s => (s && s.IsDead) || !s)) break;
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
    }
}
