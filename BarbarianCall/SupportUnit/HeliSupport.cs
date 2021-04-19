using BarbarianCall.Freemode;
using BarbarianCall.Types;
using Rage;
using N = Rage.Native.NativeFunction;

namespace BarbarianCall.SupportUnit
{
    public class HeliSupport
    {
        public Spawnpoint SpawnPoint { get; private set; }
        public Entity TargetEntity { get; private set; }
        public Vehicle Helicopter { get; private set; }
        public FreemodePed Pilot { get; private set; }
        public Blip HeliBlip { get; private set; }
        public Blip TargetBlip { get; private set; }
        public bool SuspectInSight { get; private set; } = false;
        public HeliSupport(Entity target)
        {
            bool eupInstalled = N.Natives.IS_DLC_PRESENT<bool>(Game.GetHashKey("eup")) && N.Natives.IS_DLC_PRESENT<bool>(Game.GetHashKey("sup"));
        }
        private void FindSpawnPoint()
        {

        }
        private void SetPedPilotComponent()
        {
            if (Pilot)
            {
                if (Pilot.Gender == LSPD_First_Response.Gender.Male)
                {
                    Pilot.Mask = new PedComponent(PedComponent.EComponentID.Mask, 27, 0);
                    Pilot.Tops = new PedComponent(PedComponent.EComponentID.Tops, 48, 0);
                    Pilot.Leg = new PedComponent(PedComponent.EComponentID.Leg, 30, 0);
                    Pilot.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 24, 0);
                    Pilot.Torso = new PedComponent(PedComponent.EComponentID.Torso, 18, Peralatan.Random.Next(5));
                }
                else if (Pilot.Gender == LSPD_First_Response.Gender.Female)
                {
                    Pilot.Mask = new PedComponent(PedComponent.EComponentID.Mask, 27, 0);
                    Pilot.Tops = new PedComponent(PedComponent.EComponentID.Tops, 41, 0);
                    Pilot.Leg = new PedComponent(PedComponent.EComponentID.Leg, 29, 0);
                    Pilot.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 24, 0);
                    Pilot.Torso = new PedComponent(PedComponent.EComponentID.Torso, 19, Peralatan.Random.Next(5));
                }
            }
        }
    }
}
