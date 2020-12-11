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
