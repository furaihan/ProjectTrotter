using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rage;
using Rage.Native;
using System.Diagnostics;
using System.IO;

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
		internal static bool IsPointOnRoad(this Vector3 position) => NativeFunction.Natives.IS_POINT_ON_ROAD<bool>(position.X, position.Y, position.Z, 0);
		internal static int GetVehicleLiveries(this Vehicle veh) => NativeFunction.CallByHash<int>(0x87B63E25A529D526, veh);
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
			return cardinals[(int)Math.Round(((double)degrees % 360) / 45)];
		}
		internal static string GetCardinalDirentionDetailed(float direction, bool fullform)
        {
			float degrees = Math.Abs(direction);
			string[] abb = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
			string [] full = 
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
			if (!File.Exists(filepath)) return $"{AppDomain.CurrentDomain}/{filepath} File doesn't exist";
            try
            {
				var ver = FileVersionInfo.GetVersionInfo(filepath);
				return $"{ver.FileMajorPart}.{ver.FileMinorPart}.{ver.FileBuildPart}.{ver.FilePrivatePart}";
            }
            catch (Exception e)
            {
				"Get file version error".ToLog();
				e.ToString().ToLog();
            }
			return "Unknown version";
        }
		internal static float GetRandomAbsoluteSingle() => GetRandomAbsoluteSingle(Peralatan.Random.Next(), Peralatan.Random.Next());
		internal static float GetRandomAbsoluteSingle(float min, float max) => GetRandomAbsoluteSingle(Math.Abs((int)Math.Round(min)), Math.Abs((int)Math.Round(max)));
		internal static float GetRandomAbsoluteSingle(int min, int max)
        {
			var siji = Peralatan.Random.Next(min, max);
			var loro = Peralatan.Random.NextDouble();
			return (float)((float)siji + loro);
        }
		public static string GetLocalizedString(IntPtr stringPtr)
		{
			return NativeFunction.Natives.x7B5280EBA9840C72<string>(stringPtr); //_GET_LABEL_TEXT
		}

		public static unsafe bool IsStringEmpty(IntPtr stringPtr)
		{
			return *(byte*)stringPtr == 0;
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
