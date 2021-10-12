using System;
using System.Linq;
using Rage;
using Rage.Native;
using System.Diagnostics;
using System.Collections.Generic;
using BarbarianCall.Extensions;

namespace BarbarianCall.SupportUnit
{
    public class CargobobServices
    {
        public CargobobServices(Vehicle target)
        {
            if (!CheckVehicle(target)) return;
            TargetVehicle = target;
            Start();
        }
        public Vehicle Cargobob { get; private set; }
        public Vehicle TargetVehicle { get; private set; }
        public Ped Pilot { get; private set; }
        public Blip Blip { get; private set; }

        private readonly Model[] cargobobModels = { 0xFCFCB68B, 0x60A7EA10 };
        private readonly Model[] pilotModels =
        {
            0xE75B4B1C,
            0xAB300C07,
            0xF63DE8E1,
            0x864ED68E,
        };
        internal static List<Vehicle> queueVehicle = new();
        internal static List<Vehicle> vehicleTaken = new();
        Task gotoTask = null;
        Task moveCloser = null;
        Task gainAltitude = null;
        void Start()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    TargetVehicle.IsPersistent = true;
                    TargetVehicle.MakePersistent();
                    queueVehicle.Add(TargetVehicle);
                    Vector3 sp = GetSpawnPoint(TargetVehicle.Position);
                    Model cargobobModel = cargobobModels.GetRandomElement();
                    Model pilotModel = pilotModels.GetRandomElement();
                    cargobobModel.LoadAndWait();
                    pilotModel.LoadAndWait();
                    Cargobob = new Vehicle(cargobobModel, sp)
                    {
                        IsEngineOn = true,
                        IsInvincible = true,
                        Direction = TargetVehicle.Direction,
                        IsPersistent = true,
                    };
                    Blip = new Blip(Cargobob)
                    {
                        Sprite = (BlipSprite)481,
                        Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Green),
                    };
                    NativeFunction.Natives.SET_​HELI_​BLADES_​FULL_​SPEED(Cargobob);
                    Pilot = new Ped(pilotModel, sp, 0f)
                    {
                        IsPersistent = true,
                        BlockPermanentEvents = true,
                        Armor = 1500,
                    };
                    Pilot.WarpIntoVehicle(Cargobob, -1);
                    Pilot.MakeMissionPed(true);
                    $"{GetType().Name} | Tasking the pilot to follow target".ToLog();
                    gotoTask = Pilot.HeliMission(Cargobob, TargetVehicle, null, Vector3.Zero, MissionType.Follow, 50f, 0f, -1.0f, -1, 80, 0, -1.0f);
                    cargobobModel.Dismiss();
                    pilotModel.Dismiss();
                    while (true)
                    {
                        GameFiber.Yield();
                        if (gotoTask.Status == Rage.TaskStatus.Interrupted)
                        {
                            gotoTask = Pilot.HeliMission(Cargobob, TargetVehicle, null, Vector3.Zero, MissionType.Follow, 50f, 0f, -1.0f, -1, 80, 0, -1.0f);
                        }
                        if (Cargobob.DistanceToSquared2D(TargetVehicle) < 625f)
                        {
                            break;
                        }
                        if (!gotoTask.IsActive)
                        {
                            break;
                        }
                        if (!Cargobob || !Pilot || (Cargobob && Cargobob.IsDead) || (Pilot && Pilot.IsDead))
                        {
                            Cleanup();
                            return;
                        }
                    }
                    NativeFunction.Natives.CREATE_​PICK_​UP_​ROPE_​FOR_​CARGOBOB(Cargobob, 0);
                    NativeFunction.Natives.SET_PICKUP_ROPE_LENGTH_FOR_CARGOBOB(Cargobob, 3f, 3f, true);
                    float hookDistance = 8f;
                    RAGENativeUI.Elements.TextTimerBar bar = new("Distance", "");
                    RAGENativeUI.Elements.TextTimerBar bar2 = new("Accepted", "");
                    RAGENativeUI.Elements.TextTimerBar bar3 = new("Used Offset", "");
                    RAGENativeUI.Elements.TimerBarPool pool = new()
                    {
                        bar,
                        bar2,
                        bar3,
                    };
                    string[] poss = { "Front", "Rear", "Left", "Right", "Above", "RegularPos", "RearDim", "FrontDim" };
                    bool successHook = false;
                    moveCloser = Pilot.HeliMission(Cargobob, null, null, TargetVehicle.Position + TargetVehicle.ForwardVector * -2f, MissionType.GoTo, 20f, 0f, TargetVehicle.Heading, -1, -1, 0, -1.0f);
                    Game.LogTrivial("Moving closer to pick the veh");
                    Stopwatch taskStopwatch = Stopwatch.StartNew();
                    Stopwatch distanceMonitor = new();
                    Vector3 currentTargetPos = TargetVehicle.Position;
                    TargetVehicle.Model.GetDimensions(out var rearDimensions, out var frontDimensions);
                    Rage.Object hookProp = NativeFunction.Natives.GET_CLOSEST_OBJECT_OF_TYPE<Rage.Object>(Cargobob.Position, 10f, Game.GetHashKey("prop_v_hook_s"), false, false, false);
                    while (true)
                    {
                        GameFiber.Yield();
                        if (!Cargobob || !Pilot || (Cargobob && Cargobob.IsDead) || (Pilot && Pilot.IsDead) || !TargetVehicle)
                        {
                            Cleanup();
                            return;
                        }
                        if (taskStopwatch.ElapsedMilliseconds > 80000)
                        {
                            Game.LogTrivial("Timeout Waiting");
                            break;
                        }
                        Vehicle nich = NativeFunction.Natives.GET_VEHICLE_ATTACHED_TO_CARGOBOB<Vehicle>(Cargobob);
                        if (nich && nich == TargetVehicle)
                        {
                            Game.LogTrivial("Hook success");
                            successHook = true;
                            vehicleTaken.Add(TargetVehicle);
                            break;
                        }
                        Vector3 hookPos = NativeFunction.Natives.xCBDB9B923CACC92D<Vector3>(Cargobob);
                        Vector3[] positionToChecks = {  TargetVehicle.FrontPosition, TargetVehicle.RearPosition, TargetVehicle.LeftPosition, TargetVehicle.RightPosition, TargetVehicle.AbovePosition,
                                                    TargetVehicle.Position, TargetVehicle.Position + rearDimensions, TargetVehicle.Position + frontDimensions };
                        float[] distances = positionToChecks.Select(x => Vector3.DistanceSquared(x, hookPos)).ToArray();
                        float distanceSquared = distances.OrderBy(x => x).FirstOrDefault();
                        int index = Array.IndexOf(distances, distanceSquared);
                        bar.Text = distanceSquared.ToString();
                        if (distanceSquared < hookDistance || (hookProp && NativeFunction.Natives.IS_ENTITY_TOUCHING_ENTITY<bool>(hookProp, TargetVehicle)))
                        {
                            AttachToCargobob(hookProp && hookProp.DistanceTo(Cargobob) < 8f ? hookProp.Position : hookPos, true);
                        }
                        if (distanceSquared < 20f) bar.Highlight = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Green);
                        else if (distanceSquared > 1000f) bar.Highlight = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.Red);
                        else bar.Highlight = null;
                        if (distanceSquared < 30f && !distanceMonitor.IsRunning)
                        {
                            distanceMonitor.Start();
                        }
                        if (distanceMonitor.IsRunning && distanceMonitor.ElapsedMilliseconds > 7000)
                        {
                            hookDistance += 1.2585f;
                            distanceMonitor.Reset();
                        }
                        if (Vector3.DistanceSquared(currentTargetPos, TargetVehicle.Position) > 900f)
                        {
                            Pilot.Tasks.Clear();
                            Pilot.HeliMission(Cargobob, null, null, TargetVehicle.Position + new Vector3(0f, 0f, 60f), MissionType.GoTo, 10f, 0f, -1.0f, -1, 50, 0);
                            while (true)
                            {
                                GameFiber.Yield();
                                if (TargetVehicle.Speed < 2f || taskStopwatch.ElapsedMilliseconds > 80000) break;
                            }
                            Pilot.Tasks.Clear();
                            moveCloser = Pilot.HeliMission(Cargobob, null, null, TargetVehicle.Position + TargetVehicle.ForwardVector * -2f, MissionType.GoTo, 20f, 0f, TargetVehicle.Heading, -1, -1, 0, -1.0f);
                            currentTargetPos = TargetVehicle.Position;
                        }
                        bar2.Text = hookDistance.ToString();
                        bar3.Text = poss[index];
                        pool.Draw();
                    }
                    if (!successHook)
                    {
                        "Sorry, it seems our pilot can't reach the vehicle at this moment, feel free to contact us again to rearrange the pickup".
                        DisplayNotifWithLogo(title: "Mors Mutual Insurance", subtitle: "Cargobob Services", textureDict: "CHAR_MP_MORS_MUTUAL", textureName: "CHAR_MP_MORS_MUTUAL");
                        if (queueVehicle.Contains(TargetVehicle)) if (queueVehicle.Remove(TargetVehicle)) $"{GetType().Name} | Vehicle removed from queue".ToLog(); else $"{GetType().Name} | Failed to remove vehicle from queue".ToLog();
                    }
                    Cargobob.MakePersistent();
                    GameFiber.Wait(2500);
                    //TargetVehicle.SetPositionWithSnap(hookPos);
                    //NativeFunction.Natives.x9A665550F8DA349B(Cargobob, false);
                    gainAltitude = Pilot.HeliMission(Cargobob, null, null, Cargobob.Position.Around2D(25, 35) + new Vector3(0f, 0f, 100f), MissionType.GoTo, 80f, 8f, -1.0f, -1, 80, 0, 400.0f);
                    gainAltitude.WaitForCompletion(25000);
                    if (Blip) Blip.Delete();
                    "Pilot is fleeing".ToLog();
                    Pilot.HeliMission(Cargobob, null, Game.LocalPlayer.Character, Vector3.Zero, MissionType.Flee, Cargobob.TopSpeed, -1.0f, -1.0f, -1, 100, 0, -1.0f).WaitForCompletion(25000);
                    if (TargetVehicle)
                    {
                        TargetVehicle.IsCollisionProof = false;
                        TargetVehicle.IsCollisionEnabled = true;
                        TargetVehicle.Dismiss();
                    }
                    if (Cargobob) Cargobob.Dismiss();
                    if (Pilot) Pilot.Dismiss();
                    $"Fully dismissed".ToLog();
                    Cleanup();
                }
                catch (Exception e)
                {
                    Cleanup();
                    e.ToString().ToLog();
                }
            });
        }
        Vector3 GetSpawnPoint(Vector3 targetPos)
        {
            $"Finding spawnpoint for cargobob".ToLog();
            Vector3 v3 = targetPos.Around2D(800.0f) + Vector3.WorldUp * 450.0f;
            while (Vector3.Distance2D(Game.LocalPlayer.Character.Position, v3) < 400.0f)
            {
                v3 = targetPos.Around2D(800.0f) + Vector3.WorldUp * 450.0f;
                GameFiber.Yield();
            }
            return v3;
        }
        void Cleanup()
        {
            if (Blip) Blip.Delete();
            if (TargetVehicle) TargetVehicle.IsCollisionProof = false;
            if (TargetVehicle) TargetVehicle.IsCollisionEnabled = true;
            if (Cargobob) Cargobob.Dismiss();
            if (Pilot) Pilot.Dismiss();
            if (TargetVehicle) TargetVehicle.Dismiss();
            if (TargetVehicle && queueVehicle.Contains(TargetVehicle)) queueVehicle.Remove(TargetVehicle); 
        }
        bool CheckVehicle(Vehicle vehicle)
        {
            if (vehicle)
            {
                if (vehicle.Model.Hash == 0x2EA68690) return true;
                if (vehicle.Class == VehicleClass.Van) return true;
                if (queueVehicle.Contains(vehicle) || vehicleTaken.Contains(vehicle)) return false;
                if (!vehicle.IsCar) return false;
                if (vehicle.IsBig) return false;
            }
            return true;
        }
        Vector3 GetHookOffset(float heading)
        {
            return new Vector3((float)-Math.Sin(heading), (float)Math.Cos(heading), 0f);
        }
        void AttachToCargobob(Vector3 hookPosition, bool attach = true)
        {
            if (attach)
            {
                TargetVehicle.Heading = Cargobob.Heading;
                Vector3 v = GetHookOffset(Cargobob.Heading);
                Vector3 offset = Cargobob.Position - new Vector3(3.5f, 0f, 0f) + v * 1.6f;
                offset = new Vector3(hookPosition.X, hookPosition.Y, hookPosition.Z - 1f);
                NativeFunction.Natives.SET_ENTITY_COORDS_NO_OFFSET(TargetVehicle, offset, false, false, true);
                TargetVehicle.IsCollisionProof = true;
                TargetVehicle.IsCollisionEnabled = false;
                NativeFunction.Natives.xA1DD82F3CCF9A01E(Cargobob, TargetVehicle, -1, 0f, 0f, 0f);
                NativeFunction.Natives.STABILISE_ENTITY_ATTACHED_TO_HELI(Cargobob, TargetVehicle, -0.1f);
            }
            else
            {
                NativeFunction.Natives.xAF03011701811146(Cargobob, TargetVehicle);
                NativeFunction.Natives.DETACH_ENTITY(TargetVehicle, true, true);
            }
        }
        internal static List<Vehicle> GetVehicles()
        {
            return Game.LocalPlayer.Character.GetNearbyVehicles(16).Where(x => x.IsCar && !x.IsBig && !queueVehicle.Contains(x) && !vehicleTaken.Contains(x) && x.IsEmpty).OrderBy(x => x.DistanceToSquared(Game.LocalPlayer.Character)).ToList();
        }
    }
}