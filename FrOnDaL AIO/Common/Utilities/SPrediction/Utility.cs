using System.Linq;

using EnsoulSharp;

using SharpDX;

namespace FrOnDaL_AIO.Common.Utilities.SPrediction
{
    /// <summary>
    /// Necessary utilities for SPrediction
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Checks if the given bufftype is immobilizer 
        /// </summary>
        /// <param name="type">Buff type</param>
        /// <returns></returns>
        internal static bool IsImmobilizeBuff(BuffType type)
        {
            return type == BuffType.Snare || type == BuffType.Stun || type == BuffType.Asleep || type == BuffType.Knockup || type == BuffType.Suppression;
        }

        /// <summary>
        /// Checks if the given target is immobile
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        internal static bool IsImmobileTarget(AIBaseClient target)
        {
            return target.Buffs.Any(p => IsImmobilizeBuff(p.Type));
        }

        /// <summary>
        /// Gets left immobile time of given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        internal static float LeftImmobileTime(AIBaseClient target)
        {
            return target.Buffs.Where(p => p.IsActive && IsImmobilizeBuff(p.Type) && p.EndTime > Game.Time).Max(q => q.EndTime - Game.Time);
        }

        /// <summary>
        /// Converts <see cref="Prediction.Result"/> to <see cref="Prediction.Vector.Result"/> with given from position
        /// </summary>
        /// <param name="pResult">Prediction result as <see cref="Prediction.Result"/></param>
        /// <param name="from">Vector source position</param>
        /// <returns>Converted prediction result as <see cref="Prediction.Vector.Result"/></returns>
        internal static VectorPrediction.Result AsVectorResult(this Prediction.Result pResult, Vector2 from)
        {
            return new VectorPrediction.Result
            {
                CastSourcePosition = from,
                CastTargetPosition = pResult.CastPosition,
                UnitPosition = pResult.UnitPosition,
                HitChance = pResult.HitChance,
                CollisionResult = pResult.CollisionResult,
            };
        }

        /// <summary>
        /// Converts <see cref="Prediction.Result"/> to <see cref="Prediction.AoeResult"/> with given hit count and collision result
        /// </summary>
        /// <param name="pResult">Prediction result as <see cref="Prediction.Result"/></param>
        /// <param name="hitCount">Aoe hits</param>
        /// <param name="colResult">Aoe collision result</param>
        /// <returns>Converted prediction result as <see cref="Prediction.AoeResult"/></returns>
        internal static Prediction.AoeResult ToAoeResult(this Prediction.Result pResult, int hitCount, Collision.Result colResult)
        {
            return new Prediction.AoeResult
            {
                CastPosition = pResult.CastPosition,
                HitCount = hitCount,
                CollisionResult = colResult
            };
        }
    }
}
