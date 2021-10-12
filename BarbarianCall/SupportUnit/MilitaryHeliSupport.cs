using BarbarianCall.Freemode;
using BarbarianCall.Extensions;
using LSPD_First_Response.Mod.API;
using Rage;
using N = Rage.Native.NativeFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace BarbarianCall.SupportUnit
{
    public class MilitaryHeliSupport
    {
        private static int _activeNumber = 0;
        public static bool IsAnyMilitaryHeliSupportActive => _activeNumber > 0;
        private readonly List<Model> heliModels = new List<Model>()
        {
            "BUZZARD",
            "BUZZARD2",
            "ANNIHILATOR",
            "ANNIHILATOR2",
            "MAVERICK",
            "FROGGER",
        };
        private Model _model;
        public Vehicle Helicopter { get; private set; }
        public Ped Pilot { get; private set; }
        public List<FreemodePed> Passengers { get; private set; } = new List<FreemodePed>();
        public Blip Blip { get; private set; }
        public Ped CurrentTargetPed { get; private set; }
        public Vector3 SpawnLocation { get; private set; }
        private bool cleaned = false;
        private bool added = false;
        public MilitaryHeliSupport()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    _model = heliModels.GetRandomElement();
                    SpawnLocation = FindSpawnPoint(Game.LocalPlayer.Character.AbovePosition);
                    _model.LoadAndWait();
                    $"{GetType().Name} | Selected model is: {_model.GetDisplayName()}".ToLog();
                    Helicopter = new Vehicle(_model, SpawnLocation);
                    Helicopter.Mods.ApplyAllMods();
                    Helicopter.IsEngineOn = true;
                    Helicopter.IsPersistent = true;
                    Helicopter.TopSpeed += 50f;
                    Blip = new Blip(Helicopter)
                    {
                        Sprite = (BlipSprite)487,
                        Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.RadarArmour),
                        Name = "Heli Support Unit",
                    };
                    UpgradeHeli();
                    N.Natives.SET_HELI_BLADES_FULL_SPEED(Helicopter);
                    Pilot = new Ped("s_m_y_pilot_01", Vector3.Zero, 0f);
                    Pilot.IsPersistent = true;
                    Pilot.BlockPermanentEvents = true;
                    bool success = PreparePed();
                    if (!success)
                    {
                        CleanUp();
                        $"{GetType().Name} | Failed to set up the ped properly".ToLog();
                        return;
                    }
                    Pilot.WarpIntoVehicle(Helicopter, -1);
                    Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.HeliProtect, 20f, 40f, -1.0f, 50, 10, 0, -1.0f);
                    if (N.Natives.xE43701C36CAFF1A4<bool>(Helicopter))
                    {
                        N.Natives.CONTROL_LANDING_GEAR(Helicopter, 1);
                    }
                    Functions.PlayScannerAudio("HELI_APPROACHING_DISPATCH");
                    bool targetFind = false;
                    Stopwatch notFoundTarget = new();
                    if (!added)
                    {
                        _activeNumber++;
                        added = true;
                    }

                    while (true)
                    {
                        GameFiber.Yield();
                        if (Helicopter.DistanceToSquared2D(Game.LocalPlayer.Character) < 2500)
                        {
                            if (!targetFind)
                            {
                                Ped target = FindTarget();
                                if (target)
                                {
                                    $"{GetType().Name} | Target found: {target.Model.Name}".ToLog();
                                    if (notFoundTarget.IsRunning)
                                    {
                                        $"{GetType().Name} | Resetting stopwatch".ToLog();
                                        notFoundTarget.Reset();
                                    }

                                    foreach (Ped ped in Passengers)
                                    {
                                        if (ped)
                                        {
                                            ped.Tasks.Clear();
                                            N.Natives.REGISTER_​TARGET(ped, target);
                                            N.Natives.TASK_COMBAT_PED(ped, target, 0, 16);
                                            ped.KeepTasks = true;
                                        }
                                    }
                                    Pilot.HeliMission(Helicopter, null, target, Vector3.Zero, MissionType.Circle, 8f, 10f, -1.0f, 50, 30, 0, -1.0f);
                                    targetFind = true;
                                    CurrentTargetPed = target;
                                }
                                else
                                {
                                    if (!notFoundTarget.IsRunning)
                                    {
                                        $"{GetType().Name} Starting stopwatch".ToLog();
                                        notFoundTarget.Start();
                                    }
                                    if (Helicopter && Helicopter.GetActiveMissionType() != MissionType.HeliProtect) Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.Follow, 20f, 10f, -1.0f, 100, 50, 0, -1.0f);
                                    GameFiber.Wait(500);
                                    continue;
                                }
                            }
                            else
                            {
                                if (CurrentTargetPed && targetFind)
                                {
                                    if ((CurrentTargetPed && CurrentTargetPed.IsDead) || !CurrentTargetPed.Exists())
                                    {
                                        CurrentTargetPed = null;
                                        targetFind = false;
                                    }
                                }
                            }
                            if (!targetFind && notFoundTarget.IsRunning && notFoundTarget.ElapsedMilliseconds > 60000)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (Helicopter && Helicopter.GetActiveMissionType() != MissionType.HeliProtect) Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.HeliProtect, 20f, 40f, -1.0f, 50, 20, 0);
                            N.Natives.SET_​DRIVE_​TASK_​CRUISE_​SPEED(Pilot, 40f);
                        }
                        if (
                        (Helicopter &&
                        (Helicopter.IsDead || Helicopter.IsInBurnout || (!Helicopter.IsInAir && Helicopter.Speed < 0.95f) ||
                        N.Natives.GET_HELI_TAIL_ROTOR_HEALTH<float>(Helicopter) < 5f ||
                        N.Natives.GET_HELI_MAIN_ROTOR_HEALTH<float>(Helicopter) < 5f ||
                        Helicopter.EngineHealth < 200.0f))
                        || Passengers.All(x => x && x.IsDead) || !Helicopter || (Pilot && Pilot.IsDead))
                        {
                            Functions.PlayScannerAudio("HELI_MAYDAY_DISPATCH");
                            if (Helicopter) $"Tail Rotor: {N.Natives.GET_HELI_TAIL_ROTOR_HEALTH<float>(Helicopter)}. Main Rotor: {N.Natives.GET_HELI_MAIN_ROTOR_HEALTH<float>(Helicopter)}".ToLog();
                            break;
                        }
                    }
                    CleanUp();
                }
                catch (System.Exception e)
                {
                    if (!cleaned)
                    {
                        _activeNumber--;
                        cleaned = true;
                    }
                    e.ToString().ToLog();
                    CleanUp();
                }
            });
        }
        private Vector3 FindSpawnPoint(Vector3 targetPos)
        {
            Vector3 vector3 = targetPos.Around2D(Peralatan.Random.Next(450, 801)) + Vector3.WorldUp * 850.0f;
            while (targetPos.DistanceToSquared(vector3) < 160000.0f)
            {
                vector3 = targetPos.Around2D(Peralatan.Random.Next(450, 801)) + Vector3.WorldUp * Peralatan.Random.Next(500, 850);
                GameFiber.Yield();
            }
            return vector3;
        }
        private bool PreparePed()
        {
            Ped noosePed;
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                Vehicle noose = Functions.RequestBackup(new Vector3(0f, 0f, 900f), LSPD_First_Response.EBackupResponseType.Code2, LSPD_First_Response.EBackupUnitType.NooseTeam, string.Empty, true, true, 4);
                GameFiber.Wait(500);
                noose.IsPositionFrozen = true;
                noosePed = noose.Occupants.ToList().Where(x => x.IsMale).GetRandomElement();
                if (noosePed && noosePed.IsFreemodePed())
                {
                    "MilitaryHeliSupport | Found a male occupant".ToLog();
                    noosePed.Position = Vector3.RelativeLeft;
                    noosePed.Opacity = 0.0f;
                    noosePed.IsPositionFrozen = true;
                    noosePed.IsCollisionEnabled = false;
                    foreach (Ped ped in noose.Occupants)
                    {
                        if (ped && ped != noosePed) ped.Delete();
                    }
                    if (noose) noose.Delete();
                    break;
                }
                foreach (Ped ped in noose.Occupants)
                {
                    if (ped) ped.Delete();
                }
                if (noose) noose.Delete();
                if (sw.ElapsedMilliseconds > 5000)
                {
                    return false;
                }
            }
            for (int i = 0; i < N.Natives.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS<int>(Helicopter); i++)
            {
                $"{GetType().Name} | Creating passenger {i + 1}".ToLog();
                FreemodePed pass = new(Vector3.Zero, true)
                {
                    IsPersistent = true,
                    BlockPermanentEvents = true,
                    Accuracy = Peralatan.Random.Next(95, 101),
                    MaxHealth = 1500,
                    Health = 1500,
                    RelationshipGroup = Game.LocalPlayer.Character.RelationshipGroup,
                    CanBeTargetted = false
                };
                pass.Wardrobe.CopyFromPed(FreemodePed.FromRegularPed(noosePed));
                N.Natives.GIVE_DELAYED_WEAPON_TO_PED(pass, 0xDBBD7280, -1, true);
                pass.WarpIntoVehicle(Helicopter, i);
                N.Natives.SET_PED_COMBAT_ATTRIBUTES(pass, 5, true);
                N.Natives.SET_PED_COMBAT_ATTRIBUTES(pass, 2, true);
                N.Natives.SET_PED_COMBAT_ATTRIBUTES(pass, 3, false);
                N.Natives.SET_PED_COMBAT_MOVEMENT(pass, 2);
                N.Natives.SET_PED_COMBAT_ABILITY(pass, 2);
                N.Natives.SET_PED_COMBAT_RANGE(pass, 4);
                N.Natives.SET_PED_TARGET_LOSS_RESPONSE(pass, 1);
                N.Natives.SET_PED_HIGHLY_PERCEPTIVE(pass, true);
                N.Natives.SET_PED_CAN_BE_TARGETTED(pass, true);
                N.Natives.SET_PED_SEEING_RANGE(pass, 5 + 100f);
                N.Natives.SET_PED_VISUAL_FIELD_PERIPHERAL_RANGE(pass, 400f);
                N.Natives.SET_COMBAT_FLOAT(pass, 10, 400f);
                Passengers.Add(pass);
            }
            if (noosePed) noosePed.Delete();
            return true;
        }
        private Ped FindTarget()
        {
            foreach (Ped ped in Game.LocalPlayer.Character.GetNearbyPeds(16).OrderBy(x => Vector3.DistanceSquared(x.Position, Game.LocalPlayer.Character)))
            {
                if (ped)
                {
                    if (ped.IsDead || Functions.IsPedACop(ped) || Functions.IsPedArrested(ped) || !ped.IsHuman) continue;
                    if (ped.CombatTarget == Game.LocalPlayer.Character) return ped;
                    if (Game.LocalPlayer.Character.HasBeenDamagedBy(ped) && ped.GetRelationshipAgainst(Game.LocalPlayer.Character) == Relationship.Hate) return ped;
                }
            }
            //$"{GetType().Name} Target not found".ToLog();
            return null;
        }
        private void UpgradeHeli()
        {
            int[] mods = { 11, 12, 15, 16, 18 };
            if (Helicopter)
            {
                foreach(int i in mods)
                {
                    try
                    {
                        int maxMods = N.Natives.GET_NUM_VEHICLE_MODS<int>(Helicopter, i);
                        if (maxMods <= 0) continue;
                        N.Natives.SET_VEHICLE_MOD(Helicopter, i, maxMods, false);
                    }
                    catch (System.Exception e)
                    {
                        e.ToString().ToLog();
                        continue;
                    }
                }
                try
                {

                }
                catch (System.Exception e)
                {
                    e.ToString().ToLog();
                }
            }
        }
        public void CleanUp()
        {
            if (Pilot)
            {
                Functions.SetCopAsBusy(Pilot, false);
                Pilot.Dismiss();
            }
            if (Helicopter)
            {
                Helicopter.Dismiss();
            }
            if (Blip) Blip.Delete();
            foreach(Ped ped in Passengers)
            {
                if (ped) ped.Dismiss();
            }
            _model.Dismiss();
            if (!cleaned)
            {
                _activeNumber--;
                cleaned = true;
            }
        }
    }
}
