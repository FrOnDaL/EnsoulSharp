﻿using System.Collections.Generic;
using System.Linq;

using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;

using SharpDX;

namespace FrOnDaL_AIO.Common.Utilities.SPrediction
{
    /// <summary>
    /// Line prediction class
    /// </summary>
    public static class LinePrediction
    {
        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="input">Neccesary inputs for prediction calculations</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(Prediction.Input input)
        {
            return GetPrediction(input.Target, input.SpellWidth, input.SpellDelay, input.SpellMissileSpeed, input.SpellRange, input.SpellCollisionable, input.Path, input.AvgReactionTime, input.LastMovChangeTime, input.AvgPathLenght, input.LastAngleDiff, input.From.ToVector2(), input.RangeCheckFrom.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="collisionable">Spell collisionable</param>
        /// <param name="type">Spell skillshot type</param>
        /// <param name="from">Spell casted position</param>
        /// <returns>Prediction result as <see cref="Prediction.Result"/></returns>
        public static Prediction.Result GetPrediction(AIHeroClient target, float width, float delay, float missileSpeed, float range, bool collisionable)
        {
            return GetPrediction(target, width, delay, missileSpeed, range, collisionable, target.GetWaypoints(), target.AvgMovChangeTime(), target.LastMovChangeTime(), target.AvgPathLenght(), target.LastAngleDiff(), ObjectManager.Player.PreviousPosition.ToVector2(), ObjectManager.Player.PreviousPosition.ToVector2());
        }

        /// <summary>
        /// Gets Prediction result
        /// </summary>
        /// <param name="target">Target for spell</param>
        /// <param name="width">Spell width</param>
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
        public static Prediction.Result GetPrediction(AIBaseClient target, float width, float delay, float missileSpeed, float range, bool collisionable, List<Vector2> path, float avgt, float movt, float avgp, float anglediff, Vector2 from, Vector2 rangeCheckFrom)
        {
            return Prediction.GetPrediction(target, width, delay, missileSpeed, range, collisionable, SkillshotType.Line, path, avgt, movt, avgp, anglediff, from, rangeCheckFrom);
        }

        /// <summary>
        /// Gets Aoe Prediction result
        /// </summary>
        /// <param name="width">Spell width</param>
        /// <param name="delay">Spell delay</param>
        /// <param name="missileSpeed">Spell missile speed</param>
        /// <param name="range">Spell range</param>
        /// <param name="from">Spell casted position</param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns>Prediction result as <see cref="Prediction.AoeResult"/></returns>
        public static Prediction.AoeResult GetAoePrediction(float width, float delay, float missileSpeed, float range, Vector2 from, Vector2 rangeCheckFrom)
        {
            var result = new Prediction.AoeResult();
            var enemies = GameObjects.EnemyHeroes.Where(p => p.IsValidTarget() && Prediction.GetFastUnitPosition(p, delay, 0, from).Distance(rangeCheckFrom) < range);

            foreach (var enemy in enemies)
            {
                Prediction.Result prediction = GetPrediction(enemy, width, delay, missileSpeed, range, false, enemy.GetWaypoints(), enemy.AvgMovChangeTime(), enemy.LastMovChangeTime(), enemy.AvgPathLenght(), enemy.LastAngleDiff(), from, rangeCheckFrom);
                if (prediction.HitChance > HitChance.Medium)
                {
                    Vector2 to = from + (prediction.CastPosition - from).Normalized() * range;
                    Collision.Result colResult = Collision.GetCollisions(from, to, range, width, delay, missileSpeed, false);
                    if (colResult.Objects.HasFlag(Collision.Flags.EnemyChampions))
                    {
                        int collisionCount = colResult.Units.Count(p => p.IsEnemy && p.Type == GameObjectType.AIHeroClient && p.IsValid);
                        if (collisionCount > result.HitCount)
                            result = prediction.ToAoeResult(collisionCount, colResult);
                    }
                }
            }
            return result;
        }
    }
}
