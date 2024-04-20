namespace ProjectTrotter.Types
{
    //Source: https://github.com/pnwparksfan/rph-live-lights/blob/3c36c4d1603e9a3fe03ff1bb69ac5de2dfce2759/LiveLights/Utils/Marker.cs
    using Rage;
    using Rage.Native;
    using System.Drawing;
    internal class Marker
    {
        public Color Color { get; set; } = Color.Blue;
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);
        public Rotator Rotation { get; set; } = Rotator.Zero;
        public MarkerType Style { get; set; } = MarkerType.UpsideDownCone;

        public Marker(Vector3 pos)
        {
            Position = pos;
            Direction = pos;
        }

        public Marker(Vector3 pos, Color color, MarkerType type = MarkerType.UpsideDownCone)
        {
            Style = type;
            Position = pos;
            Direction = pos;
            Color = color;
        }

        public Marker(Vector3 position, Vector3 direction, Vector3 scale, Rotator rotation, Color color, MarkerType style)
        {
            Position = position;
            Direction = direction;
            Scale = scale;
            Rotation = rotation;
            Color = Color;
            Style = style;
        }

        /// <summary>
        /// Must be called on each tick, e.g. in a GameFiber or FrameRender
        /// </summary>
        public void Draw() => DrawMarker(Style, Position, Direction, Rotation, Scale, Color);

        public static void DrawMarker(MarkerType type, Vector3 position, Vector3 scale, Color color) => DrawMarker(type, position, Vector3.Zero, Rotator.Zero, scale, color);

        public static void DrawMarker(MarkerType type, Vector3 position, Color color) => DrawMarker(type, position, Vector3.Zero, Rotator.Zero, new Vector3(1, 1, 1), color);

        public static void DrawMarker(Entity entity, MarkerType type, Color color, Vector3 scale, float extraZ)
        {
            if (!entity) return;

            DrawMarker(type, entity.AbovePosition + Vector3.WorldUp * ((scale.Z * 0.5f) + extraZ), scale, color);
        }

        public static void DrawMarker(MarkerType type, Vector3 position, Vector3 direction, Rotator rotation, Vector3 scale, Color color, bool bobUpAndDown = false)
        {

            if (Game.IsPaused || Game.Console.IsOpen || Game.IsScreenFadedOut)
            {
                return;
            }

            NativeFunction.Natives.DRAW_MARKER(
                    (int)type,
                    position.X,
                    position.Y,
                    position.Z,
                    direction.X,
                    direction.Y,
                    direction.Z,
                    rotation,
                    scale.X,
                    scale.Y,
                    scale.Z,
                    (int)color.R,
                    (int)color.G,
                    (int)color.B,
                    (int)color.A,
                    bobUpAndDown, 
                    true,
                    2, 
                    false,
                    0,
                    0,
                    false);
        }
    }
}
