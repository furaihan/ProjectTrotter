using System.Collections.Generic;
using Rage.Native;

namespace BarbarianCall.MyPed
{
    public class PedComponentCollection
    {
        private readonly MyPed _owner;
        private readonly Dictionary<PedComponentType, PedComponent> _components;
        internal PedComponentCollection(MyPed owner)
        {
            _owner = owner;
        }
        internal PedComponent this[PedComponentType componentType]
        {
            get
            {
                if (_components.TryGetValue(componentType, out var component))
                    return component;

                int drawable = NativeFunction.Natives.GET_PED_DRAWABLE_VARIATION<int>(_owner, componentType);
                int texture = NativeFunction.Natives.GET_PED_TEXTURE_VARIATION<int>(_owner, componentType);
                int pallete = NativeFunction.Natives.GET_PED_PALETTE_VARIATION<int>(_owner, componentType);
                component = new PedComponent(drawable, texture, pallete);
                _components[componentType] = component;
                return component;
            }
            set
            {
                if (!ValidateDrawable(componentType, value) || !ValidateTexture(componentType, value)) return;
                _components[componentType] = value;
                NativeFunction.Natives.SET_PED_COMPONENT_VARIATION(_owner, componentType, value.DrawableID, value.TextureID, value.PalleteID);
            }
        }
        private bool ValidateDrawable(PedComponentType type, PedComponent pedComponent)
        {
            int max = NativeFunction.Natives.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS<int>(_owner, type);
            if (pedComponent.DrawableID < 0 || pedComponent.DrawableID >= max)
            {
                Logger.Log($"DrawableID {pedComponent.DrawableID} is out of range for {type}");
                return false;
            }
            return true;
        }
        private bool ValidateTexture(PedComponentType type, PedComponent pedComponent)
        {
            int max = NativeFunction.Natives.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS<int>(_owner, type, pedComponent.DrawableID);
            if (pedComponent.TextureID < 0 || pedComponent.TextureID >= max)
            {
                Logger.Log($"TextureID {pedComponent.TextureID} is out of range for {type}");
                return false;
            }
            return true;
        }
    }
}
