using BarbarianCall.Extensions;
using System.Drawing;
using Rage;
using N = Rage.Native.NativeFunction;
using System;

namespace BarbarianCall.Types
{
    public class Checkpoint: IHandleable, ISpatial, IDeletable, IEquatable<Checkpoint>
    {		
		public PoolHandle Handle { get; protected set; }
        private bool _valid = false;
        private Color _color;
        public Color Color 
        {
            get 
            {
                return _color;
            }
            set
            {
                if (!_valid)
                    return;
                uint handle = Handle;
                N.Natives.SET_CHECKPOINT_RGBA(handle, value.R, value.G, value.B, value.A);
				_color = value;
            }
        }
		private Vector3 _position;
        public Vector3 Position 
		{
            get
            {
				return _position;
            }
			set
            {
				_position = value;
				ReCreate();
            }
		}
        public Vector3 NextPosition { get; private set; }
        public float Scale 
        { 
            set
            {
                if (!_valid) return;
                uint handle = Handle;
                N.Natives.SET_CHECKPOINT_INSIDE_CYLINDER_HEIGHT_SCALE(handle, value);
                Scale = value;
            } 
        }
        private float _radius;
        public float Radius 
        {
            get
            {
                return _radius;
            }
            set
            {
                if (!_valid)
                    return;
                uint handle = Handle;
                N.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(handle, Height, Height, value);
                _radius = value;
            }
        }
        private float _height;
        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (!_valid) return;
                uint handle = Handle;
                N.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(handle, value, value, Radius);
                _height = value;
            }
        }
        private CheckpointIcon _icon;
		public CheckpointIcon Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (!_valid) return;
                _icon = value;
                ReCreate();
            }
        }
		private Color _iconColor;
		public Color IconColor 
		{ 
			get
            {
				return _iconColor;
            } 
            set
            {
                if (!_valid) return;
				uint handle = Handle;
				N.Natives.SET_CHECKPOINT_RGBA2(handle, value.R, value.G, value.B, value.A);
				_iconColor = value;
			}
		}
        public Checkpoint(CheckpointIcon icon, Vector3 position, Vector3 nextPosition, float radius, float height, Color color, Color iconColor, bool placeOnGround = true)
        {
            _icon = icon;
            Vector3 pos = placeOnGround ? position.ToGround() : position;
            _position = pos;
            NextPosition = nextPosition;
            _radius = radius;
            _height = height;
            _color = color;
            _iconColor = iconColor;
            Create();
        }
        public Checkpoint(CheckpointIcon icon, Vector3 position, float radius, float height, Color color, Color iconColor, bool placeOnGround = true)
        {
            _icon = icon;
            Vector3 pos = placeOnGround ? position.ToGround() : position;
            _position = pos;
            NextPosition = pos;
            _radius = radius;
            _height = height;
            _color = color;
            _iconColor = iconColor;
            Create();
        }
        private void Create()
        {
            try
            {
                uint handle = (uint)N.Natives.CREATE_CHECKPOINT<int>((int)Icon, Position.X, Position.Y, Position.Z, NextPosition.X, NextPosition.Y, NextPosition.Z, Radius, Color.R, Color.G, Color.B, Color.A, 0);
                Handle = new PoolHandle(handle);
                //Game.LogTrivialDebug("After assigning handle");
                N.Natives.SET_CHECKPOINT_RGBA2(handle, IconColor.R, IconColor.G, IconColor.B, IconColor.A);
                //Game.LogTrivialDebug("After set checkpoint icon color");
                N.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(handle, Height, Height, Radius);
                _valid = true;
            }
            catch (Exception e)
            {
                Logger.ToLog("Failed to create checkpoint");
                e.ToString().ToLog();
            }          
        }
		void ReCreate()
        {
            Delete();
            Create();
        }

        public bool IsValid()
        {
            if (this == null) return false;
            if (Handle.IsZero) return false;
            return _valid;
        }

        public bool Equals(IHandleable other)
        {
            if (other is Checkpoint && IsValid() && other.IsValid()) return Handle.Value == other.Handle.Value;
            return false;
        }

        public float DistanceTo(Vector3 position) => Position.DistanceTo(position);

        public float DistanceTo(ISpatial spatialObject) => Position.DistanceTo(spatialObject.Position);

        public float DistanceTo2D(Vector3 position) => Position.DistanceTo2D(position);

        public float DistanceTo2D(ISpatial spatialObject) => Position.DistanceTo2D(spatialObject);

        public float TravelDistanceTo(Vector3 position) => Position.TravelDistanceTo(position);

        public float TravelDistanceTo(ISpatial spatialObject) => Position.TravelDistanceTo(spatialObject);

        public void Delete()
        {
            if (_valid)
            {
                uint handle = Handle;
                N.Natives.DELETE_CHECKPOINT(handle);
                _valid = false;
            }
            else throw new Rage.Exceptions.InvalidHandleableException($"Operation is not valid because the specified {nameof(Checkpoint)} is invalid.");
        }

        public bool Equals(Checkpoint other)
        {
            return ReferenceEquals(other, null) && Handle == other.Handle;
        }

        public static implicit operator bool(Checkpoint value)
        {
            if (value is null) return false;
            if (value.Handle.IsZero) return false;
            return value.IsValid();
        }       
    }
}
