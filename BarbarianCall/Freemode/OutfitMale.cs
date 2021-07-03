using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbarianCall.Freemode
{
    public static class OutfitMale
    {
		internal static List<Action<FreemodePed>> Casual = new List<Action<FreemodePed>>()
		{
			SetCasual1Component,
			SetCasual2Component,
			SetCasual3Component,
			SetCasual4Component,
			SetCasual5Component,
			SetCasual6Component,
			SetCasual7Component,
			SetCasual8Component,
			SetCasual9Component,
		};
		public static void SetCasual1Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 0, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 0, 5, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 1, 0, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 17, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 33, 0, 0);
		}
		public static void SetCasual2Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 4, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 23, 8, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 20, 8, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 12, 2, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 10, 2, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 23, 0, 0);
		}
		public static void SetCasual3Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 11, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 10, 0, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 11, 12, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 10, 2, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 26, 9, 0);
		}
		public static void SetCasual4Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 4, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 22, 11, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 20, 6, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 25, 4, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 4, 1, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 24, 0, 0);
		}
		public static void SetCEO1Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 4, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 82, 6, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 12, 3, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 15, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 0, 1, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 192, 1, 0);
		}
		public static void SetCEO2Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 4, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 63, 0, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 2, 13, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 139, 3, 0);
		}
		public static void SetCEO3Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 101, 13, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 33, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 78, 2, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 57, 10, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 71, 3, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 203, 25, 0);
		}
		public static void SetMariner1Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 2, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 124, 16, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 97, 0, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 171, 2, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 238, 1, 0);
		}
		public static void SetCasual5Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 0, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 90, 2, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 61, 5, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 208, 9, 0);
		}
		public static void SetCasual6Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 1, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 4, 0, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 0, 10, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 12, 1, 0);
		}
		public static void SetCasual7Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 1, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 1, 0, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 4, 0, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 1, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 7, 4, 0);
		}
		public static void SetCasual8Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 0, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 1, 1, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 7, 3, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 9, 2, 0);
		}
		public static void SetRich1Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 4, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 26, 9, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 28, 4, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 54, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 10, 2, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 70, 1, 0);
		}
		public static void SetCasual9Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 41, 1, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 46, 1, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 35, 0, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 112, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 97, 0, 0);
		}
		public static void SetJuggernautComponent(this FreemodePed ped)
		{
			int rnd = Peralatan.Random.Next(2, 11);
			if (ped.IsMale)
            {
				ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 91, rnd, 0);
				ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 42, 0, 0);
				ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 84, rnd, 0);
				ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
				ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 33, 0, 0);
				ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
				ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 97, rnd, 0);
				ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
				ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
				ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 186, rnd, 0);
			}		
			else if (ped.IsFemale)
            {
				ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 91, rnd, 0);
				ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 49, 0, 0);
				ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 86, rnd, 0);
				ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
				ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 1, 16, 0);
				ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
				ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 105, rnd, 0);
				ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
				ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
				ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 188, rnd, 0);
			}
		}
		public static void SetCasual10Component(this FreemodePed ped)
		{
			if (!ped.IsMale) return;
			ped.Wardrobe.Mask = new PedComponent(PedComponent.EComponentID.Mask, 0, 0, 0);
			ped.Wardrobe.Torso = new PedComponent(PedComponent.EComponentID.Torso, 11, 0, 0);
			ped.Wardrobe.Leg = new PedComponent(PedComponent.EComponentID.Leg, 1, 11, 0);
			ped.Wardrobe.Parachute = new PedComponent(PedComponent.EComponentID.Parachute, 0, 0, 0);
			ped.Wardrobe.Shoes = new PedComponent(PedComponent.EComponentID.Shoes, 38, 1, 0);
			ped.Wardrobe.Accessories = new PedComponent(PedComponent.EComponentID.Accessories, 0, 0, 0);
			ped.Wardrobe.UnderShirt = new PedComponent(PedComponent.EComponentID.UnderShirt, 15, 0, 0);
			ped.Wardrobe.BodyArmor = new PedComponent(PedComponent.EComponentID.BodyArmor, 0, 0, 0);
			ped.Wardrobe.Decal = new PedComponent(PedComponent.EComponentID.Decal, 0, 0, 0);
			ped.Wardrobe.Tops = new PedComponent(PedComponent.EComponentID.Tops, 135, 0, 0);
		}
	}
}
