using System;
using System.Collections.Generic;

namespace BarbarianCall.MyPed
{
    internal static class OutfitMale
    {
        internal static List<Action<FreemodePed>> Casual = new()
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
        internal static void SetCasual1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(0, 5, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(1, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(17, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(33, 0, 0);
        }
        internal static void SetCasual2Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(23, 8, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(20, 8, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(12, 2, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(10, 2, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(23, 0, 0);

        }
        internal static void SetCasual3Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(11, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(10, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(11, 12, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(10, 2, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(26, 9, 0);
        }
        internal static void SetCasual4Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(22, 11, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(20, 6, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(25, 4, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(4, 1, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(24, 0, 0);
        }
        internal static void SetCEO1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(82, 6, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(12, 3, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(0, 1, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(192, 1, 0);
        }

        internal static void SetCEO2Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(63, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(2, 13, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(139, 3, 0);
        }
        internal static void SetCEO3Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(101, 13, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(33, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(78, 2, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(57, 10, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(71, 3, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(203, 25, 0);
        }
        internal static void SetMariner1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(2, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(124, 16, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(97, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(171, 2, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(238, 1, 0);
        }

        public static void SetCasual5Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(90, 2, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(61, 5, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(208, 9, 0);
        }

        public static void SetCasual6Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(1, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(0, 10, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(12, 1, 0);
        }

        internal static void SetCasual7Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(1, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(1, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(1, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(7, 4, 0);
        }

        internal static void SetCasual8Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(1, 1, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(7, 3, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(9, 2, 0);
        }

        internal static void SetRich1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(26, 9, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(28, 4, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(54, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(10, 2, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(70, 1, 0);
        }

        internal static void SetCasual9Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(41, 1, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(46, 1, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(35, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(112, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(97, 0, 0);
        }
        internal static void SetJuggernautComponent(this FreemodePed ped)
        {
            int rnd = MyRandom.Next(2, 11);
            if (ped.IsMale)
            {
                ped.Wardrobe[PedComponentType.Mask] = new PedComponent(91, rnd, 0);
                ped.Wardrobe[PedComponentType.Torso] = new PedComponent(42, 0, 0);
                ped.Wardrobe[PedComponentType.Leg] = new PedComponent(84, rnd, 0);
                ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(33, 0, 0);
                ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(97, rnd, 0);
                ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.Tops] = new PedComponent(186, rnd, 0);
            }
            else if (ped.IsFemale)
            {
                ped.Wardrobe[PedComponentType.Mask] = new PedComponent(91, rnd, 0);
                ped.Wardrobe[PedComponentType.Torso] = new PedComponent(49, 0, 0);
                ped.Wardrobe[PedComponentType.Leg] = new PedComponent(86, rnd, 0);
                ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(1, 16, 0);
                ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(105, rnd, 0);
                ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
                ped.Wardrobe[PedComponentType.Tops] = new PedComponent(188, rnd, 0);
            }
        }
        internal static void SetCasual10Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponent(11, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponent(1, 11, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponent(38, 1, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponent(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponent(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponent(135, 0, 0);
        }

    }
}
