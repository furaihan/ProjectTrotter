﻿using BarbarianCall.Extensions;
using System.Drawing;
using Rage;
using N = Rage.Native.NativeFunction;

namespace BarbarianCall.Types
{
    public class Checkpoint: IHandleable, ISpatial, IDeletable
    {
		public enum CheckpointIcon
		{
			CylinderSingleArrow,
			CylinderDoubleArrow,
			CylinderTripleArrow,
			CylinderCycleArrow,
			CylinderCheckerboard,
			CylinderSingleArrow2,
			CylinderDoubleArrow2,
			CylinderTripleArrow2,
			CylinderCycleArrow2,
			CylinderCheckerboard2,
			RingSingleArrow,
			RingDoubleArrow,
			RingTripleArrow,
			RingCycleArrow,
			RingCheckerboard,
			SingleArrow,
			DoubleArrow,
			TripleArrow,
			CycleArrow,
			Checkerboard,
			CylinderSingleArrow3,
			CylinderDoubleArrow3,
			CylinderTripleArrow3,
			CylinderCycleArrow3,
			CylinderCheckerboard3,
			CylinderSingleArrow4,
			CylinderDoubleArrow4,
			CylinderTripleArrow4,
			CylinderCycleArrow4,
			CylinderCheckerboard4,
			CylinderSingleArrow5,
			CylinderDoubleArrow5,
			CylinderTripleArrow5,
			CylinderCycleArrow5,
			CylinderCheckerboard5,
			RingPlaneUp,
			RingPlaneLeft,
			RingPlaneRight,
			RingPlaneDown,
			Empty,
			Ring,
			Empty2,
			Cyclinder = 45,
			Cyclinder2,
			Cyclinder3,
		}
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
                N.Natives.x4B5B4DA5D79F1943(handle, value);
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
            uint handle = N.Natives.CREATE_CHECKPOINT((int)Icon, Position.X, Position.Y, Position.Z, NextPosition.X, NextPosition.Y, NextPosition.Z, Radius, Color.R, Color.G, Color.B, Color.A, 0);
            Handle = new PoolHandle(handle);
            N.Natives.SET_CHECKPOINT_RGBA2(handle, IconColor.R, IconColor.G, IconColor.B, IconColor.A);
            N.Natives.SET_CHECKPOINT_CYLINDER_HEIGHT(handle, Height, Height, Radius);
            _valid = true;
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
            try
            {
                uint handle = other.Handle;
                bool? isEntity = N.Natives.DOES_ENTITY_EXIST<bool>(handle);
                if (isEntity.HasValue && isEntity.Value) return false;
            }
            catch { }
            return other.Handle == Handle;
        }

        public float DistanceTo(Vector3 position) => Position.DistanceTo(position);

        public float DistanceTo(ISpatial spatialObject) => Position.DistanceTo(spatialObject);

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
        }
        public static implicit operator bool(Checkpoint value)
        {
            if (value is null) return false;
            if (value.Handle.IsZero) return false;
            return value.IsValid();
        }
    }
}