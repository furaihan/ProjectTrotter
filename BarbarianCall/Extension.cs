using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rage;
using Rage.Native;

namespace BarbarianCall
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
		/// <param name="drawOnEntity">if set to <c>true</c> draw on any <see cref="Rage.Entity"/> that intersects the marker.</param>
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
		internal static int GetVehicleLiveries(this Vehicle veh) => NativeFunction.CallByHash<int>(0x87B63E25A529D526, veh);
		internal static void SetVehicleLivery(this Vehicle veh, int liveryIndex) => NativeFunction.Natives.SET_VEHICLE_LIVERY(veh, liveryIndex);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ped"></param>
		/// <param name="veh"></param>
		/// <param name="location"></param>
		/// <param name="heading"></param>
		/// <param name="mode"></param>
		/// <param name="radius">Radius seems to define how close the vehicle has to be -after parking- to the position for this task considered completed. 
		/// If the value is too small, the vehicle will try to park again until it's exactly where it should be. 20.0 Works well but lower values don't, 
		/// like the radius is measured in centimeters or something.  </param>
		/// <param name="keepEngineOn"></param>
		/// <returns></returns>
		internal static Rage.Task ParkVehicle(this Ped ped, Vehicle veh, Vector3 location, float heading, EParkMode mode, float radius, bool keepEngineOn)
		{
			var diff = ped.Heading.CompareTo(heading);
			$"Heading difference : {diff}".ToLog();
			NativeFunction.Natives.TASK_VEHICLE_PARK(ped, veh, location.X, location.Y, location.Z, heading, (int)mode, radius, keepEngineOn);
			return Rage.Task.GetTask(ped, "TASK_VEHICLE_PARK");
		}
		internal static Rage.Task ParkVehicle(this Ped ped, Vector3 location, float heading, EParkMode mode, float radius, bool keepEngineOn) => 
			ParkVehicle(ped, ped.CurrentVehicle, location, heading, mode, radius, keepEngineOn);
		internal static Ped CreateRandomPedAtCoords(Vector3 coord) => NativeFunction.Natives.CREATE_RANDOM_PED<Ped>(coord.X, coord.Y, coord.Z);
		public enum EParkMode:int { IgnoreHeading = 0, ParkForward, ParkBackWards}

		internal static void PlaceWaypoint(this Vector3 pos) => PlaceWaypoint(new Vector2(pos.X, pos.Y));
		internal static void PlaceWaypoint(this Vector2 pos) => NativeFunction.Natives.SET_NEW_WAYPOINT(pos.X, pos.Y);
		internal static void RemoveWaypoint() => NativeFunction.Natives.SET_WAYPOINT_OFF();

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
