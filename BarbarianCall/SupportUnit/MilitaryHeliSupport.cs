using BarbarianCall.Freemode;
using BarbarianCall.Extensions;
using LSPD_First_Response.Mod.API;
using Rage;
using N = Rage.Native.NativeFunction;
using RAGENativeUI.Elements;
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
        internal static List<MilitaryHeliSupport> InternalList = new();
        private readonly List<Model> heliModels = new()
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
        TimerBarPool MyPool = new();
        TextTimerBar TimerBar = new("Mission Type", "");
        TextTimerBar targetDistance = new("Target", "");
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
                    Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.Follow, Helicopter.TopSpeed, 40f, -1.0f, 50, 10, 0, -1.0f);
                    if (N.Natives.xE43701C36CAFF1A4<bool>(Helicopter))
                    {
                        N.Natives.CONTROL_LANDING_GEAR(Helicopter, 1);
                    }
                    Functions.PlayScannerAudio("HELI_APPROACHING_DISPATCH");
                    bool targetFind = false;
                    InternalList.Add(this);
                    if (!added)
                    {
                        _activeNumber++;
                        added = true;
                    }
                    foreach (Ped ped1 in Passengers)
                    {
                        if (ped1)
                        {
                            if (ped1.Inventory.Weapons.Contains(0x969C3D67))
                            {
                                $"{GetType().Name} | Add weapon component to ped".ToLog();
                                ped1.Inventory.AddComponentToWeapon(0x969C3D67, "COMPONENT_SPECIALCARBINE_MK2_CLIP_ARMORPIERCING");
                                ped1.Inventory.AddComponentToWeapon(0x969C3D67, "COMPONENT_AT_SCOPE_MEDIUM_MK2");
                                ped1.Inventory.AddComponentToWeapon(0x969C3D67, "COMPONENT_AT_MUZZLE_07");
                                ped1.Inventory.AddComponentToWeapon(0x969C3D67, "COMPONENT_AT_AR_AFGRIP_02");
                                ped1.Inventory.AddComponentToWeapon(0x969C3D67, "COMPONENT_AT_SC_BARREL_02");
                            }
                        }
                    }
                    MyPool.Add(TimerBar);
                    MyPool.Add(targetDistance);
                    GetClose();
                    int gameTimer = Globals.GameTimer;
                    while (true)
                    {
                        GameFiber.Yield();
                        if (!targetFind)
                        {
                            Ped target = FindTarget();
                            if (target != null)
                            {
                                $"{GetType().Name} | Target found: {target.Model.Name}".ToLog();
                                $"{GetType().Name} | Resetting stopwatch".ToLog();
                                gameTimer = Globals.GameTimer;
                                foreach (Ped ped in Passengers)
                                {
                                    if (ped)
                                    {
                                        if (ped.Metadata.BAR_HELI_WEAPON != null && ped.Inventory.Weapons.Contains(ped.Metadata.BAR_HELI_WEAPON))
                                        {
                                            $"Ped equip weapon".ToLog();
                                            ped.Inventory.EquippedWeapon = ped.Metadata.BAR_HELI_WEAPON;
                                        }
                                        $"Tasking {Functions.GetPersonaForPed(ped).FullName} to attack {Functions.GetPersonaForPed(target).FullName}".ToLog();
                                        ped.Tasks.Clear();
                                        N.Natives.REGISTER_​TARGET(ped, target);
                                        N.Natives.TASK_COMBAT_PED(ped, target, 0, 16);
                                        ped.KeepTasks = true;
                                    }
                                }
                                Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.HeliProtect, 20f, 40f, -1.0f, (int)Math.Ceiling(GetHeight()), 10, 0, -1.0f);
                                targetFind = true;
                                CurrentTargetPed = target;
                            }
                            else
                            {
                                //$"{GetType().Name} Starting stopwatch".ToLog();
                                if (Helicopter && Helicopter.GetActiveMissionType() != MissionType.HeliProtect)
                                {
                                    Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.HeliProtect, 50f, 80f, -1.0f, (int)Math.Ceiling(GetHeight()), 50, 0);
                                }
                                targetFind = false;
                            }
                            targetDistance.Text = "Not Found";
                        }
                        else
                        {
                            if (CurrentTargetPed && targetFind)
                            {
                                if ((CurrentTargetPed && (CurrentTargetPed.IsDead || CurrentTargetPed.DistanceToSquared2D(Helicopter) > 62500)) || !CurrentTargetPed.Exists())
                                {
                                    CurrentTargetPed = null;
                                    targetFind = false;
                                }
                            }
                            if (Helicopter && Helicopter.GetActiveMissionType() != MissionType.HeliProtect)
                            {
                                Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.HeliProtect, 20f, 40f, -1.0f, (int)Math.Ceiling(GetHeight()), 10, 0, -1.0f);
                            }
                            gameTimer = Globals.GameTimer;
                            if (CurrentTargetPed) Types.Marker.DrawMarker(MarkerType.UpsideDownCone, CurrentTargetPed.Position + new Vector3(0f, 0f, CurrentTargetPed.Height / 2), RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Red));
                            if (CurrentTargetPed) targetDistance.Text = CurrentTargetPed.DistanceToSquared(Helicopter).ToString();
                            else targetDistance.Text = "Not Found";
                        }
                        if (!targetFind && Globals.GameTimer - gameTimer > 60000)
                        {
                            break;
                        }
                        if (
                        (Helicopter &&
                        (Helicopter.IsDead || Helicopter.IsInBurnout || (!Helicopter.IsEngineOn && Helicopter.Speed < 0.5f) ||
                        N.Natives.GET_HELI_TAIL_ROTOR_HEALTH<float>(Helicopter) < 10f ||
                        N.Natives.GET_HELI_MAIN_ROTOR_HEALTH<float>(Helicopter) < 10f ||
                        Helicopter.EngineHealth < 200.0f))
                        || Passengers.All(x => x && x.IsDead) || !Helicopter || (Pilot && Pilot.IsDead))
                        {
                            Functions.PlayScannerAudio("HELI_MAYDAY_DISPATCH");
#if DEBUG
                            if (Helicopter)
                            {
                                $"Tail Rotor: {N.Natives.GET_HELI_TAIL_ROTOR_HEALTH<float>(Helicopter)}. Main Rotor: {N.Natives.GET_HELI_MAIN_ROTOR_HEALTH<float>(Helicopter)}. Engine: {Helicopter.EngineHealth}".ToLog();
                                $"In Air: {Helicopter.IsInAir}, Burnout: {Helicopter.IsInBurnout}".ToLog();
                                $"Pilot Is Dead: {Pilot && Pilot.IsDead}".ToLog();
                            }
#endif
                            break;
                        }
                        TimerBar.Text = Helicopter.GetActiveMissionType().ToString();
                        MyPool.Draw();
                    }
                    CleanUp();
                }
                catch (Exception e)
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
        private void GetClose()
        {
            while (true)
            {
                GameFiber.Yield();
                if (Helicopter.DistanceToSquared2D(Game.LocalPlayer.Character) < 2500f)
                {
                    $"{GetType().Name} | Close enough".ToLog();
                    break;
                }
                if (Helicopter.GetActiveMissionType() != MissionType.Follow)
                {
                    Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.Follow, Helicopter.TopSpeed, 40f, -1.0f, 50, 10, 0, -1.0f);
                }
            }
        }
        private bool PreparePed()
        {
            Ped noosePed;
            var unit = Functions.RequestBackup(Vector3.Zero, LSPD_First_Response.EBackupResponseType.Code2, LSPD_First_Response.EBackupUnitType.NooseTeam, string.Empty, true, true, 6);
            noosePed = unit.Passengers.GetRandomElement(x => x);
            noosePed.Position = unit.AbovePosition;
            for (int i = 0; i < N.Natives.GET_VEHICLE_MAX_NUMBER_OF_PASSENGERS<int>(Helicopter); i++)
            {
                $"{GetType().Name} | Creating passenger {i + 1}".ToLog();
                FreemodePed pass;
                Ped ped = noosePed.Clone(false);
                $"Ped exist {ped.Exists()}".ToLog();
                pass = FreemodePed.FromRegularPed(ped);
                pass.IsPersistent = true;
                pass.BlockPermanentEvents = true;
                Functions.SetPedAsCop(pass);
                Functions.SetCopAsBusy(pass, true);
                Functions.SetCopIgnoreAmbientCombatControl(pass, true);
                pass.Accuracy = Peralatan.Random.Next(95, 101);
                pass.MaxHealth = 1500;
                pass.Health = 1500;
                pass.RelationshipGroup = Game.LocalPlayer.Character.RelationshipGroup;
                pass.CanBeTargetted = false;
                if (pass.IsFreemodePed()) pass.RandomizeAppearance();
                pass.Metadata.BAR_HELI_WEAPON = pass.Inventory.GiveNewWeapon(0x969C3D67, -1, false);
                pass.WarpIntoVehicle(Helicopter, i);
                pass.CombatProperty.AlwaysFight = true;
                pass.CombatProperty.CanDoDrivebys = true;
                pass.CombatProperty.CanLeaveVehicle = false;
                pass.CombatProperty.CombatMovement = CombatMovement.WillAdvance;
                pass.CombatProperty.CombatAbility = CombatAbility.Professional;
                pass.CombatProperty.CombatRange = CombatRange.VeryFar;
                pass.CombatProperty.SeeingRange = 105f;
                pass.CombatProperty.HearingRange = 105f;
                pass.CombatProperty.AttackWindowDistanceForCover = 400f;
                N.Natives.SET_PED_TARGET_LOSS_RESPONSE(pass, 1);
                N.Natives.SET_PED_HIGHLY_PERCEPTIVE(pass, true);
                N.Natives.SET_PED_CAN_BE_TARGETTED(pass, true);
                N.Natives.SET_PED_VISUAL_FIELD_PERIPHERAL_RANGE(pass, 400f);               
                Passengers.Add(pass);
            }
            if (noosePed) noosePed.Delete();
            if (unit) unit.Delete();
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
                    if (ped.CombatTarget != null && ped.CombatTarget.RelationshipGroup == RelationshipGroup.Cop) return ped;
                    if (Game.LocalPlayer.Character.HasBeenDamagedBy(ped) && ped.GetRelationshipAgainst(Game.LocalPlayer.Character) == Relationship.Hate) return ped;
                }
            }
            //$"{GetType().Name} Target not found".ToLog();
            return null;
        }
        private float GetHeight()
        {
            float z = N.Natives.x29C24BFBED8AB8FB<float>(Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y);
            z = z < 85f ? 85f : z;
            return z;
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
                    catch (Exception e)
                    {
                        e.ToString().ToLog();
                        continue;
                    }
                }               
            }
            Helicopter.EngineHealth = 2000.0f;
            Helicopter.FuelTankHealth = 2000.0f;
            Helicopter.Health = 2000;
        }
        public void CleanUp()
        {
            if (Pilot && Helicopter && Pilot.IsAlive && Helicopter.IsAlive && Helicopter.IsInAir)
            {
                Pilot.HeliMission(Helicopter, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.Flee, Helicopter.TopSpeed, -1.0f, -1.0f, -1, 80, 0, 400.0f);
            }
            else if (Pilot && Pilot.IsDead)
            {
                Passengers.ForEach(x =>
                {
                    if (x && x.IsAlive) x.Tasks.ParachuteToTarget(World.GetNextPositionOnStreet(x.Position));
                });
            }
            else if (Helicopter && !Helicopter.IsInAir)
            {
                Helicopter.Occupants.ToList().ForEach(x =>
                {
                    if (x && x.IsAlive) x.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                });
            }
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
                if (ped)
                {
                    Functions.SetCopAsBusy(ped, false);
                    ped.Dismiss();
                }
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
