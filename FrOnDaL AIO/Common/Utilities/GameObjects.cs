using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using static FrOnDaL_AIO.Common.Utilities.spellMisc;
using Color = System.Drawing.Color;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI.Values;
using System.Text.RegularExpressions;

namespace FrOnDaL_AIO.Common.Utilities
{
    public static class GameObjects
    {
        private static readonly List<AIHeroClient> AllyHeroesList = new List<AIHeroClient>();

        private static readonly List<AIBaseClient> AllyList = new List<AIBaseClient>();

        private static readonly List<AIMinionClient> AllyMinionsList = new List<AIMinionClient>();

        private static readonly List<AITurretClient> AllyTurretsList = new List<AITurretClient>();

        private static readonly List<AIMinionClient> AllyWardsList = new List<AIMinionClient>();

        private static readonly List<AttackableUnit> AttackableUnitsList = new List<AttackableUnit>();

        private static readonly List<AIHeroClient> EnemyHeroesList = new List<AIHeroClient>();

        private static readonly List<AIBaseClient> EnemyList = new List<AIBaseClient>();

        private static readonly List<AIMinionClient> EnemyMinionsList = new List<AIMinionClient>();

        private static readonly List<AITurretClient> EnemyTurretsList = new List<AITurretClient>();

        private static readonly List<AIMinionClient> EnemyWardsList = new List<AIMinionClient>();

        private static readonly List<GameObject> GameObjectsList = new List<GameObject>();

        private static readonly List<AIHeroClient> HeroesList = new List<AIHeroClient>();

        private static readonly List<AIMinionClient> JungleLargeList = new List<AIMinionClient>();

        private static readonly List<AIMinionClient> JungleLegendaryList = new List<AIMinionClient>();

        private static readonly List<AIMinionClient> JungleList = new List<AIMinionClient>();

        private static readonly List<AIMinionClient> JungleSmallList = new List<AIMinionClient>();

        private static readonly string[] LargeNameRegex =
        {
            "SRU_Murkwolf[0-9.]{1,}", "SRU_Gromp", "SRU_Blue[0-9.]{1,}",
            "SRU_Razorbeak[0-9.]{1,}", "SRU_Red[0-9.]{1,}",
            "SRU_Krug[0-9]{1,}"
        };

        private static readonly string[] LegendaryNameRegex = { "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        private static readonly List<AIMinionClient> MinionsList = new List<AIMinionClient>();

        private static readonly string[] SmallNameRegex = { "SRU_[a-zA-Z](.*?)Mini", "Sru_Crab" };

        private static readonly List<AITurretClient> TurretsList = new List<AITurretClient>();

        private static readonly List<AIMinionClient> WardsList = new List<AIMinionClient>();

        private static bool _initialized;
        static GameObjects()
        {
            Initialize();
        }

        public enum JungleType
        {
            Unknown,
            Small,
            Large,
            Legendary
        }
        public static IEnumerable<GameObject> AllGameObjects => GameObjectsList;

        public static IEnumerable<AIBaseClient> Ally => AllyList;
        public static IEnumerable<AIHeroClient> AllyHeroes => AllyHeroesList;

        public static IEnumerable<AIMinionClient> AllyMinions => AllyMinionsList;
        public static IEnumerable<AITurretClient> AllyTurrets => AllyTurretsList;
        public static IEnumerable<AIMinionClient> AllyWards => AllyWardsList;

        public static IEnumerable<AttackableUnit> AttackableUnits => AttackableUnitsList;

        public static IEnumerable<AIBaseClient> Enemy => EnemyList;

        public static IEnumerable<AIHeroClient> EnemyHeroes => EnemyHeroesList;

        public static IEnumerable<AIMinionClient> EnemyMinions => EnemyMinionsList;

        public static IEnumerable<AITurretClient> EnemyTurrets => EnemyTurretsList;

        public static IEnumerable<AIMinionClient> EnemyWards => EnemyWardsList;

        public static IEnumerable<AIHeroClient> Heroes => HeroesList;

        public static IEnumerable<AIMinionClient> Jungle => JungleList;

        public static IEnumerable<AIMinionClient> JungleLarge => JungleLargeList;

        public static IEnumerable<AIMinionClient> JungleLegendary => JungleLegendaryList;

        public static IEnumerable<AIMinionClient> JungleSmall => JungleSmallList;

        public static IEnumerable<AIMinionClient> Minions => MinionsList;

        public static AIHeroClient Player { get; set; }

        public static IEnumerable<AITurretClient> Turrets => TurretsList;

        public static IEnumerable<AIMinionClient> Wards => WardsList;

        public static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return gameObject != null && gameObject.IsValid && @object != null && @object.IsValid
                   && gameObject.NetworkId == @object.NetworkId;
        }

        public static IEnumerable<T> Get<T>()
            where T : GameObject, new()
        {
            return AllGameObjects.OfType<T>();
        }

        public static JungleType GetJungleType(this AIMinionClient minion)
        {
            if (SmallNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Small;
            }

            if (LargeNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Large;
            }

            if (LegendaryNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Legendary;
            }

            return JungleType.Unknown;
        }

        public static IEnumerable<T> GetNative<T>()
            where T : GameObject, new()
        {
            return ObjectManager.Get<T>();
        }

        internal static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            Player = spellMisc.Gamer;

            HeroesList.AddRange(ObjectManager.Get<AIHeroClient>());
            MinionsList.AddRange(ObjectManager.Get<AIMinionClient>().Where(o => o.Team != GameObjectTeam.Neutral && !o.Name.Contains("ward")));
            TurretsList.AddRange(ObjectManager.Get<AITurretClient>());
            JungleList.AddRange(ObjectManager.Get<AIMinionClient>().Where(o => o.Team == GameObjectTeam.Neutral && o.Name != "WardCorpse" && o.Name != "Barrel" && !o.Name.Contains("SRU_Plant_")));
            WardsList.AddRange(ObjectManager.Get<AIMinionClient>().Where(o => o.Name.Contains("ward")));

            GameObjectsList.AddRange(ObjectManager.Get<GameObject>());
            AttackableUnitsList.AddRange(ObjectManager.Get<AttackableUnit>());

            EnemyHeroesList.AddRange(HeroesList.Where(o => o.IsEnemy));
            EnemyMinionsList.AddRange(MinionsList.Where(o => o.IsEnemy));
            EnemyTurretsList.AddRange(TurretsList.Where(o => o.IsEnemy));
            EnemyList.AddRange(EnemyHeroesList.Cast<AIBaseClient>().Concat(EnemyMinionsList).Concat(EnemyTurretsList));

            AllyHeroesList.AddRange(HeroesList.Where(o => o.IsAlly));
            AllyMinionsList.AddRange(MinionsList.Where(o => o.IsAlly));
            AllyTurretsList.AddRange(TurretsList.Where(o => o.IsAlly));
            AllyList.AddRange(
                AllyHeroesList.Cast<AIBaseClient>().Concat(AllyMinionsList).Concat(AllyTurretsList));

            JungleSmallList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Small));
            JungleLargeList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Large));
            JungleLegendaryList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Legendary));

            AllyWardsList.AddRange(WardsList.Where(o => o.IsAlly));
            EnemyWardsList.AddRange(WardsList.Where(o => o.IsEnemy));
        }

        internal static int GetBuffCount(AIBaseClient target, string buffName)
        {
            foreach (var buff in target.Buffs.Where(buff => buff.Name.ToLower() == buffName.ToLower()))
            {
                if (buff.Count == 0)
                    return 1;
                else
                    return buff.Count;
            }
            return 0;
        }
    }
}
