using Rage;
using LSPD_First_Response.Mod.API;
using N = Rage.Native.NativeFunction;
using ProjectTrotter.Extensions;
using ProjectTrotter.MyPed;

namespace ProjectTrotter.SupportUnit
{
    public class HeliSupport
    {
        public Vector3 SpawnLocation { get; private set; }
        public Entity TargetEntity { get; private set; }
        public Vehicle Helicopter { get; private set; }
        internal FreemodePed Pilot { get; private set; }
        private Blip Blip { get; set; }
        private HeliSupport(Entity target)
        {
            $"Eup Installed: {N.Natives.IS_DLC_PRESENT<bool>(Game.GetHashKey("eup"))} Sup Installed: {N.Natives.IS_DLC_PRESENT<bool>(Game.GetHashKey("sup"))}".ToLog();
            SpawnLocation = FindSpawnPoint(target.Position);
            TargetEntity = target;
            Helicopter = new Vehicle(0x494752F7, SpawnLocation);
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
            N.Natives.SET_HELI_BLADES_FULL_SPEED(Helicopter);
            Pilot = new FreemodePed(Vector3.Zero, MyRandom.Next(5) != 1);
            SetPedPilotComponent();
            Pilot.WarpIntoVehicle(Helicopter, -1);
            Pilot.HeliMission(Helicopter, (Vehicle)(TargetEntity.IsVehicle() ? TargetEntity : null), (Ped)(TargetEntity.IsPed() ? TargetEntity : null), Vector3.Zero, MissionType.Circle, 35f, 30f, -1.0f, -1, 80, 2048);
            Pilot.MakeMissionPed(true);
            Functions.SetPedAsCop(Pilot);
            Functions.SetCopAsBusy(Pilot, true);
            Functions.PlayScannerAudio("HELI_APPROACHING_DISPATCH");
        }
        public HeliSupport(Ped targetPed) : this(target: targetPed)
        {
        }
        public HeliSupport(Vehicle vehicle) : this(target: vehicle)
        {
        }
        private Vector3 FindSpawnPoint(Vector3 targetPos)
        {
            Vector3 vector3 = targetPos.Around2D(MyRandom.Next(450, 801)) + Vector3.WorldUp * 450.0f;
            while (targetPos.DistanceTo(vector3) < 400.0f)
            {
                vector3 = targetPos.Around2D(MyRandom.Next(450, 801)) + Vector3.WorldUp * 450.0f;
                GameFiber.Yield();
            }
            return vector3;
        }
        private void SetPedPilotComponent()
        {
            "Setting heli unit pilot component".ToLog();
            if (Pilot != null)
            {
                if (Pilot.Gender == LSPD_First_Response.Gender.Male)
                {
                    Pilot.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(27, 0);
                    Pilot.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(48, 0);
                    Pilot.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(30, 0);
                    Pilot.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(24, 0);
                    Pilot.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(18, MyRandom.Next(5));
                }
                else if (Pilot.Gender == LSPD_First_Response.Gender.Female)
                {
                    Pilot.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(27, 0);
                    Pilot.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(41, 0);
                    Pilot.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(29, 0);
                    Pilot.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(24, 0);
                    Pilot.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(19, MyRandom.Next(5));
                }
            }

            else "Pilot not exist".ToLog();
        }      
        public void CleanUp()
        {
            if (Pilot) Functions.SetCopAsBusy(Pilot, false);
            if (Helicopter) Helicopter.Dismiss();
            if (Pilot) Pilot.Dismiss();
            if (Blip) Blip.Delete();
        }
    }
}
