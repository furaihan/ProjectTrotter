using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using BarbarianCall.Types;
using Rage;
using BarbarianCall.Extensions;
using RAGENativeUI;

namespace BarbarianCall.SupportUnit
{
    public class Mechanic
    {
        private static readonly List<Vehicle> fixedVehs = new List<Vehicle>();
        public static List<Vehicle> VehicleFixed
        {
            get
            {
                return fixedVehs;
            }
        }
        private static readonly List<Vehicle> fixQueue = new List<Vehicle>();
        public static List<Vehicle> VehicleQueue
        {
            get
            {
                return fixQueue;
            }
        }
        public Ped MechanicPed { get; private set; }
        public Vehicle MechanicVehicle { get; private set; }
        public Vehicle VehicleToFix { get; private set; }
        public Blip Blip { get; private set; }
        public Blip BrokenVehicleBlip { get; private set; }
        /// <summary>
        /// a floating-point between 0 to 1, set to 0 to force the mechanic will never success to repair the vehicle, 
        /// set to 1 to make the vehicle always be repaired
        /// </summary>
        public float SuccessProbability { get; set; } = 0.825f;
        public bool DismissFixedVehicle { get; set; } = true;
        public float MinimumSpawnDistance { get; set; } = 150f;
        public float MaximumSpawnDistance { get; set; } = 350f;
        public float MaximumSpeed { get; set; } = 20f;
        public enum EMechanicState { Prepare, Driving, Arrive, GetCloseWithVehicle, Repairing, Finish, CleanUp }
        public EMechanicState State { get; private set; }
        public enum EMechanicStatus { Running, Error }
        public EMechanicStatus Status { get; private set; }
        public enum ERepairStatus { Repairing, Perfect, Imperfect, Failed }
        public ERepairStatus RepairStatus { get; private set; }
        private static List<Model> vehModels = new List<Model>() { "riata", "rebel", "bodhi2", "rebel2", "sadler" };
        private static List<Model> pedModels = new List<Model>() { "s_m_y_armymech_01", "s_m_y_xmech_01", "s_m_y_xmech_02", "mp_m_waremech_01", "s_m_y_xmech_02_mp", "s_m_m_gaffer_01" };
        public List<Model> VehicleModels { get { return vehModels; } set { vehModels = value; } }
        public List<Model> PedModels { get { return pedModels; } set { pedModels = value; } }
        private enum EMethodPassed { Prepare, Respond, Repair, CleanUp }
        private EMethodPassed MethodPassed;
        private Rage.Object ToolBox1;
        private Rage.Object ToolBox2;
        private Rage.Object Wrench;
        private AnimationTask JerryCan;
        private readonly AnimationDictionary jc = new AnimationDictionary("move_weapon@jerrycan@generic");
        private readonly RelationshipGroup MechanicRelationship;
        public Mechanic(Vehicle brokenVeh)
        {
            State = EMechanicState.Prepare;
            RepairStatus = ERepairStatus.Repairing;
            MethodPassed = EMethodPassed.Prepare;
            ReadIniFile();

            fixQueue.Add(brokenVeh);

            VehicleToFix = brokenVeh;
            VehicleToFix.IsPersistent = true;
            VehicleToFix.IsExplosionProof = true;
            VehicleToFix.LockStatus = VehicleLockStatus.Locked;

            MechanicRelationship = new RelationshipGroup("MECHANIC");
            MechanicRelationship.SetRelationshipWith(RelationshipGroup.Cop, Relationship.Companion);
            MechanicRelationship.SetRelationshipWith(RelationshipGroup.Medic, Relationship.Companion);
            MechanicRelationship.SetRelationshipWith(RelationshipGroup.Fireman, Relationship.Companion);
            MechanicRelationship.SetRelationshipWith(RelationshipGroup.Player, Relationship.Companion);
            RelationshipGroup.Cop.SetRelationshipWith(MechanicRelationship, Relationship.Companion);
            RelationshipGroup.Fireman.SetRelationshipWith(MechanicRelationship, Relationship.Companion);
            RelationshipGroup.Medic.SetRelationshipWith(MechanicRelationship, Relationship.Companion);
        }
        public void RespondToLocation()
        {
            if (VehicleToFix)
            {              
                GameFiber.StartNew(() =>
                {
                    try
                    {
                        var Position = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, MinimumSpawnDistance, MaximumSpawnDistance);
                        if (Position == Spawnpoint.Zero) Position.Position = World.GetNextPositionOnStreet(VehicleToFix.Position.Around(MinimumSpawnDistance, MaximumSpawnDistance));
                        float Heading = Position;

                        MechanicVehicle = new Vehicle(VehicleModels.GetRandomElement(), Position, Heading)
                        {
                            IsPersistent = true,
                            IsExplosionProof = true,
                            DirtLevel = 10f,
                            EngineHealth = 1000f,
                            CanTiresBurst = false,
                            TopSpeed = MaximumSpeed,
                            LicensePlateStyle = ((LicensePlateStyle[])Enum.GetValues(typeof(LicensePlateStyle))).GetRandomElement(),
                            IsStolen = false,
                            RadioStation = ((RadioStation[])Enum.GetValues(typeof(RadioStation))).GetRandomElement(),
                        };
                        MechanicVehicle.PlaceOnGroundProperly();
                        MechanicVehicle.RandomiseLicensePlate();

                        MechanicPed = new Ped(PedModels.GetRandomElement(), Position, Heading)
                        {
                            IsExplosionProof = true,
                            MaxHealth = 3000,
                            Health = 3000,
                            Armor = 2500,
                            CanBeTargetted = false,
                            RelationshipGroup = MechanicRelationship,
                        };
                        MechanicPed.MakeMissionPed(true);
                        MechanicPed.WarpIntoVehicle(MechanicVehicle, -1);
                        LSPD_First_Response.Mod.API.Functions.SetPedCantBeArrestedByPlayer(MechanicPed, true);

                        Blip = MechanicPed.AttachBlip();
                        Blip.SetBlipSprite(446);
                        Blip.Color = HudColor.NetPlayer26.GetColor();

                        BrokenVehicleBlip = VehicleToFix.AttachBlip();
                        BrokenVehicleBlip.Sprite = BlipSprite.GetawayCar;
                        BrokenVehicleBlip.Color = Color.LavenderBlush;
                        MethodPassed = EMethodPassed.Respond;
                        MechanicPed.DisplayNotificationsWithPedHeadshot("~g~En Route",
                            $"~h~Mechanic engineer~h~~s~ is ~g~en route~s~ to ~y~repair~s~ ~c~{VehicleToFix.GetVehicleDisplayName()}~s~. Please be ~g~patient",
                            "~y~Mechanic Engineer");
                        Status = EMechanicStatus.Running;
                        Spawnpoint drivePosition = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, 25, 35);
                        if (drivePosition == Spawnpoint.Zero) drivePosition.Position = World.GetNextPositionOnStreet(VehicleToFix.GetOffsetPosition(Vector3.RelativeFront * 15f).Around(5, 25));
                        State = EMechanicState.Driving;
                        if (VehicleToFix)
                            VehicleToFix.LockStatus = VehicleLockStatus.Locked;
                        if (MechanicPed)
                            MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraight).WaitForCompletion(500);
                        if (Game.LocalPlayer.Character.IsAlive) Game.LocalPlayer.Character.ToggleMobilePhone();
                        Task task = MechanicPed.Tasks.DriveToPosition(drivePosition, MaximumSpeed, VehicleDrivingFlags.FollowTraffic
                            | VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.YieldToCrossingPedestrians);
                        int slow = 0;
                        bool warped = false;
                        bool findRoadSide = false;
                        TimeSpan allowWarpTimer = new TimeSpan(0, 0, 120);
                        Stopwatch driveSW = Stopwatch.StartNew();
                        while (task.IsActive)
                        {
                            GameFiber.Yield(); if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                            if (MechanicVehicle && MechanicVehicle.Speed < 2.5f && !MechanicVehicle.IsStoppedAtTrafficLights) slow++;
                            else if (MechanicVehicle && VehicleToFix &&
                            VehicleToFix.Position.DistanceTo(MechanicVehicle.Position) > 500f && MechanicVehicle.TravelDistanceTo(VehicleToFix.Position) > 1250f) slow++;
                            else if (MechanicVehicle && !MechanicVehicle.IsOnAllWheels) slow++;
                            if (slow == 80 && MechanicVehicle.DistanceTo(VehicleToFix) > 200f)
                            {
                                Spawnpoint changePos = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, 150, 200);
                                if (changePos == Spawnpoint.Zero) changePos.Position = World.GetNextPositionOnStreet(VehicleToFix.Position.Around(150, 250));
                                if (MechanicVehicle)
                                    MechanicVehicle.Position = changePos;
                                if (MechanicVehicle)
                                    MechanicVehicle.Heading = changePos;
                            }
                            if (MechanicVehicle && VehicleToFix && slow == 160 && MechanicVehicle.DistanceTo(VehicleToFix) > 100f)
                            {
                                GameFiber.StartNew(() =>
                                {
                                    Game.DisplayHelp($"Mechanic engineering is ~o~not moving~s~ or ~o~too far away~s~, Press "
                                        + $"{Peralatan.FormatKeyBinding(Keys.None, Keys.Back)} ~y~[Backspace]~s~ to force spawn nearby", 7500);
                                    Stopwatch helpSW = Stopwatch.StartNew();
                                    while (State == EMechanicState.Driving)
                                    {
                                        GameFiber.Yield();
                                        if (helpSW.ElapsedMilliseconds > 18000L)
                                        {
                                            Game.DisplayHelp($"Mechanic engineering is ~o~not moving~s~ or ~o~too far away~s~, Press and hold"
                                                + $"{Peralatan.FormatKeyBinding(Keys.None, Keys.Back)} ~y~[Backspace]~s~ to force spawn nearby", 7500);
                                            helpSW.Restart();
                                        }
                                        if (Game.IsKeyDown(Keys.Back))
                                        {
                                            GameFiber.Sleep(300);
                                            if (Game.IsKeyDownRightNow(Keys.Back))
                                            {
                                                GameFiber.Sleep(1650);
                                                if (Game.IsKeyDownRightNow(Keys.Back)) warped = true;
                                            }
                                            else Game.DisplayHelp($"To warp mechanic service, press and hold {Peralatan.FormatKeyBinding(Keys.None, Keys.Back)} ~y~[Backspace]~s~ for 2 seconds");
                                        }
                                    }
                                });
                            }
                            if (warped)
                            {
                                Spawnpoint warpingCloser = SpawnManager.GetRoadSideSpawnPoint(VehicleToFix.Position);
                                if (warpingCloser == Spawnpoint.Zero) warpingCloser = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, 5, 25);
                                if (warpingCloser == Spawnpoint.Zero) warpingCloser.Position = World.GetNextPositionOnStreet(VehicleToFix.Position);
                                if (MechanicVehicle) MechanicVehicle.Position = warpingCloser;
                                if (MechanicVehicle) MechanicVehicle.Heading = warpingCloser;
                                Game.DisplayHelp("~y~Warping~s~ ~g~Mechanic~s~ ~y~Closer...", 5000);
                                break;
                            }
                            if (MechanicVehicle && VehicleToFix && MechanicVehicle.DistanceTo(VehicleToFix) < 40f && !MechanicVehicle.IsStoppedAtTrafficLights && !findRoadSide)
                            {
                                findRoadSide = true;
                                Vector3 front = MechanicVehicle.GetOffsetPositionFront(35f);
                                Spawnpoint roadSide = SpawnManager.GetRoadSideSpawnPoint(front, MechanicVehicle.Heading);
                                if (roadSide == Spawnpoint.Zero) roadSide = SpawnManager.GetRoadSideSpawnPointFavored(MechanicVehicle, 35);
                                if (roadSide != Spawnpoint.Zero)
                                {
                                    if (MechanicPed) MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(800);
                                    if (MechanicPed) MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.ReverseStraight).WaitForCompletion(1500);
                                    if (MechanicPed)
                                    {
                                        task = MechanicPed.ParkVehicle(roadSide, roadSide);
                                        task.WaitForCompletion(20000);
                                        if (task.IsActive)
                                        {
                                            if (MechanicVehicle) MechanicVehicle.Position = roadSide;
                                            if (MechanicVehicle) MechanicVehicle.Heading = roadSide;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (MechanicPed && MechanicVehicle) MechanicPed.Tasks.PerformDrivingManeuver(MechanicVehicle ,VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(250);
                                    if (MechanicPed && MechanicVehicle) LSPD_First_Response.Mod.API.Functions.StartTaskParkVehicle(MechanicPed, 20000);
                                    if (MechanicPed && MechanicVehicle)
                                    {
                                        GameFiber.WaitWhile(()=> MechanicVehicle.Speed > 2f, 20000);
                                        break;
                                    }
                                }
                            }
                            if (driveSW.ElapsedMilliseconds > allowWarpTimer.TotalMilliseconds) slow = 160;
                            if (MechanicVehicle)
                                MechanicVehicle.Repair();
                            //task = MechanicPed.Tasks.DriveToPosition(drivePosition, 21f, VehicleDrivingFlags.FollowTraffic
                            //| VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.YieldToCrossingPedestrians);
                        }
                        if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                        State = EMechanicState.Arrive;
                        if (MechanicPed) MechanicPed.Tasks.LeaveVehicle(MechanicVehicle, LeaveVehicleFlags.None).WaitForCompletion(5000);
                        if (MechanicPed && !MechanicPed.IsOnFoot) MechanicPed.Tasks.LeaveVehicle(MechanicVehicle, LeaveVehicleFlags.WarpOut);
                        if (MechanicPed) MechanicPed.PlayAmbientSpeech(Speech.GENERIC_HI);
                        if (MechanicPed && MechanicVehicle) MechanicPed.Tasks.FollowNavigationMeshToPosition(MechanicVehicle.RearPosition, MechanicVehicle.Heading, 1.2f).WaitForCompletion(12000);
                        if (MechanicPed) MechanicPed.Tasks.PlayAnimation("rcmepsilonism8", "bag_handler_close_trunk_walk_left", 4f, AnimationFlags.UpperBodyOnly | AnimationFlags.NoSound1 | AnimationFlags.SecondaryTask);
                        ToolBox1 = new Rage.Object("ch_prop_toolbox_01a", Vector3.Zero);
                        if (ToolBox1) ToolBox1.MakePersistent();
                        if (ToolBox1) ToolBox1.IsCollisionEnabled = true;
                        int bone = MechanicPed.GetBoneIndex(PedBoneId.RightPhHand);
                        GameFiber.Wait(2000);
                        if (ToolBox1) ToolBox1.AttachTo(MechanicPed, bone, new Vector3(0.24f, -0.050f, -0.050f), new Rotator(-99.9999924f, -100.000015f, -1.90734863e-06f));
                        GameFiber.Wait(1000);
                        if (MechanicPed) MechanicPed.Tasks.ClearImmediately();
                        if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                        State = EMechanicState.GetCloseWithVehicle;
                        Vector3 repairPos = VehicleToFix.FrontPosition + (VehicleToFix.ForwardVector * 0.25f);
                        jc.LoadAndWait();
                        VehicleToFix.IsPositionFrozen = true;
                        var runtask = MechanicPed.Tasks.FollowNavigationMeshToPosition(repairPos, repairPos.GetHeadingTowards(VehicleToFix), 10f);
                        runtask.WaitForCompletion(100000);
                        if (MechanicPed.DistanceTo(repairPos) > 15f || runtask.Status == TaskStatus.Interrupted)
                        {
                            MechanicPed.Position = repairPos;
                            MechanicPed.Heading = MechanicPed.GetHeadingTowards(VehicleToFix);
                        }                      
                        if (MechanicPed)
                            JerryCan = MechanicPed.Tasks.PlayAnimation(jc, "run", 4.0f, AnimationFlags.UpperBodyOnly | AnimationFlags.SecondaryTask | AnimationFlags.Loop);                                                
                        if (VehicleToFix)
                            RepairVehicle();
                        else throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                    }
                    catch (Exception e)
                    {
                        Game.DisplayNotification("~y~Mechanic backup~s~ ~r~error~s~, please send your ~o~log~s~ to ~g~author~s~");
                        e.ToString().ToLog();
                        $"Mechanic state: {State}".ToLog();
                        Status = EMechanicStatus.Error;
                        if (MethodPassed == EMethodPassed.Respond)
                            CleanUp();
                    }
                }, "[BarbarianCall] Mechanic Service Fiber");
            }
        }
        private void RepairVehicle()
        {
            MethodPassed = EMethodPassed.Repair;
            try
            {
                MechanicPed.Tasks.Clear();
                Game.DisplaySubtitle("~b~Mechanic~s~: " + handleDialogue.GetRandomElement());
                MechanicPed.Tasks.AchieveHeading(VehicleToFix.Heading - 180f).WaitForCompletion(1000);
                State = EMechanicState.Repairing;
                JerryCan.WaitForCompletion(12);
                if (ToolBox1)
                {
                    ToolBox1.Detach();
                    ToolBox1.Position = Vector3.Zero;
                    ToolBox1.MakePersistent();
                }
                ToolBox2 = new Rage.Object("ch_prop_toolbox_01b", MechanicPed.GetOffsetPositionRight(0.5f))
                {
                    Rotation = new Rotator(-0.0100011788f, 2.0844082e-10f, -99.9999542f),
                    IsPersistent = true,
                    IsPositionFrozen = true,
                    IsCollisionEnabled = false
                };
                ToolBox2.Heading = ToolBox2.GetHeadingTowards(MechanicPed);
                //"after make tob2".ToLog();
                float? zPosBox = World.GetGroundZ(ToolBox2.Position, true, true);
                ToolBox2.SetPositionZ(zPosBox ?? ToolBox2.Position.Z);
                //"after set Z".ToLog();
                MechanicPed.Tasks.AchieveHeading(MechanicPed.GetHeadingTowards(ToolBox2)).WaitForCompletion(2000);
                //"after Achieve heading".ToLog();
                MechanicPed.Tasks.PlayAnimation("pickup_object", "pickup_low", 4.0f, AnimationFlags.SecondaryTask);
                GameFiber.Wait(1250);
                Wrench = new Rage.Object("prop_tool_wrench", Vector3.Zero);
                Wrench.AttachTo(MechanicPed, MechanicPed.GetBoneIndex(PedBoneId.RightPhHand), Vector3.Zero, Rotator.Zero);
                GameFiber.Wait(2000);
                MechanicPed.Tasks.ClearImmediately();
                MechanicPed.Tasks.AchieveHeading(MechanicPed.GetHeadingTowards(VehicleToFix)).WaitForCompletion(1500);
                if (VehicleToFix.HasBone("bonnet") && VehicleToFix.Doors[4].IsValid())
                {
                    MechanicPed.Tasks.PlayAnimation("rcmnigel3_trunk", "out_trunk_trevor", 3f, AnimationFlags.UpperBodyOnly | AnimationFlags.SecondaryTask);
                    GameFiber.Wait(800);
                    OpenHood(VehicleToFix, true);
                    GameFiber.Wait(1650);
                }
                MechanicPed.Tasks.ClearImmediately();
                AnimationTask taskRepair = MechanicPed.Tasks.PlayAnimation("mini@repair", "fixing_a_ped", 3f, AnimationFlags.None);
                while (taskRepair.IsActive)
                {
                    GameFiber.Yield(); if (!VehicleToFix) throw new ArgumentException("Vehicle does not exist");
                }
                string selectedTalk;
                if (Peralatan.Random.NextDouble() < SuccessProbability)
                {
                    VehicleToFix.Repair();
                    VehicleToFix.Health = VehicleToFix.MaxHealth;
                    VehicleToFix.EngineHealth = 1000f;
                    VehicleToFix.IsDriveable = true;
                    VehicleToFix.IsExplosionProof = false;
                    VehicleToFix.FuelTankHealth = 1000f;
                    VehicleToFix.Wash();
                    OpenHood(VehicleToFix, false);
                    MechanicPed.Tasks.ClearImmediately();
                    selectedTalk = "~b~Mechanic~s~: " + perfectDialogue.GetRandomElement();
                    RepairStatus = ERepairStatus.Perfect;
                }
                else
                {
                    OpenHood(VehicleToFix, false);
                    selectedTalk = "~b~Mechanic~s~: " + failedDialogue.GetRandomElement();             
                    RepairStatus = ERepairStatus.Failed;
                }
                VehicleToFix.IsPositionFrozen = false;
                if (Wrench) Wrench.Delete();
                GameFiber.Wait(75);
                if (ToolBox2) ToolBox2.Delete();
                GameFiber.Wait(75);
                ToolBox1.AttachTo(MechanicPed, MechanicPed.GetBoneIndex(PedBoneId.RightPhHand),
                    new Vector3(0.25f, -0.06f, -0.04f), new Rotator(-90.0299988f, -79.9999924f, -9.99999905f));
                GameFiber.Wait(75);
                if (Game.LocalPlayer.Character.Position.DistanceTo(MechanicPed.Position) < 60f)
                    MechanicPed.Tasks.FollowNavigationMeshToPosition(Game.LocalPlayer.Character.Position + Game.LocalPlayer.Character.ForwardVector * 1.1125f,
                        Game.LocalPlayer.Character.Heading - 180.0f, 10.0f).WaitForCompletion(7500);
                Game.DisplaySubtitle(selectedTalk, 1000000);
                State = EMechanicState.Finish;
                VehicleToFix.LockStatus = VehicleLockStatus.Unlocked;
                Game.DisplayHelp($"Press {Peralatan.FormatKeyBinding(Keys.None, Keys.NumPad0)} to dissmiss the mechanic", 10000);
                GameFiber.SleepUntil(() => Game.IsKeyDown(Keys.NumPad0), 10000);
                Game.DisplaySubtitle("");
                Game.HideHelp();
                CleanUp();
            }
            catch (Exception e)
            {
                Game.DisplayNotification("~y~Mechanic backup~s~ ~r~error~s~, please send your ~o~log~s~ to ~g~author~s~");
                e.ToString().ToLog();
                Status = EMechanicStatus.Error;
                if (MethodPassed == EMethodPassed.Repair)
                    CleanUp();
            }
        }
        private void CleanUp()
        {
            MethodPassed = EMethodPassed.CleanUp;
            if (fixQueue.Contains(VehicleToFix)) fixQueue.Remove(VehicleToFix);
            try
            {
                State = EMechanicState.CleanUp;
                if (MechanicVehicle && MechanicPed)
                {
                    jc.LoadAndWait();
                    JerryCan = MechanicPed.Tasks.PlayAnimation(jc, "run", 4.0f, AnimationFlags.Loop | AnimationFlags.SecondaryTask | AnimationFlags.UpperBodyOnly);
                    MechanicPed.Tasks.FollowNavigationMeshToPosition(MechanicVehicle.GetOffsetPosition(Vector3.RelativeLeft * 1.8f), MechanicVehicle.Heading, 10f).WaitForCompletion(10000);
                    if (MechanicVehicle && MechanicPed)
                        if (MechanicPed.Position.DistanceTo(MechanicVehicle.GetOffsetPosition(Vector3.RelativeLeft * 1.8f)) > 8f)
                            MechanicPed.SetPositionWithSnap(MechanicVehicle.GetOffsetPosition(Vector3.RelativeLeft * 1.8f));
                    if (MechanicVehicle && MechanicPed)
                        MechanicPed.Tasks.EnterVehicle(MechanicVehicle, -1).WaitForCompletion(5000);
                    if (MechanicVehicle && MechanicPed)
                        if (!MechanicPed.IsInVehicle(MechanicVehicle, false))
                            MechanicPed.WarpIntoVehicle(MechanicVehicle, -1);
                    if (MechanicVehicle && MechanicPed)
                    {
                        MechanicPed.Tasks.ClearSecondary();
                    }
                }
            }
            catch (Exception e)
            {
                Game.DisplayNotification("~r~Vehicle cleanup error, please send your log file to author");
                e.ToString().ToLog();
            }
            finally
            {
                if (VehicleToFix)
                {
                    if (RepairStatus == ERepairStatus.Perfect || RepairStatus == ERepairStatus.Imperfect) fixedVehs.Add(VehicleToFix);
                    VehicleToFix.IsExplosionProof = false;
                    if (DismissFixedVehicle) VehicleToFix.Dismiss();
                }
                if (MechanicVehicle && MechanicPed) { MechanicVehicle.Dismiss(); MechanicPed.Dismiss(); }
                if (Blip) Blip.Delete();
                if (BrokenVehicleBlip) BrokenVehicleBlip.Delete();
                if (ToolBox1) ToolBox1.Delete();
                if (ToolBox2) ToolBox2.Delete();
                if (Wrench) Wrench.Delete();
                if (jc.IsLoaded) jc.Dismiss();
            }
        }
        private void OpenHood(Vehicle vehicle, bool open, bool instantly = false)
        {
            var hood = vehicle.Doors[4];
            if (vehicle.HasBone("bonnet") && hood.IsValid())
            {
                if (open)
                {
                    hood.Open(instantly);
                }
                else { hood.Close(instantly); }
            }
        }
        private readonly List<string> handleDialogue = new List<string>()
        {
            "Hello, we will handle this vehicle",
            "Hello, thanks for calling, we will attempt to repair this vehicle",
            "Hello, looks like this vehicle needs engine repair",
            "Hello, we will take care of this vehicle, you can do your job now",
            "Hello, we will handle this vehicle properly, you can go now",
            "Hello, it seems this vehicle is damaged very bad, we will attempting to repair it carefully",
            "Hello, let see what we got here"
        };
        private readonly List<string> perfectDialogue = new List<string>()
        {
            "This vehicle has been repaired successfully",
            "Sir, luckily the damage is not too bad, this vehicle is perfectly fine right now",
            "Oh Yuck, what a horrible smell. but fortunately this vehicle is not a problem for me",
            "Oh man, this was not too hard. You can drive this vehicle smoothly now",
            "Woah, this vehicle damage is pretty severe, but thats not a problem for a professional mechanic like me",
            "What a damage, You're lucky to have a mechanic like me",
            "Perfectly Done!!, lucky you entrusted this to me",
            "The damage is pretty bad, but my mechanic skill can handle this perfectly"
        };
        private readonly List<string> failedDialogue = new List<string>()
        {
            "Sorry, this vehicle is too hard to repair, i cant do anything",
            "I'm sorry, i tried my best but this vehicle is badly damaged",
            "How stupid of me, i can't repair this vehicle, maybe you can call a tow truck. I do really sorry for what i have done",
            "Sir my sincerest apologies, i can't repair this vehicle at all. i'm really sorry",
            "I'm sorry, this vehicle damage is too severe, i can't repair it now. I sincerely apologize",
            "Aah sh*t, my skills is very bad, i can't repair this vehicle, maybe you can call a tow truck",
            "Sir, I apologize from the deepest of my heart because this vehicle seems have severe damage and i can't fix it"
        };
        internal void ReadIniFile()
        {
            try
            {
                var path = @"Plugins\LSPDFR\BarbarianCall\SupportUnit\Mechanic.ini";
                InitializationFile mechanicIni = new InitializationFile(path, true);
                var stringPeds = mechanicIni.ReadString("Mechanic", "PedModels", "s_m_y_armymech_01 s_m_y_xmech_01 s_m_y_xmech_02 mp_m_waremech_01 s_m_y_xmech_02_mp").ToLower();
                var modelPeds = stringPeds.Split(' ').Select(s => new Model(s)).Where(m => m.IsValid).ToList();
                var stringVehs = mechanicIni.ReadString("Mechanic", "VehicleModels", "riata rebel bodhi2 rebel2 sadler").ToLower();
                var modelVehs = stringVehs.Split(' ').Select(s => new Model(s)).Where(m => m.IsValid).ToList();
                float maxDis = mechanicIni.ReadSingle("Mechanic", "SpawnMaximumDistance", 350);
                float minDis = mechanicIni.ReadSingle("Mechanic", "SpawnMinimumDistance", 150);
                float speed = mechanicIni.ReadSingle("Mechanic", "MaximumSpeed", 20);
                float prob = mechanicIni.ReadSingle("Mechanic", "SuccessProbability", 0.825f);
#if DEBUG
                stringPeds.ToLog();
                 stringVehs.ToLog();
                $"MaximumDistance: {maxDis}. MinimumDistance: {minDis}. Speed: {speed}. Probability: {prob}".ToLog();
                $"PL: {modelPeds.Count}. VL:{modelVehs.Count}".ToLog();
#endif
                if (modelPeds.Count > 0) PedModels = modelPeds;
                if (modelVehs.Count > 0) VehicleModels = modelVehs;
                MaximumSpawnDistance = maxDis;
                MinimumSpawnDistance = minDis;
                MaximumSpeed = speed;
                SuccessProbability = prob;
                //AlwaysWork = neverFail;
            }           
            catch (Exception e)
            {
                "Read INI File error".ToString();
                e.ToString().ToLog();
                NetExtension.SendError(e);
            }
        }
    }
}
