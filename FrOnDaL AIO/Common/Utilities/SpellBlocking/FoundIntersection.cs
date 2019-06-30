using EnsoulSharp.SDK;
using SharpDX;

namespace FrOnDaL_AIO.Common.Utilities.SpellBlocking
{
    public struct FoundIntersection
    {
        public Vector2 ComingFrom;
        public float Distance;
        public Vector2 Point;
        public int Time;
        public bool Valid;

        public FoundIntersection(float distance, int time, Vector2 point, Vector2 comingFrom)
        {
            Distance = distance;
            ComingFrom = comingFrom;
            Valid = (point.X != 0) && (point.Y != 0);
            Point = point + EvadeManager.GridSize * (Vector2)(ComingFrom - point).ToVector3().Normalized();
            Time = time;
        }
    }
}
