using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using static Rage.Native.NativeFunction;

namespace BarbarianCall.Types
{
    public sealed class SynchronizedScene : IHandleable, IDeletable, ISpatial
    {
        public PoolHandle Handle { get; private set; }
        private uint HandleValue => Handle;
        public float Phase
        {
            get => Natives.GET_SYNCHRONIZED_SCENE_PHASE<float>(HandleValue);
            set => Natives.SET_SYNCHRONIZED_SCENE_PHASE(HandleValue, value);
        }
        public float Rate
        {
            get => Natives.GET_SYNCHRONIZED_SCENE_RATE<float>(HandleValue);
            set => Natives.SET_SYNCHRONIZED_SCENE_RATE(HandleValue, value);
        }
        public bool Looped
        {
            get => Natives.IS_SYNCHRONIZED_SCENE_LOOPED<bool>(HandleValue);
            set => Natives.SET_SYNCHRONIZED_SCENE_LOOPED(HandleValue, value);
        }
        public bool HoldLastFrame
        {
            get => Natives.IS_SYNCHRONIZED_SCENE_HOLD_LAST_FRAME<bool>(HandleValue);
            set => Natives.SET_SYNCHRONIZED_SCENE_HOLD_LAST_FRAME(HandleValue, value);
        }
        public bool IsAttached { get; private set; }

        public Vector3 Position { get; set; }
        public Rotator Rotation { get; set; }

        public void AttachToEntity(Entity entity, int entityBoneIndex)
        {
            IsAttached = true;
        }
        public void Detach()
        {
            IsAttached = false;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public float DistanceTo(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo2D(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float DistanceTo2D(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IHandleable other)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public float TravelDistanceTo(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public float TravelDistanceTo(ISpatial spatialObject)
        {
            throw new NotImplementedException();
        }
    }
}
