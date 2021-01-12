using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using BarbarianCall.Types;
using Rage;

namespace BarbarianCall.SupportUnit
{
    internal class Mechanic
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
        public bool AlwaysWork { get; set; } = false;
        public bool DismissFixedVehicle { get; set; } = true;
        public enum MechanicState { Prepare, Driving, Arrive, GetCloseWithVehicle, Repairing, Finish, CleanUp }
        public MechanicState State { get; private set; }
        public enum MechanicStatus { Running, Error }
        public MechanicStatus Status { get; private set; }
        public enum ERepairStatus { Unrepaired, Perfect, Imperfect, Failed }
        public ERepairStatus RepairStatus { get; private set; }
        private readonly string[] modelVeh = { "riata", "rebel", "bodhi2", "rebel2", "sadler" };
        private readonly string[] modelPed = { "s_m_y_armymech_01", "s_m_y_xmech_01", "s_m_y_xmech_02", "mp_m_waremech_01", "s_m_y_xmech_02_mp", "s_m_m_gaffer_01" };
        private enum EMethodPassed { Prepare, Respond, Repair, CleanUp }
        private EMethodPassed MethodPassed;
        private Rage.Object ToolBox1;
        private Rage.Object ToolBox2;
        private Rage.Object Wrench;
        private AnimationTask JerryCan;
        private readonly AnimationDictionary jc = new AnimationDictionary("move_weapon@jerrycan@generic");
        public Mechanic(Vehicle brokenVehs)
        {
            State = MechanicState.Prepare;
            RepairStatus = ERepairStatus.Unrepaired;
            MethodPassed = EMethodPassed.Prepare;

            fixQueue.Add(brokenVehs);

            VehicleToFix = brokenVehs;
            VehicleToFix.IsPersistent = true;
            VehicleToFix.IsExplosionProof = true;
            VehicleToFix.LockStatus = VehicleLockStatus.Locked;

            var Position = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, 250, 300);
            var Heading = Position;

            MechanicVehicle = new Vehicle(modelVeh.GetRandomElement(), Position, Heading)
            {
                IsPersistent = true,
                IsExplosionProof = true,
                DirtLevel = 10f,
                EngineHealth = 1000f,
                CanTiresBurst = false
            };
            MechanicVehicle.PlaceOnGroundProperly();
            MechanicVehicle.RandomiseLicensePlate();

            MechanicPed = new Ped(modelPed.GetRandomElement(), Position, Heading)
            {
                IsExplosionProof = true,
                MaxHealth = 3000,
                Health = 3000,
                Armor = 2500,
                CanBeTargetted = false,
            };
            MechanicPed.MakeMissionPed(true);
            MechanicPed.WarpIntoVehicle(MechanicVehicle, -1);

            Blip = MechanicPed.AttachBlip();
            Blip.Color = Color.PaleVioletRed;
            Blip.Scale = 0.8f;
            Blip.SetBlipSprite(446);

            BrokenVehicleBlip = VehicleToFix.AttachBlip();
            BrokenVehicleBlip.Sprite = BlipSprite.GetawayCar;
            BrokenVehicleBlip.Color = Color.LavenderBlush;
        }
        public void RespondToLocation()
        {
            if (VehicleToFix)
            {
                MethodPassed = EMethodPassed.Respond;
                MechanicPed.DisplayNotificationsWithPedHeadshot("~g~En Route",
                    $"~h~Mechanic engineer~s~ is ~g~en route~s~ to ~y~repair~s~ ~c~{VehicleToFix.GetVehicleDisplayName()}~s~. Please be ~g~patient",
                    "~g~Mechanic Engineer");
                Status = MechanicStatus.Running;
                GameFiber.StartNew(() =>
                {
                    try
                    {
                        SpawnPoint drivePosition = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.GetOffsetPosition(Vector3.RelativeFront * 15f), 5, 25);
                        if (drivePosition == SpawnPoint.Zero) drivePosition.Position = World.GetNextPositionOnStreet(VehicleToFix.GetOffsetPosition(Vector3.RelativeFront * 15f).Around(5, 25));
                        State = MechanicState.Driving;
                        VehicleToFix.LockStatus = VehicleLockStatus.Locked;
                        MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraight).WaitForCompletion(500);
                        if (Game.LocalPlayer.Character.IsAlive) Game.LocalPlayer.Character.ToggleMobilePhone();
                        Rage.Task task = MechanicPed.Tasks.DriveToPosition(drivePosition, 21f, VehicleDrivingFlags.FollowTraffic
                            | VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.YieldToCrossingPedestrians);
                        int slow = 0;
                        bool warped = false;
                        bool findRoadSide = false;
                        TimeSpan allowWarpTimer = new TimeSpan(0, 0, 120);
                        Stopwatch driveSW = Stopwatch.StartNew();
                        while (true)
                        {
                            GameFiber.Yield(); if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                            if (MechanicVehicle.Speed < 2.5f && !MechanicVehicle.IsStoppedAtTrafficLights) slow++;
                            else if (VehicleToFix.Position.DistanceTo(MechanicVehicle.Position) > 500f && MechanicVehicle.TravelDistanceTo(VehicleToFix.Position) > 1250f) slow++;
                            else if (!MechanicVehicle.IsOnAllWheels) slow++;
                            if (slow == 80 && MechanicVehicle.DistanceTo(VehicleToFix) > 200f)
                            {
                                SpawnPoint changePos = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, 150, 200);
                                if (changePos == SpawnPoint.Zero) changePos.Position = World.GetNextPositionOnStreet(VehicleToFix.Position.Around(150, 250));
                                MechanicVehicle.Position = changePos;
                                MechanicVehicle.Heading = changePos;
                            }
                            if (slow == 160 && MechanicVehicle.DistanceTo(VehicleToFix) > 100f)
                            {
                                GameFiber.StartNew(() =>
                                {
                                    Game.DisplayHelp($"Mechanic engineering is ~o~not moving~s~ or ~o~too far away~s~, Press " 
                                        + $"{Peralatan.FormatKeyBinding(Keys.None, Keys.Back)} ~y~[Backspace]~s~ to force spawn nearby", 7500);
                                    Stopwatch helpSW = Stopwatch.StartNew();
                                    while (State == MechanicState.Driving)
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
                                SpawnPoint warpingCloser = SpawnManager.GetRoadSideSpawnPoint(VehicleToFix.Position);
                                if (warpingCloser == SpawnPoint.Zero) warpingCloser = SpawnManager.GetVehicleSpawnPoint(VehicleToFix.Position, 5, 25);
                                if (warpingCloser == SpawnPoint.Zero) warpingCloser.Position = World.GetNextPositionOnStreet(VehicleToFix.Position);
                                MechanicVehicle.Position = warpingCloser;
                                MechanicVehicle.Heading = warpingCloser;
                                Game.DisplayHelp("~y~Warping~s~ ~g~Mechanic~s~ ~y~Closer...", 5000);
                                break;
                            }
                            if (MechanicVehicle.DistanceTo(VehicleToFix) < 38f && !MechanicVehicle.IsStoppedAtTrafficLights && !findRoadSide)
                            {
                                findRoadSide = true;
                                Vector3 front = MechanicVehicle.FrontPosition + MechanicVehicle.ForwardVector * 35f;
                                SpawnPoint roadSide = SpawnManager.GetRoadSideSpawnPoint(front);
                                if (roadSide != SpawnPoint.Zero)
                                {
                                    MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(800);
                                    MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.ReverseStraight).WaitForCompletion(1500);
                                    task = MechanicPed.Tasks.ParkVehicle(roadSide, roadSide);
                                    task.WaitForCompletion(20000);
                                    if (task.IsActive)
                                    {
                                        MechanicVehicle.Position = roadSide;
                                        MechanicVehicle.Heading = roadSide;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (MechanicPed) MechanicPed.Tasks.PerformDrivingManeuver(VehicleManeuver.GoForwardStraightBraking).WaitForCompletion(250);
                                    if (MechanicVehicle) MechanicVehicle.TopSpeed = 12f;
                                }
                            }
                            if (driveSW.ElapsedMilliseconds > allowWarpTimer.TotalMilliseconds) slow = 160;
                            if (!task.IsActive) break;
                            MechanicVehicle.Repair();
                            //task = MechanicPed.Tasks.DriveToPosition(drivePosition, 21f, VehicleDrivingFlags.FollowTraffic
                            //| VehicleDrivingFlags.DriveAroundVehicles | VehicleDrivingFlags.DriveAroundObjects | VehicleDrivingFlags.AllowMedianCrossing | VehicleDrivingFlags.YieldToCrossingPedestrians);
                        }
                        if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                        State = MechanicState.Arrive;
                        MechanicPed.Tasks.LeaveVehicle(LeaveVehicleFlags.None).WaitForCompletion(2000);
                        MechanicPed.PlayAmbientSpeech(Speech.GENERIC_HI);
                        MechanicPed.Tasks.FollowNavigationMeshToPosition(MechanicVehicle.RearPosition, MechanicVehicle.Heading, 1.2f).WaitForCompletion(12000);
                        MechanicPed.Tasks.PlayAnimation("rcmepsilonism8", "bag_handler_close_trunk_walk_left", 4f, AnimationFlags.UpperBodyOnly | AnimationFlags.NoSound1 | AnimationFlags.SecondaryTask);
                        ToolBox1 = new Rage.Object("ch_prop_toolbox_01a", Vector3.Zero);
                        ToolBox1.MakePersistent();
                        ToolBox1.IsCollisionEnabled = true;
                        int bone = MechanicPed.GetBoneIndex(PedBoneId.RightPhHand);
                        GameFiber.Wait(2000);
                        ToolBox1.AttachTo(MechanicPed, bone, new Vector3(0.240000024f, -0.049999997f, -0.049999997f), new Rotator(-99.9999924f, -100.000015f, -1.90734863e-06f));
                        GameFiber.Wait(1000);
                        MechanicPed.Tasks.ClearImmediately();
                        if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                        State = MechanicState.GetCloseWithVehicle;
                        VehicleToFix.Model.GetDimensions(out Vector3 backBottomLeft, out Vector3 frontTopRight);
                        //$"Dimension: {backBottomLeft} ==> {frontTopRight}".ToLog();
                        Vector3 repairPos = VehicleToFix.FrontPosition + (VehicleToFix.ForwardVector * 0.25f);
                        jc.LoadAndWait();
                        JerryCan = MechanicPed.Tasks.PlayAnimation(jc, "run", 4.0f, AnimationFlags.UpperBodyOnly | AnimationFlags.SecondaryTask | AnimationFlags.Loop);
                        Rage.Task task2 = MechanicPed.Tasks.FollowNavigationMeshToPosition(repairPos, VehicleToFix.Heading - 180f, 2.5f);
                        int runCount = 0;
                        var allowWarp = DateTime.UtcNow + new TimeSpan(0, 0, 20);
                        bool warpToVtf = false;
                        while (true)
                        {
                            GameFiber.Yield(); if (!VehicleToFix) throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                            runCount++;
                            if (task2.Status == TaskStatus.Interrupted)
                                task2 = MechanicPed.Tasks.FollowNavigationMeshToPosition(repairPos, VehicleToFix.Heading - 180f, 10f);
                            if (!task2.IsActive) break;
                            if (runCount > 1000)
                            {
                                MechanicPed.Tasks.ClearImmediately();
                                MechanicPed.Position = repairPos;
                                MechanicPed.Heading = VehicleToFix.Heading - 180f;
                                break;
                            }
                            if (DateTime.UtcNow >= allowWarp && Game.IsKeyDown(Keys.Back))
                            {
                                GameFiber.StartNew(() =>
                                {
                                    GameFiber.Wait(300);
                                    if (Game.IsKeyDownRightNow(Keys.Back))
                                    {
                                        GameFiber.Wait(1700);
                                        if (Game.IsKeyDownRightNow(Keys.Back)) warpToVtf = true;
                                    }
                                    else Game.DisplayHelp($"~y~Hold down {Peralatan.FormatKeyBinding(Keys.None, Keys.Back)}~y~ ~o~(BackSpace)~y~ key to warp the mechanic closer");
                                });
                            }
                            if (warpToVtf) runCount = 980;
                            //task2 = MechanicPed.Tasks.FollowNavigationMeshToPosition(repairPos, VehicleToFix.Heading - 180f, 2.5f);
                        }
                        if (VehicleToFix)
                            RepairVehicle();
                        else throw new Rage.Exceptions.InvalidHandleableException("Vehicle does not exist");
                    }
                    catch (Exception e)
                    {
                        Game.DisplayNotification("~y~Mechanic backup~s~ ~r~error~s~, please send your ~o~log~s~ to ~g~author~s~");
                        e.ToString().ToLog();
                        $"Mechanic state: {State}".ToLog();
                        Status = MechanicStatus.Error;
                        if (MethodPassed == EMethodPassed.Respond)
                            CleanUp();
                    }
                });
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
                State = MechanicState.Repairing;
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
                AnimationTask taskRepair = MechanicPed.Tasks.PlayAnimation(new AnimationDictionary("mini@repair"), "fixing_a_ped", 3f, AnimationFlags.None);
                while (taskRepair.IsActive)
                {
                    GameFiber.Yield(); if (!VehicleToFix) throw new ArgumentException("Vehicle does not exist");
                }
                int num = Peralatan.Random.Next(1, 100);
                if (AlwaysWork) num = Peralatan.Random.Next(1, 65);
                string selectedTalk;
                if (VehicleToFix.Health > VehicleToFix.MaxHealth - 30f && VehicleToFix.EngineHealth > 700f) num = 50;
                if (num <= 70)
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
                else if (num <= 90)
                {
                    VehicleToFix.Repair();
                    VehicleToFix.EngineHealth = Peralatan.Random.Next(350, 750);
                    VehicleToFix.FuelTankHealth = Peralatan.Random.Next(700, 950);
                    VehicleToFix.IsDriveable = true;
                    VehicleToFix.IsExplosionProof = false;
                    VehicleToFix.FuelTankHealth = Peralatan.Random.Next(710, 980);
                    VehicleToFix.Wash();
                    OpenHood(VehicleToFix, false);
                    selectedTalk = "~b~Mechanic~s~: " + imperfectDialogue.GetRandomElement();
                    RepairStatus = ERepairStatus.Imperfect;
                }
                else
                {
                    if (VehicleToFix.HasBone("bonnet") && VehicleToFix.Doors[4].IsValid()) VehicleToFix.Doors[4].BreakOff();
                    VehicleToFix.IsDriveable = false;
                    VehicleToFix.IsExplosionProof = false;
                    VehicleToFix.EngineHealth = 250f;
                    VehicleToFix.FuelTankHealth = Peralatan.Random.Next(150, 750);
                    selectedTalk = "~b~Mechanic~s~: " + failedDialogue.GetRandomElement();
                    int num2 = Peralatan.Random.Next(1, 1000);
                    if (num2 >= 650 && num2 <= 700)
                    {
                        if (VehicleToFix)
                        {
                            VehicleToFix.Explode(false);
                        }
                    }
                    RepairStatus = ERepairStatus.Failed;
                }
                if (Wrench) Wrench.Delete();
                GameFiber.Wait(75);
                if (ToolBox2) ToolBox2.Delete();
                GameFiber.Wait(75);
                ToolBox1.AttachTo(MechanicPed, MechanicPed.GetBoneIndex(PedBoneId.RightPhHand),
                    new Vector3(0.25000003f, -0.0600000098f, -0.0399999879f), new Rotator(-90.0299988f, -79.9999924f, -9.99999905f));
                GameFiber.Wait(75);
                if (Game.LocalPlayer.Character.Position.DistanceTo(MechanicPed.Position) < 60f)
                    MechanicPed.Tasks.FollowNavigationMeshToPosition(Game.LocalPlayer.Character.Position + Game.LocalPlayer.Character.ForwardVector * 1.1125f,
                        Game.LocalPlayer.Character.Heading - 180.0f, 10.0f).WaitForCompletion(7500);
                Game.DisplaySubtitle(selectedTalk, 1000000);
                State = MechanicState.Finish;
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
                Status = MechanicStatus.Error;
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
                State = MechanicState.CleanUp;
                if (State == MechanicState.Finish && RepairStatus != ERepairStatus.Failed && Status != MechanicStatus.Error)
                {
                    if (VehicleToFix)
                    {
                        fixedVehs.Add(VehicleToFix);
                        VehicleToFix.IsExplosionProof = false;
                        if (DismissFixedVehicle) VehicleToFix.Dismiss();
                    }
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
                            MechanicVehicle.Dismiss();
                            MechanicPed.Dismiss();
                        }
                    }
                }
                else
                {
                    if (DismissFixedVehicle && VehicleToFix) VehicleToFix.Dismiss();
                    if (MechanicVehicle && MechanicPed)
                    {
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
                            MechanicVehicle.Dismiss();
                            MechanicPed.Dismiss();
                        }
                    }

                }
                if (Blip) Blip.Delete();
                if (BrokenVehicleBlip) BrokenVehicleBlip.Delete();
                if (ToolBox1) ToolBox1.Delete();
                if (ToolBox2) ToolBox2.Delete();
                if (Wrench) Wrench.Delete();
                if (jc.IsLoaded) jc.Dismiss();
            }
            catch (Exception e)
            {
                if (DismissFixedVehicle && VehicleToFix) VehicleToFix.Delete();
                if (MechanicVehicle && MechanicPed) { MechanicVehicle.Delete(); MechanicPed.Delete(); }
                if (Blip) Blip.Delete();
                if (BrokenVehicleBlip) BrokenVehicleBlip.Delete();
                if (ToolBox1) ToolBox1.Delete();
                if (ToolBox2) ToolBox2.Delete();
                if (Wrench) Wrench.Delete();
                Game.DisplayNotification("~r~Vehicle cleanup error, please send your log file to author");
                e.ToString().ToLog();
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
        private readonly List<string> imperfectDialogue = new List<string>()
        {
            "This should be work now, but unfortunately i can't fix it perfectly",
            "This vehicle is now working, however the engine health decreases",
            "I can't promise this vehicle works as before, but at least the vehicle is drivable now"
        };
        private readonly List<string> failedDialogue = new List<string>()
        {
            "Sorry, this vehicle is too hard to repair, i cant do anything",
            "I'm sorry, i tried my best but this vehicle is badly damaged",
            "My apologize",
            "I'am very sorry",
            "How stupid of me, i can't repair this vehicle, maybe you can call a tow truck. I do really sorry for what i have done",
            "Sir my sincerest apologies, i can't repair this vehicle at all. i'm really sorry",
            "I'm sorry, this vehicle damage is too severe, i can't repair it now. I sincerely apologize",
            "Aah sh*t, my skills is very bad, i can't repair this vehicle, maybe you can call a tow truck",
            "Sir, I apologize from the deepest of my heart because this vehicle seems have severe damage and i can't fix it"
        };
    }
}
