using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Rage;
using Rage.Native;
using System.Diagnostics;
using System.IO;
using BarbarianCall.Types;
using BarbarianCall.Freemode;
using L = LSPD_First_Response.Mod.API.Functions;

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
        internal static Model GetRandomMaleModel() => World.GetAllPeds().Where(x => x && x.IsHuman && x.IsMale && !x.IsFreemodePed() && !L.IsPedACop(x) && !x.CreatedByTheCallingPlugin).Select(x => x.Model).GetRandomElement();
        internal static Model GetRandomFemaleModel() => World.GetAllPeds().Where(x => x && x.IsHuman && x.IsFemale && !x.IsFreemodePed() && !L.IsPedACop(x) && !x.CreatedByTheCallingPlugin).Select(x => x.Model).GetRandomElement();
        internal static void GetAudibleVehicleModel()
        {
            IEnumerable<string> files = Directory.GetFiles(@"lspdfr\audio\scanner\CAR_MODEL", "*.wav", SearchOption.AllDirectories).Select(Path.GetFileNameWithoutExtension).Where(x => !x.ToLower().StartsWith("0x"));
            files = files.Select(x => x.Replace("_01", "")).Select(x => x.Replace("_", ""));
            List<uint> hashes = files.Select(x => Game.GetHashKey(x.ToLower())).ToList();
            List<Model> ret = Model.VehicleModels.Where(m => hashes.Contains(m.Hash)).ToList();
            int c = 0;
            IEnumerable<string> fb = Directory.EnumerateFiles(@"lspdfr\audio\scanner\CAR_MODEL", "*.wav", SearchOption.AllDirectories).Select(Path.GetFileNameWithoutExtension);
            foreach (var x in fb)
            {
                if (x.ToLower().StartsWith("0x"))
                {
                    if (uint.TryParse(x, out var hash))
                    {
                        Model model = new(hash);
                        if (model.IsInCdImage && model.IsValid && !ret.Contains(model))
                        {
                            ret.Add(model);
                            if (!Globals.AudioHash.ContainsKey(model.Hash)) Globals.AudioHash.Add(model.Hash, x.Replace("_01", ""));
                            c++;
                            if (c % 10 == 0) GameFiber.Yield();
                        }
                    }
                }
                else
                {
                    var name = x.Replace("_01", "").Replace("_", "");
                }
            }
            foreach (var x in fb)
            {
                string file = x.Replace("_01", "");
                file = file.Replace("_", "").ToLower();
                //Peralatan.ToLog($"Reading car model {file}");
                for (int i = 2; i <= 9; i++)
                {
                    Model model = new(file + i.ToString());
                    if (model.IsInCdImage && model.IsValid && !ret.Contains(model))
                    {
                        ret.Add(model);
                        if (!Globals.AudioHash.ContainsKey(model.Hash)) Globals.AudioHash.Add(model.Hash, x.Replace("_01", ""));
                    }
                }
                c++;
                if (c % 10 == 0) GameFiber.Yield();
            }
            Globals.AudibleCarModel = ret.Where(x => x.IsInCdImage && x.IsValid).ToArray();
        }
        internal static bool IsFreemodePed(this Ped ped) => ped.Model.Hash == 0x705E61F2 || ped.Model.Hash == 0x9C9EFFD8;
        internal static Vector3 GetOffsetFromEntityGivenWorldCoords(Entity entity, Vector3 position) => NativeFunction.Natives.GET_OFFSET_FROM_ENTITY_GIVEN_WORLD_COORDS<Vector3>(entity, position.X, position.Y, position.Z);        
        internal static string GetVehicleMakeName(Model model) => NativeFunction.Natives.GET_MAKE_NAME_FROM_VEHICLE_MODEL<string>(model.Hash);
        internal static bool IsPointOnRoad(this Vector3 position) => NativeFunction.Natives.IS_POINT_ON_ROAD<bool>(position.X, position.Y, position.Z, 0);
        internal static bool IsOccupied(this Vector3 position) => NativeFunction.Natives.IS_POSITION_OCCUPIED<bool>(position.X, position.Y, position.Z, 3f, false, true, true, false, false, 0, false); //IS_POSITION_OCCUPIED
        internal static bool IsSuitableCar(this Model model) => model.IsCar && !model.IsBigVehicle && (model.NumberOfSeats == 2 || model.NumberOfSeats == 4) && !model.IsEmergencyVehicle && !model.IsLawEnforcementVehicle;
        internal static bool IsSuitableMotor(this Model model) => model.IsBike && !model.IsEmergencyVehicle && !model.IsLawEnforcementVehicle && !model.IsCar && !model.IsBigVehicle && model.NumberOfSeats <= 2;
        internal static bool IsAmbientOnFoot(this Ped ped)
        {
            RelationshipGroup[] bl = { RelationshipGroup.Cop, RelationshipGroup.Medic, RelationshipGroup.Army, RelationshipGroup.PrivateSecurity, RelationshipGroup.Fireman, RelationshipGroup.SecurityGuard };
            return ped && !ped.CreatedByTheCallingPlugin && ped.IsHuman && ped.GetAttachedBlips().Length == 0 && ped.IsOnFoot && !L.IsPedACop(ped) && !bl.Contains(ped.RelationshipGroup) && !ped.IsPlayer;
        }
        internal static Ped GetRandomHatedPed(this Ped ped)
        {
            var pds = ped.GetNearbyPeds(16).Where(x => x && NativeFunction.Natives.GET_RELATIONSHIP_BETWEEN_PEDS<Relationship>(ped, x) == Relationship.Hate);
            Ped ret = null;
            if (pds.Any(x => x)) ret = pds.Where(x => x).OrderBy(x => x.DistanceToSquared(ped)).FirstOrDefault();
            if (ret) return ret;
            var pds1 = World.GetAllPeds();
            return pds1.Where(x => x && NativeFunction.Natives.GET_RELATIONSHIP_BETWEEN_PEDS<Relationship>(x, ped) == Relationship.Hate).OrderBy(x => x.DistanceToSquared(ped)).FirstOrDefault();
        }
        internal static Relationship GetRelationshipAgainst(this Ped ped, Ped target)
        {
            int relationship = NativeFunction.Natives.GET_RELATIONSHIP_BETWEEN_PEDS<int>(ped, target);
            return (Relationship)relationship;
        }
        public static Ped Clone(this Ped ped, bool copyHeadblend)
        {
            return NativeFunction.Natives.CLONE_PED<Ped>(ped, false, false, copyHeadblend);
        }
        public static bool IsTaskActive(this Ped ped, PedTask task)
        {
            return NativeFunction.Natives.GET_IS_TASK_ACTIVE<bool>(ped, (int)task);
        }
        public static void SufferCriticalHit(this Ped ped, bool value)
        {
            if (ped)
            {
                NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(ped, value);
            }
        }
        public static PedCombatProperty GetCombatProperty(this Ped ped) => ped.Metadata.BAR_CombatProperty ??= new PedCombatProperty(ped);
        internal static bool IsPed(this Entity entity) => NativeFunction.Natives.IS_ENTITY_A_PED<bool>(entity);
        internal static bool IsVehicle(this Entity entity) => NativeFunction.Natives.IS_ENTITY_A_VEHICLE<bool>(entity);
        internal static bool IsObject(this Entity entity) => NativeFunction.Natives.IS_ENTITY_AN_OBJECT<bool>(entity);
        internal static int GetLiveries(this Vehicle veh) => NativeFunction.Natives.GET_VEHICLE_LIVERY_COUNT<int>(veh);
        internal static void SetLivery(this Vehicle veh, int liveryIndex) => NativeFunction.Natives.SET_VEHICLE_LIVERY(veh, liveryIndex);
        internal static bool IsStuckOnRoof(this Vehicle vehicle) => NativeFunction.Natives.IS_VEHICLE_STUCK_ON_ROOF<bool>(vehicle);
        internal static MissionType GetActiveMissionType(this Vehicle vehicle)
        {
            if (vehicle)
            {
                var missionType = NativeFunction.Natives.GET_ACTIVE_VEHICLE_MISSION_TYPE<int>(vehicle);
                return (MissionType)missionType;
            }
            return (MissionType)(-1);
        }
        internal static string GetCardinalDirection(this Entity e, bool fullform) => GetCardinalDirection(e.Heading, fullform);
        internal static string GetCardinalDirection(this Entity e) => GetCardinalDirection(e.Heading, false);
        /// <summary>
        /// Sets a vehicle on the ground on all wheels.  Returns whether or not the operation was successful.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>return true if the operation was successful, otherwise false</returns>
        internal static bool PlaceOnGroundProperly(this Vehicle vehicle) => NativeFunction.Natives.SET_VEHICLE_ON_GROUND_PROPERLY<bool>(vehicle, 5.0f);
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
        internal static bool CanSee(this Ped ped, Ped target) => NativeFunction.Natives.CAN_PED_SEE_HATED_PED<bool>(ped, target);
        internal static Vector3 ToGround(this Vector3 position)
        {
            if (NativeFunction.Natives.GET_GROUND_Z_EXCLUDING_OBJECTS_FOR_3D_COORD<bool>(position.X, position.Y, 1250.0125f, out float groundPosition, 0,0)) //_GET_GROUND_Z_FOR_3D_COORD_2
            {
                return new Vector3(position.X, position.Y, groundPosition);
            }
            return position;
        }
        internal static Vector3 GetEntryPositionOfVehicleDoor(Vehicle vehicle, int doorIndex) => NativeFunction.Natives.GET_ENTRY_POINT_POSITION<Vector3>(vehicle, doorIndex);
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
            exist = NativeFunction.Natives.DOES_RELATIONSHIP_GROUP_EXIST<bool>(relationshipGroup.Hash); //_DOES_RELATIONSHIP_GROUP_EXIST
            if (exist) NativeFunction.Natives.REMOVE_RELATIONSHIP_GROUP(relationshipGroup.Hash);
        }          

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
        public static void SetRandomColor(this Vehicle vehicle)
        {
            VehiclePaint[] blackList =
            {
                VehiclePaint.DEFAULT_ALLOY_COLOR, VehiclePaint.Unknown, VehiclePaint.Police_Car_Blue, VehiclePaint.Metallic_Taxi_Yellow, VehiclePaint.MP100_GOLD, VehiclePaint.MP100_GOLD_SATIN,
                VehiclePaint.MP100_GOLD_SPEC, VehiclePaint.MODSHOP_BLACK1,
            };
            VehiclePaint selected = Enum.GetValues(typeof(VehiclePaint)).Cast<VehiclePaint>().Except(blackList).GetRandomElement();
            vehicle.SetColor(new VehicleColor(selected, selected));
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
    }
}
