using System.Collections.Generic;
using Rage.Native;

namespace BarbarianCall.MyPed
{
    public class PedWardrobe
    {
        private readonly MyPed _owner;
        private readonly Dictionary<PedComponentType, PedComponentVariation> _components;
        internal PedWardrobe(MyPed owner)
        {
            _owner = owner;
        }
        // Outdated
        internal PedComponentVariation this[PedComponentType componentType]
        {
            get => GetPedComponentVariation(componentType);
            set => SetPedComponentVariation(componentType, value);
        }
        internal PedComponentVariation GetPedComponentVariation(PedComponentType componentType)
        {
            if (_components.TryGetValue(componentType, out var component))
                return component;

            int drawable = NativeFunction.Natives.GET_PED_DRAWABLE_VARIATION<int>(_owner, componentType);
            int texture = NativeFunction.Natives.GET_PED_TEXTURE_VARIATION<int>(_owner, componentType);
            int pallete = NativeFunction.Natives.GET_PED_PALETTE_VARIATION<int>(_owner, componentType);
            component = new PedComponentVariation(drawable, texture, pallete);
            _components[componentType] = component;
            return component;
        }
        internal void SetPedComponentVariation(PedComponentType componentType, PedComponentVariation pedComponent)
        {
            if (!ValidateDrawable(componentType, pedComponent) || !ValidateTexture(componentType, pedComponent)) return;
            _components[componentType] = pedComponent;
            NativeFunction.Natives.SET_PED_COMPONENT_VARIATION(_owner, componentType, pedComponent.Drawable, pedComponent.Texture, pedComponent.Pallete);
        }
        private bool ValidateDrawable(PedComponentType type, PedComponentVariation pedComponent)
        {
            int max = NativeFunction.Natives.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS<int>(_owner, type);
            if (pedComponent.Drawable < 0 || pedComponent.Drawable >= max)
            {
                Logger.Log($"DrawableID {pedComponent.Drawable} is out of range for {type}");
                return false;
            }
            return true;
        }
        private bool ValidateTexture(PedComponentType type, PedComponentVariation pedComponent)
        {
            int max = NativeFunction.Natives.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS<int>(_owner, type, pedComponent.Drawable);
            if (pedComponent.Texture < 0 || pedComponent.Texture >= max)
            {
                Logger.Log($"TextureID {pedComponent.Texture} is out of range for {type}");
                return false;
            }
            return true;
        }
    }
}
