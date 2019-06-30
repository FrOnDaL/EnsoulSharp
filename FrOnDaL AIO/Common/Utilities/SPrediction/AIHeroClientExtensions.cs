using System;
using System.Runtime.CompilerServices;
using EnsoulSharp;

namespace FrOnDaL_AIO.Common.Utilities.SPrediction
{
    /// <summary>
    /// AIHeroClient extensions for SPrediction
    /// </summary>
    public static class AIHeroClientExtensions
    {
        /// <summary>
        /// Gets passed time without moving
        /// </summary>
        /// <param name="t">target</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MovImmobileTime(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].IsStopped ? Environment.TickCount - PathTracker.EnemyInfo[t.NetworkId].StopTick : 0;
        }

        /// <summary>
        /// Gets passed time from last movement change
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastMovChangeTime(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return Environment.TickCount - PathTracker.EnemyInfo[t.NetworkId].LastWaypointTick;
        }

        /// <summary>
        /// Gets average movement reaction time
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AvgMovChangeTime(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].AvgTick + ConfigMenu.IgnoreReactionDelay;
        }

        /// <summary>
        /// Gets average path lenght
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AvgPathLenght(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].AvgPathLenght;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">target</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LastAngleDiff(this AIHeroClient t)
        {
            Prediction.AssertInitializationMode();
            return PathTracker.EnemyInfo[t.NetworkId].LastAngleDiff;
        }
    }
}
