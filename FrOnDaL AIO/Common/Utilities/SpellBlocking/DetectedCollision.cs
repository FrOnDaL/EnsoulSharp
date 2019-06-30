using EnsoulSharp;
using SharpDX;

namespace FrOnDaL_AIO.Common.Utilities.SpellBlocking
{
    internal class DetectedCollision
    {
        public float Diff;
        public float Distance;
        public Vector2 Position;
        public CollisionObjectTypes Type;
        public AIBaseClient Unit;
    }
}
