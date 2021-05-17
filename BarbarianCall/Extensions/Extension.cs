using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Rage;
using Rage.Native;
using System.Diagnostics;
using System.IO;
using BarbarianCall.Types;

namespace BarbarianCall.Extensions
{
    internal static class Extension
    {
        /// <summary>
        /// Draws a marker in the world, this needs to be done on a per frame basis
        /// </summary>
        /// <param name="type">The type of marker.</param>
        /// <param name="pos">The position of the marker.</param>
        /// <param name="dir">The direction the marker points in.</param>
        /// <param name="rot">The rotation of the marker.</param>
        /// <param name="scale">The amount to scale the marker by.</param>
        /// <param name="color">The color of the marker.</param>
        /// <param name="bobUpAndDown">if set to <c>true</c> the marker will bob up and down.</param>
        /// <param name="faceCamera">if set to <c>true</c> the marker will always face the camera, regardless of its rotation.</param>
        /// <param name="rotateY">if set to <c>true</c> rotates only on the y axis(heading).</param>
        /// <param name="textueDict">Name of texture dictionary to load the texture from, leave null for no texture in the marker.</param>
        /// <param name="textureName">Name of texture inside the dictionary to load the texture from, leave null for no texture in the marker.</param>
        /// <param name="drawOnEntity">if set to <c>true</c> draw on any <see cref="Entity"/> that intersects the marker.</param>
        public static void DrawMarker(MarkerType type, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, Color color,
            bool bobUpAndDown = false, bool faceCamera = false, bool rotateY = false, string textueDict = null, string textureName = null, bool drawOnEntity = false)
        {
            if (!string.IsNullOrEmpty(textueDict) && !string.IsNullOrEmpty(textureName))
            {
                NativeFunction.Natives.DRAW_MARKER((int)type, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, rot.X, rot.Y, rot.Z, scale.X,
                    scale.Y, scale.Z, color.R, color.G, color.B, color.A, bobUpAndDown, faceCamera, 2, rotateY, textueDict,
                    textureName, drawOnEntity);
            }
            else
            {
                NativeFunction.Natives.DRAW_MARKER((int)type, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, rot.X, rot.Y, rot.Z, scale.X,
                    scale.Y, scale.Z, color.R, color.G, color.B, color.A, bobUpAndDown, faceCamera, 2, rotateY, null, null, drawOnEntity);
            }
        }
        internal static Model GetRandomMaleModel() => World.GetAllPeds().Where(x => x && x.IsMale && !x.IsFreemodePed()).Select(x => x.Model).GetRandomElement();
        internal static Model GetRandomFemaleModel() => World.GetAllPeds().Where(x => x && x.IsFemale && !x.IsFreemodePed()).Select(x => x.Model).GetRandomElement();
        internal static Model[] GetAudibleVehicleModel()
        {
            IEnumerable<string> files = Directory.GetFiles(@"lspdfr\audio\scanner\CAR_MODEL").Select(Path.GetFileNameWithoutExtension);
            return Model.VehicleModels.Where(m => files.Any(s => s.ToLower().Contains(m.Name.ToLower()))).ToArray();
        }
        private static bool IsFreemodePed(this Ped ped) => ped.Model.Hash == 0x705E61F2 || ped.Model.Hash == 0x9C9EFFD8;
        internal static Vector3 GetOffsetFromEntityGivenWorldCoords(Entity entity, Vector3 position) => NativeFunction.Natives.GET_OFFSET_FROM_ENTITY_GIVEN_WORLD_COORDS<Vector3>(entity, position.X, position.Y, position.Z);        
        internal static string GetVehicleMakeName(Model model) => NativeFunction.Natives.xF7AF4F159FF99F97<string>(model.Hash);
        internal static bool IsPointOnRoad(this Vector3 position) => NativeFunction.Natives.IS_POINT_ON_ROAD<bool>(position.X, position.Y, position.Z, 0);
        internal static bool IsOccupied(this Vector3 position) => NativeFunction.Natives.xADCDE75E1C60F32D<bool>(position.X, position.Y, position.Z, 3f, false, true, true, false, false, 0, false); //IS_POSITION_OCCUPIED
        internal static bool IsSuitableCar(this Model model) => model.IsCar && !model.IsBigVehicle && (model.NumberOfSeats == 2 || model.NumberOfSeats == 4) && !model.IsEmergencyVehicle && !model.IsLawEnforcementVehicle;
        internal static bool IsSuitableMotor(this Model model) => model.IsBike && !model.IsEmergencyVehicle && !model.IsLawEnforcementVehicle && !model.IsCar && !model.IsBigVehicle && model.NumberOfSeats <= 2;
        internal static bool IsPed(this Entity entity) => NativeFunction.Natives.IS_ENTITY_A_PED<bool>(entity);
        internal static bool IsVehicle(this Entity entity) => NativeFunction.Natives.IS_ENTITY_A_VEHICLE<bool>(entity);
        internal static bool IsObject(this Entity entity) => NativeFunction.Natives.IS_ENTITY_AN_OBJECT<bool>(entity);
        internal static int GetVehicleLiveries(this Vehicle veh) => NativeFunction.Natives.x87B63E25A529D526<int>(veh);
        internal static void SetVehicleLivery(this Vehicle veh, int liveryIndex) => NativeFunction.Natives.SET_VEHICLE_LIVERY(veh, liveryIndex);  
        internal static string GetCardinalDirection(this Entity e, bool fullform) => GetCardinalDirection(e.Heading, fullform);
        internal static string GetCardinalDirection(this Entity e) => GetCardinalDirection(e.Heading, false);
        /// <summary>
        /// Sets a vehicle on the ground on all wheels.  Returns whether or not the operation was successful.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>return true if the operation was successful, otherwise false</returns>
        internal static bool PlaceOnGroundProperly(this Vehicle vehicle) => NativeFunction.Natives.SET_VEHICLE_ON_GROUND_PROPERLY<bool>(vehicle);
        internal static string GetCardinalDirection(float direction, bool fullform)
        {
            float degrees = Math.Abs(direction);
            Game.Console.Print(degrees.ToString());
            string[] abb = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
            string[] full = { "North", "NorthEast", "East", "SouthEast", "South", "SouthWest", "West", "NorthWest", "North" };
            List<string> cardinals = fullform ? full.ToList() : abb.ToList();
            return cardinals[(int)Math.Round((double)degrees % 360 / 45)];
        }
        internal static string GetCardinalDirentionDetailed(float direction, bool fullform)
        {
            float degrees = Math.Abs(direction);
            string[] abb = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
            string[] full =
                {
                "North", "North-NorthEast", "NorthEast", "East-NorthEast", "East",
                "East-SouthEast", "SouthEast", "South-SouthEast", "South", "South-SouthWest",
                "SouthWest", "West-SouthWest", "West", "West-NorthWest", "NorthWest", "North-NorthWest", "North"
            };
            List<string> cardinals = fullform ? full.ToList() : abb.ToList();
            return cardinals[(int)Math.Round((double)degrees * 10 % 3600 / 225)];
        }
        internal static string GetFileVersion(string filepath)
        {
            if (!File.Exists(filepath)) return $"{filepath} File doesn't exist";
            try
            {
                FileVersionInfo ver = FileVersionInfo.GetVersionInfo(filepath);
                return $"{ver.FileMajorPart}.{ver.FileMinorPart}.{ver.FileBuildPart}.{ver.FilePrivatePart}";
            }
            catch (Exception e)
            {
                "Get file version error".ToLog();
                e.ToString().ToLog();
            }
            return "Unknown version";
        }
        internal static bool CanSee(this Ped ped, Ped target) => NativeFunction.Natives.x6CD5A433374D4CFB<bool>(ped, target);
        internal static Vector3 ToGround(this Vector3 position)
        {
            if (NativeFunction.Natives.x9E82F0F362881B29<bool>(position.X, position.Y, 1250.0125f, out float groundPosition, 0,0)) //_GET_GROUND_Z_FOR_3D_COORD_2
            {
                return new Vector3(position.X, position.Y, groundPosition);
            }
            return position;
        }
        internal static Vector3 GetEntryPositionOfVehicleDoor(Vehicle vehicle, int doorIndex) => NativeFunction.Natives.xC0572928C0ABFDA3<Vector3>(vehicle, doorIndex);
        internal static Vector3 GetEntryPositionOfVehicleDoor(Vehicle vehicle, VehicleDoorIndex doorIndex)
        {
            Vector3 ret = GetEntryPositionOfVehicleDoor(vehicle, (int)doorIndex);
            ret.ToString().ToLog();
            return ret;
        }
        internal static void SetPedVehicleForcedSeatUsage(Ped ped, Vehicle vehicle, int seatIndex) => NativeFunction.Natives.SET_PED_VEHICLE_FORCED_SEAT_USAGE(ped, vehicle, seatIndex, 2, -2);
        internal static void DeleteRelationshipGroup(RelationshipGroup relationshipGroup)
        {
            bool exist = false;
            exist = NativeFunction.Natives.xCC6E3B6BB69501F1<bool>(relationshipGroup.Hash); //_DOES_RELATIONSHIP_GROUP_EXIST
            if (exist) NativeFunction.Natives.REMOVE_RELATIONSHIP_GROUP(relationshipGroup.Hash);
        }          
        public static string GetLocalizedString(IntPtr stringPtr) => NativeFunction.Natives.x7B5280EBA9840C72<string>(stringPtr); //_GET_LABEL_TEXT

        public static void SetForwardSpeed(this Vehicle vehicle, float speed)
        {
            speed = MathHelper.Clamp(speed, 1, 29);
            NativeFunction.Natives.SET_VEHICLE_FORWARD_SPEED(vehicle, speed);
        }
        public static int GetAge(this LSPD_First_Response.Engine.Scripting.Entities.Persona persona)
        {
            var BirthDay = persona.Birthday;
            DateTime today = DateTime.Today;
            int age = today.Year - BirthDay.Year;
            if (BirthDay.Date < today.AddYears(-age)) age--;
            return age;
        }
        internal static void PlaySoundFrontEnd(string audioName, string audioRef) => NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, audioName, audioRef, true);
        public static void DrawLine(Vector3 pos1, Vector3 pos2, Color color) => NativeFunction.Natives.DRAW_LINE(pos1, pos2, color.R, color.G, color.B, color.A);
        /// <summary>
        /// Gets the <see cref="VehicleColor"/> of this <see cref="Vehicle"/>
        /// </summary>
        public static VehicleColor GetColor(this Vehicle vehicle)
        {
            NativeFunction.Natives.GET_VEHICLE_COLOURS(vehicle, out int primary, out int secondary);
            VehiclePaint primaryColor = primary >= 0 && primary <= 160 ? (VehiclePaint)primary : VehiclePaint.Unknown;
            VehiclePaint secondaryColor = secondary >= 0 && secondary <= 160 ? (VehiclePaint)secondary : VehiclePaint.Unknown;
            return new VehicleColor(primaryColor, secondaryColor);
        }
        /// <summary>
        /// Sets this <see cref="Vehicle"/> colors
        /// </summary>
        public static void SetColor(this Vehicle vehicle, VehicleColor vehicleColor)
        {
            if (vehicleColor.PrimaryColor == VehiclePaint.Unknown || vehicleColor.SecondaryColor == VehiclePaint.Unknown)
            {
                throw new NotSupportedException();
            }
            NativeFunction.Natives.SET_VEHICLE_COLOURS(vehicle, (int)vehicleColor.PrimaryColor, (int)vehicleColor.SecondaryColor);
        }
        /// <summary>
        /// Sets this <see cref="Vehicle"/> primary and secondary colors
        /// </summary>
        /// <param name="primaryColor">The primary color to be sets</param>
        /// <param name="secondaryColor">The secondary color to be sets</param>
        public static void SetColor(this Vehicle vehicle, VehiclePaint primaryColor, VehiclePaint secondaryColor) => vehicle.SetColor(new VehicleColor(primaryColor, secondaryColor));
        /// <summary>
        /// Sets this <see cref="Vehicle"/> primary color only
        /// </summary>
        /// <param name="primaryColor">The primary color to be sets</param>
        public static void SetColor(this Vehicle vehicle, VehiclePaint primaryColor) => vehicle.SetColor(new VehicleColor(primaryColor, vehicle.GetColor().SecondaryColor));
        public static void MarkAsNoLongerNeeded(this Entity entity)
        {
            if (entity)
            {
                entity.Dismiss();
            }
        }

        public enum VehicleWindowIndex
        {
            FrontLeftWindow,
            FrontRightWindow,
            MiddleLeftWindow,
            MiddleRightWindow,
            BackLeftWindow,
            BackRightWindow,
            WindscreenFront,
            BackWindow,
        }
        public enum VehicleDoorIndex
        {
            FrontRightDoor = 1,
            FrontLeftDoor = 0,
            BackRightDoor = 3,
            BackLeftDoor = 2,
            Hood = 4,
            Trunk = 5
        }
        public enum MarkerType
        {
            UpsideDownCone, VerticalCylinder, ThickChevronUp,
            ThinChevronUp, CheckeredFlagRect,
            CheckeredFlagCircle, VerticleCircle,
            PlaneModel, LostMCDark, LostMCLight, Number0,
            Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8,
            Number9, ChevronUpx1, ChevronUpx2, ChevronUpx3, HorizontalCircleFat, ReplayIcon, HorizontalCircleSkinny,
            HorizontalCircleSkinnyArrow, HorizontalSplitArrowCircle, DebugSphere, DollarSign, HorizontalBars, WolfHead, QuestionMark,
            PlaneSymbol, HelicopterSymbol, BoatSymbol, CarSymbol, MotorcycleSymbol, BikeSymbol, TruckSymbol, ParachuteSymbol,
            SawbladeSymbol
        }
    }
}
