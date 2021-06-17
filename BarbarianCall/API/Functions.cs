using System.Reflection;
using Rage;

namespace BarbarianCall.API
{
    public static class Functions
    {
        public static void DisplayPedDetails()
        {
            $"Executing DisplayPedDetails. Request from {Assembly.GetCallingAssembly().GetName().Name}".ToLog();
            if (Types.Manusia.CurrentManusia == null)
            {
                "Ped description is null".ToLog();
                return;
            }
            else if (!LSPD_First_Response.Mod.API.Functions.IsCalloutRunning())
            {
                Game.DisplayHelp("No callout running detected");
                return;
            }
            Types.Manusia.CurrentManusia.DisplayNotif();
        }
        public static void CallMechanic()
        {
            $"Executing CallMechanic. Request From {Assembly.GetCallingAssembly().GetName().Name}".ToLog();
            string temp = Menus.MainMenu.mechanic.SelectedItem;
            Menus.MainMenu.mechanic.SelectedItem = "Nearby Vehicle";
            GameFiber.Wait(75);
            Menus.MainMenu.mechanic.Activate(Menus.MainMenu.BarbarianCallMenu);
            GameFiber.Wait(75);
            Menus.MainMenu.mechanic.SelectedItem = temp;
        }
        public static void CallMechanic(Vehicle vehicle)
        {
            $"Executing CallMechanic. Request From {Assembly.GetCallingAssembly().GetName().Name}".ToLog();
            if (vehicle)
            {
                SupportUnit.Mechanic mechanic = new(vehicle);
                mechanic.RespondToLocation();
            }
            else "Vehicle is null".ToLog();
        }
        public static void CallMechanicToRepairPlayerVehicle()
        {
            $"Executing CallMechanicToRepairPlayerVehicle. Request From {Assembly.GetCallingAssembly().GetName().Name}".ToLog();
            if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
            {
                Game.DisplayNotification("~b~Please leave any vehicle first");
                return;
            }
            if (Game.LocalPlayer.Character.LastVehicle)
            {
                SupportUnit.Mechanic mechanic = new(Game.LocalPlayer.Character.LastVehicle)
                {
                    DismissFixedVehicle = false,
                    SuccessProbability = 1f
                };
                mechanic.RespondToLocation();
            }
            else "Your last vehicle is not found, please make sure you has been in any vehicle before".DisplayNotifWithLogo("~y~Mechanic Service");
        }
    }
}
