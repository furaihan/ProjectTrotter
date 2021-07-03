using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using N = Rage.Native.NativeFunction;
using BarbarianCall.Freemode;
using BarbarianCall.Extensions;
using LSPD_First_Response.Mod.API;

namespace BarbarianCall.SupportUnit
{
    public class MilitaryHeliSupport
    {
        private Model _model;
        public Vehicle Vehicle { get; private set; }
        public FreemodePed Ped { get; private set; }
        public MilitarySupportType Type { get; private set; }
        public Blip Blip { get; private set; }
        public Entity TargetEntity { get; private set; }
        public Vector3 SpawnLocation { get; private set; }
        private MilitaryHeliSupport(Entity entity, MilitarySupportType type)
        {
            GameFiber.StartNew(() =>
            {
                _model = type switch
                {
                    MilitarySupportType.Annihilator => 0x11962E49,
                    MilitarySupportType.Buzzard => 0x2C75F0DD,
                    MilitarySupportType.Hunter => 0xFD707EDE,
                    MilitarySupportType.Sparrow => 0x494752F7,
                    _ => 0x494752F7,
                };
                SpawnLocation = FindSpawnPoint(entity.AbovePosition);
                TargetEntity = entity;
                _model.LoadAndWait();
                Vehicle = new Vehicle(_model, SpawnLocation);
                Vehicle.Mods.ApplyAllMods();
                Vehicle.IsEngineOn = true;
                Vehicle.IsPersistent = true;
                Vehicle.TopSpeed += 50f;
                Blip = new Blip(Vehicle)
                {
                    Sprite = (BlipSprite)487,
                    Color = RAGENativeUI.HudColorExtensions.GetColor(RAGENativeUI.HudColor.RadarArmour),
                    Name = "Heli Support Unit",
                };
                N.Natives.SET_HELI_BLADES_FULL_SPEED(Vehicle);
                Ped = new FreemodePed(Vector3.Zero, Peralatan.Random.Next(5) != 1);
                SetPedPedComponent();
                Ped.WarpIntoVehicle(Vehicle, -1);
                GameFiber.Wait(2500);
                $"Vehicle has weapon: {N.Natives.DOES_VEHICLE_HAVE_WEAPONS<bool>(Vehicle)}".ToLog();
                Ped.HeliMission(Vehicle, (Vehicle)(TargetEntity.IsVehicle() ? TargetEntity : null), (Ped)(TargetEntity.IsPed() ? TargetEntity : null), Vector3.Zero, MissionType.Attack, 35f, 30f, -1f, -1, 50, 0);
                Ped.MakeMissionPed(true);
                Functions.SetPedAsCop(Ped);
                Functions.SetCopAsBusy(Ped, true);
                Functions.PlayScannerAudio("HELI_APPROACHING_DISPATCH");
                while (true)
                {
                    GameFiber.Yield();
                    if (TargetEntity)
                    {
                        if (TargetEntity.IsDead) break;
                    }
                    else break;
                }
                CleanUp();
            });          
        }
        public MilitaryHeliSupport(Ped targetPed, MilitarySupportType type) : this(entity: targetPed, type)
        {
        }
        public MilitaryHeliSupport(Vehicle vehicleTarget, MilitarySupportType type) : this(entity: vehicleTarget, type)
        {
        }
        private Vector3 FindSpawnPoint(Vector3 targetPos)
        {
            Vector3 vector3 = targetPos.Around2D(Peralatan.Random.Next(450, 801)) + Vector3.WorldUp * 450.0f;
            while (targetPos.DistanceTo(vector3) < 400.0f)
            {
                vector3 = targetPos.Around2D(Peralatan.Random.Next(450, 801)) + Vector3.WorldUp * 450.0f;
                GameFiber.Yield();
            }
            return vector3;
        }
        private void SetPedPedComponent()
        {
            "Setting heli unit pilot component".ToLog();
            if (Ped)
            {
                if (Ped.Gender == LSPD_First_Response.Gender.Male)
                {
                    Ped.Mask = new PedComponent(PedComponent.EComponentID.Mask, 27, 0);
                    Ped.Tops = new PedComponent(PedComponent.EComponentID.Tops, 48, 0);
                    Ped.Leg = new PedComponent(PedComponent.EComponentID.Leg, 30, 0);
                    Ped.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 24, 0);
                    Ped.Torso = new PedComponent(PedComponent.EComponentID.Torso, 18, Peralatan.Random.Next(5));
                }
                else if (Ped.Gender == LSPD_First_Response.Gender.Female)
                {
                    Ped.Mask = new PedComponent(PedComponent.EComponentID.Mask, 27, 0);
                    Ped.Tops = new PedComponent(PedComponent.EComponentID.Tops, 41, 0);
                    Ped.Leg = new PedComponent(PedComponent.EComponentID.Leg, 29, 0);
                    Ped.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 24, 0);
                    Ped.Torso = new PedComponent(PedComponent.EComponentID.Torso, 19, Peralatan.Random.Next(5));
                }
            }
            else "Ped not exist".ToLog();
        }
        public void CleanUp()
        {
            if (Ped)
            {
                Functions.SetCopAsBusy(Ped, false);
                Ped.Dismiss();
            }
            if (Vehicle)
            {
                Vehicle.Dismiss();
            }
            if (Blip) Blip.Delete();
        }
    }
}
