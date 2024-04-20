using System;
using System.Collections.Generic;

namespace ProjectTrotter.MyPed
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
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(0, 5, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(1, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(17, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(33, 0, 0);
        }
        internal static void SetCasual2Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(23, 8, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(20, 8, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(12, 2, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(10, 2, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(23, 0, 0);

        }
        internal static void SetCasual3Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(11, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(10, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(11, 12, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(10, 2, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(26, 9, 0);
        }
        internal static void SetCasual4Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(22, 11, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(20, 6, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(25, 4, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(4, 1, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(24, 0, 0);
        }
        internal static void SetCEO1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(82, 6, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(12, 3, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(0, 1, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(192, 1, 0);
        }

        internal static void SetCEO2Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(63, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(2, 13, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(139, 3, 0);
        }
        internal static void SetCEO3Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(101, 13, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(33, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(78, 2, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(57, 10, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(71, 3, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(203, 25, 0);
        }
        internal static void SetMariner1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(2, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(124, 16, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(97, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(171, 2, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(238, 1, 0);
        }

        public static void SetCasual5Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(90, 2, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(61, 5, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(208, 9, 0);
        }

        public static void SetCasual6Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(1, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(0, 10, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(12, 1, 0);
        }

        internal static void SetCasual7Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(1, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(1, 0, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(1, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(7, 4, 0);
        }

        internal static void SetCasual8Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(1, 1, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(7, 3, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(9, 2, 0);
        }

        internal static void SetRich1Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(4, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(26, 9, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(28, 4, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(54, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(10, 2, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(70, 1, 0);
        }

        internal static void SetCasual9Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(41, 1, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(46, 1, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(35, 0, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(112, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(97, 0, 0);
        }
        internal static void SetJuggernautComponent(this FreemodePed ped)
        {
            int rnd = MyRandom.Next(2, 11);
            if (ped.IsMale)
            {
                ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(91, rnd, 0);
                ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(42, 0, 0);
                ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(84, rnd, 0);
                ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(33, 0, 0);
                ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(97, rnd, 0);
                ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(186, rnd, 0);
            }
            else if (ped.IsFemale)
            {
                ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(91, rnd, 0);
                ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(49, 0, 0);
                ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(86, rnd, 0);
                ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(1, 16, 0);
                ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(105, rnd, 0);
                ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
                ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(188, rnd, 0);
            }
        }
        internal static void SetCasual10Component(this FreemodePed ped)
        {
            if (!ped.IsMale) return;
            ped.Wardrobe[PedComponentType.Mask] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Torso] = new PedComponentVariation(11, 0, 0);
            ped.Wardrobe[PedComponentType.Leg] = new PedComponentVariation(1, 11, 0);
            ped.Wardrobe[PedComponentType.Parachute] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Shoes] = new PedComponentVariation(38, 1, 0);
            ped.Wardrobe[PedComponentType.Accessories] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.UnderShirt] = new PedComponentVariation(15, 0, 0);
            ped.Wardrobe[PedComponentType.BodyArmor] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Decal] = new PedComponentVariation(0, 0, 0);
            ped.Wardrobe[PedComponentType.Tops] = new PedComponentVariation(135, 0, 0);
        }

    }
}
