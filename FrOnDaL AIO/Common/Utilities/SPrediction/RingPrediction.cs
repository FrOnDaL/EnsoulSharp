using System.Collections.Generic;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;

using SharpDX;

namespace FrOnDaL_AIO.Common.Utilities.SPrediction
{
    /// <summary>
    /// Ring prediction class 
    /// </summary>
    public class RingPrediction
    {
        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(Prediction.Input input, float ringRadius)
        {
            return GetPrediction(input.Target, input.SpellWidth, ringRadius, input.SpellDelay, input.SpellMissileSpeed, input.SpellRange, input.SpellCollisionable, input.Path, input.AvgReactionTime, input.LastMovChangeTime, input.AvgPathLenght, input.From.ToVector2(), input.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="radius">Spell radius</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(AIHeroClient target, float radius, float ringRadius, float delay, float missileSpeed, float range, bool collisionable)
        {
            return GetPrediction(target, radius, ringRadius, delay, missileSpeed, range, collisionable, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), ObjectManager.Player.PreviousPosition.ToVector2(), ObjectManager.Player.PreviousPosition.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="radius">Spell radius</param>
        /// <param name="ringRadius">Ring radius</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="path">Waypoints of target</param>
        /// <param name="avgt">Average reaction time (in ms)</param>
        /// <param name="movt">Passed time from last movement change (in ms)</param>
        /// <param name="avgp">Average Path Lenght</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(AIBaseClient target, float radius, float ringRadius, float delay, float missileSpeed, float range, bool collisionable, List<Vector2> path, float avgt, float movt, float avgp, Vector2 from, Vector2 rangeCheckFrom)
        {
            //if you are copying it negro; dont forget sprediction credits, ty.
            Prediction.Result result = CirclePrediction.GetPrediction(target, ringRadius, delay, missileSpeed, range + radius, collisionable, path, avgt, movt, avgp, 360, from, rangeCheckFrom);
            if (result.HitChance > HitChance.Low)
            {
                Vector2 direction = (result.CastPosition - from + target.Direction.ToVector2()).Normalized();
                result.CastPosition -= direction * (radius - ringRadius / 2f);

                if (result.CastPosition.Distance(from) > range)
                    result.HitChance = HitChance.OutOfRange;
            }

            return result;
        }
    }
}
