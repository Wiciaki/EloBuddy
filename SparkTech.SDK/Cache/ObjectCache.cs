namespace SparkTech.SDK.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using EloBuddy;
    using EloBuddy.SDK.Utils;

    using SharpDX;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    /// <summary>
    ///     An alternative to the <see cref="ObjectManager" /> class
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "These fields won't work with any other naming")]
    [Trigger]
    public static class ObjectCache
    {
        #region Constants

        /// <summary>
        ///     Represents all the teams
        /// </summary>
        private const ObjectTeam AllTeams = ObjectTeam.Ally | ObjectTeam.Enemy | ObjectTeam.Neutral | ObjectTeam.Unknown;

        #endregion

        #region Static Fields

        /// <summary>
        ///     Saves the <see cref="E:Player" /> instance
        /// </summary>
        public static readonly AIHeroClient Player;

        /// <summary>
        ///     Contains the <see cref="FieldInfo" /> instances
        /// </summary>
        private static readonly Dictionary<string, FieldInfo> FieldData;

        /// <summary>
        ///     Contains all the <see cref="GameObject" /> instances
        /// </summary>
        private static readonly List<GameObject> GameObjectList;

        /// <summary>
        ///     Gets the list containing the jungle creeps
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleMinions;

        /// <summary>
        ///     Gets the list containing the plants
        /// </summary>
        private static readonly List<Obj_AI_Minion> Plants;

        /// <summary>
        ///     Gets the clean list containing just the minions (no ghost wards, clones or any other crap just lane minions)
        /// </summary>
        private static readonly List<Obj_AI_Minion> Minions;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Minion" /> instances
        /// </summary>
        private static readonly List<Obj_AI_Minion> Obj_AI_MinionList;

        /// <summary>
        ///     Gets the list containing the unknown minion type (e.g. trundle walls)
        /// </summary>
        private static readonly List<Obj_AI_Minion> OtherMinions;

        /// <summary>
        ///     Gets the list containing the attackable objects (e.g. Shaco's boxes)
        /// </summary>
        private static readonly List<Obj_AI_Minion> Pets;

        /// <summary>
        ///     The team relation dictionary
        /// </summary>
        private static readonly Dictionary<GameObjectTeam, ObjectTeam> TeamDictionary;

        /// <summary>
        ///     Gets the list containing just the wards
        /// </summary>
        private static readonly List<Obj_AI_Minion> Wards;

#pragma warning disable 649
#pragma warning disable 169 // The compiler just can't know we're going to use reflection!
#pragma warning disable RCS1169

        /// <summary>
        ///     Contains all the <see cref="AttackableUnit" /> instances
        /// </summary>
        private static List<AttackableUnit> AttackableUnitList;

        /// <summary>
        ///     Contains all the <see cref="MissileClient" /> instances
        /// </summary>
        private static List<MissileClient> MissileClientList;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Base" /> instances
        /// </summary>
        private static List<Obj_AI_Base> Obj_AI_BaseList;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Turret" /> instances
        /// </summary>
        private static List<Obj_AI_Turret> Obj_AI_TurretList;

        /// <summary>
        ///     Contains all the <see cref="Obj_HQ" /> instances
        /// </summary>
        private static List<Obj_HQ> Obj_HQList;

        /// <summary>
        ///     Contains all the <see cref="Obj_BarracksDampener" /> instances
        /// </summary>
        private static List<Obj_BarracksDampener> Obj_BarracksDampenerList;

        /// <summary>
        ///     Contains all the <see cref="Obj_GeneralParticleEmitter" /> instances
        /// </summary>
        private static List<Obj_GeneralParticleEmitter> Obj_GeneralParticleEmitterList;

        /// <summary>
        ///     Contains all the <see cref="Obj_AnimatedBuilding" /> instances
        /// </summary>
        private static List<Obj_AnimatedBuilding> Obj_AnimatedBuildingList;

        /// <summary>
        ///     Contains all the <see cref="AIHeroClient" /> instances
        /// </summary>
        private static List<AIHeroClient> AIHeroClientList;

#pragma warning restore 649
#pragma warning restore 169
#pragma warning restore RCS1169

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="ObjectCache" /> class
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        static ObjectCache()
        {
            GameObjectList = ObjectManager.Get<GameObject>().ToList();
            Obj_AI_MinionList = GameObjectList.OfType<Obj_AI_Minion>().ToList();

            FieldData = new Dictionary<string, FieldInfo>(10)
                        {
                            ["GameObjectList"] = GetField("GameObjectList"),
                            ["Obj_AI_MinionList"] = GetField("Obj_AI_MinionList")
            };

            Minions = new List<Obj_AI_Minion>();
            Pets = new List<Obj_AI_Minion>();
            Wards = new List<Obj_AI_Minion>();
            JungleMinions = new List<Obj_AI_Minion>();
            Plants = new List<Obj_AI_Minion>();
            OtherMinions = new List<Obj_AI_Minion>();

            foreach (var minion in Obj_AI_MinionList)
            {
                switch (minion.DetermineType())
                {
                    case AIMinionType.Normal:
                    case AIMinionType.Siege:
                    case AIMinionType.Super:
                        Minions.Add(minion);
                        break;
                    case AIMinionType.Jungle:
                    case AIMinionType.JungleBoss:
                        JungleMinions.Add(minion);
                        break;
                    case AIMinionType.Plant:
                        Plants.Add(minion);
                        break;
                    case AIMinionType.Ward:
                        Wards.Add(minion);
                        break;
                    case AIMinionType.Pet:
                        Pets.Add(minion);
                        break;
                    case AIMinionType.Unknown:
                        OtherMinions.Add(minion);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(AIMinionType));
                }
            }

            Player = Game.Mode == GameMode.Running ? (AIHeroClient)GameObjectList.Single(o => o.IsMe) : ObjectManager.Player;

            var alliedTeam = Player.Team;

            TeamDictionary = new Dictionary<GameObjectTeam, ObjectTeam>
                                 {
                                     [alliedTeam] = ObjectTeam.Ally,
                                     [alliedTeam == GameObjectTeam.Order ? GameObjectTeam.Chaos : GameObjectTeam.Order] = ObjectTeam.Enemy,
                                     [GameObjectTeam.Neutral] = ObjectTeam.Neutral,
                                     [GameObjectTeam.Unknown] = ObjectTeam.Unknown
                                 };

            GameObject.OnCreate += (sender, args) => Process(sender, true);
            GameObject.OnDelete += (sender, args) => Process(sender, false);
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The <see cref="ListAction{TItem}" /> delegate
        /// </summary>
        /// <typeparam name="TItem">The item type to take action on</typeparam>
        /// <param name="list">The <see cref="List{T}" /> to take action on</param>
        private delegate void ListAction<TItem>(List<TItem> list);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the GameObjects of the specified type and team
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <param name="team">The specified object team</param>
        /// <param name="inrange">The function determining whether this instance is in range</param>
        /// <returns></returns>
        public static List<TGameObject> Get<TGameObject>(ObjectTeam team = ObjectTeam.Ally | ObjectTeam.Enemy | ObjectTeam.Neutral, Predicate<TGameObject> inrange = null) where TGameObject : GameObject
        {
            FieldInfo field;
            var name = typeof(TGameObject).Name + "List";

            // Found it cached
            if (FieldData.TryGetValue(name, out field))
            {
                return Selector((List<TGameObject>)field.GetValue(null), team, true, true, inrange);
            }

            var container = GameObjectList.ConvertAll(o => o as TGameObject).FindAll(o => o.IsValid());
            field = GetField(name);

            // This type of GameObject is supported however cache is being just initialized
            if (field != null)
            {
                field.SetValue(null, container);
                FieldData.Add(name, field);
            }

            return Selector(container, team, true, false, inrange);
        }

        /// <summary>
        ///     Gets the GameObjects of the specified type
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <returns></returns>
        public static List<TGameObject> GetNative<TGameObject>() where TGameObject : GameObject
        {
            FieldInfo field;
            var name = typeof(TGameObject).Name + "List";

            if (FieldData.TryGetValue(name, out field))
            {
                return Selector((List<TGameObject>)field.GetValue(null), AllTeams, false, true, null);
            }

            var container = GameObjectList.ConvertAll(o => o as TGameObject).FindAll(o => o.IsValid());
            field = GetField(name);

            if (field != null)
            {
                field.SetValue(null, container);
                FieldData.Add(name, field);
            }

            return Selector(container, AllTeams, false, false, null);
        }

        /// <summary>
        ///     Gets the minions of the specified type and team
        /// </summary>
        /// <param name="type">The minion type flags</param>
        /// <param name="team">The requested team</param>
        /// <param name="inrange">The function determining whether this instance is in range</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetMinions(ObjectTeam team = ObjectTeam.Enemy | ObjectTeam.Ally, MinionType type = MinionType.Minion, Predicate<Obj_AI_Minion> inrange = null)
        {
            var container = new List<Obj_AI_Minion>(Obj_AI_MinionList.Count);

            if ((type & MinionType.Minion) != 0)
                container.AddRange(Minions);
            if ((type & MinionType.Ward) != 0)
                container.AddRange(Wards);
            if ((type & MinionType.Pet) != 0)
                container.AddRange(Pets);
            if ((type & MinionType.Plant) != 0)
                container.AddRange(Plants);
            if ((type & MinionType.Jungle) != 0)
                container.AddRange(JungleMinions);
            if ((type & MinionType.Other) != 0)
                container.AddRange(OtherMinions);

            return Selector(container, team, true, false, inrange);
        }

        /// <summary>
        ///     Gets minions in a similar way to <see cref="E:MinionManager" />
        /// </summary>
        /// <param name="from">The from</param>
        /// <param name="range">The range to take minions from</param>
        /// <param name="team">The team</param>
        /// <param name="type">The minion type</param>
        /// <returns></returns>
        public static List<Obj_AI_Base> GetMinions(Vector2 from, float range, ObjectTeam team = ObjectTeam.Enemy, MinionType type = MinionType.Minion)
        {
            var container = new List<Obj_AI_Base>(Obj_AI_MinionList.Count);

            if ((type & MinionType.Minion) != 0)
                container.AddRange(Minions);
            if ((type & MinionType.Ward) != 0)
                container.AddRange(Wards);
            if ((type & MinionType.Pet) != 0)
                container.AddRange(Pets);
            if ((type & MinionType.Plant) != 0)
                container.AddRange(Plants);
            if ((type & MinionType.Jungle) != 0)
                container.AddRange(JungleMinions);
            if ((type & MinionType.Other) != 0)
                container.AddRange(OtherMinions);

            range *= range;

            if (from.IsZero)
                from = Player.ServerPosition.ToVector2();

            var infinity = float.IsPositiveInfinity(range);

            return Selector(container, team, true, false, @base => infinity || Vector2.DistanceSquared(@base.ServerPosition.ToVector2(), from) <= range);
        }

        /// <summary>
        ///     Gets the <see cref="ObjectTeam" /> representation of the current object
        /// </summary>
        /// <param name="object">The <see cref="GameObject" /> to be inspected</param>
        /// <returns></returns>
        public static ObjectTeam Team(this GameObject @object)
        {
            return @object.Team.ToObjectTeam();
        }

        /// <summary>
        ///     Gets the <see cref="ObjectTeam" /> representation of the current object
        /// </summary>
        /// <param name="team">The <see cref="GameObjectTeam" /> instance to be converted</param>
        /// <returns></returns>
        public static ObjectTeam ToObjectTeam(this GameObjectTeam team)
        {
            return TeamDictionary[team];
        }

        /// <summary>
        ///     Determines whether a <see cref="GameObject"/> is valid
        /// </summary>
        /// <param name="object">The <see cref="GameObject" /> to be inspected</param>
        /// <returns></returns>
        public static bool IsValid(this GameObject @object)
        {
            return @object?.IsValid == true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the executor function
        /// </summary>
        /// <typeparam name="TItem">The <see cref="GameObject" /> type to take action on</typeparam>
        /// <param name="object">The sender</param>
        /// <param name="new">Determines whether to add or remove item</param>
        /// <returns></returns>
        private static ListAction<TItem> GetExecutor<TItem>(TItem @object, bool @new) where TItem : GameObject
        {
            if (@new)
            {
                return list => list?.Add(@object);
            }

            return list =>
            {
                if (list == null)
                    return;

                var searched = @object?.NetworkId;

                while (true)
                {
                    var index = list.FindIndex(item => searched.HasValue ? item?.NetworkId == searched : item == null);

                    if (index < 0)
                        break;

                    list.RemoveAt(index);
                }
            };
        }

        /// <summary>
        ///     Gets the specified field
        /// </summary>
        /// <param name="fieldName">The name of the searched field</param>
        /// <returns></returns>
        private static FieldInfo GetField(string fieldName)
        {
            return typeof(ObjectCache).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        }

        /// <summary>
        ///     Adds or removes a <see cref="GameObject" /> from the appropriate lists
        /// </summary>
        /// <param name="object">The <see cref="GameObject" /> to be processed</param>
        /// <param name="new">Determines whether to add or remove an item</param>
        private static void Process(GameObject @object, bool @new)
        {
            GetExecutor(@object, @new)(GameObjectList);

            var attackable = @object as AttackableUnit;
            if (attackable == null)
            {
                GetExecutor(@object as Obj_GeneralParticleEmitter, @new)(Obj_GeneralParticleEmitterList);
                return;
            }

            GetExecutor(attackable, @new)(AttackableUnitList);

            var @base = attackable as Obj_AI_Base;
            if (@base == null)
            {
                GetExecutor(attackable as Obj_HQ, @new)(Obj_HQList);
                GetExecutor(attackable as Obj_BarracksDampener, @new)(Obj_BarracksDampenerList);
                return;
            }

            GetExecutor(@base, @new)(Obj_AI_BaseList);
            GetExecutor(@base as AIHeroClient, @new)(AIHeroClientList);
            GetExecutor(@base as Obj_AI_Turret, @new)(Obj_AI_TurretList);

            var minion = attackable as Obj_AI_Minion;
            if (minion == null)
                return;

            GetExecutor(minion, @new)(Obj_AI_MinionList);

            if (minion.IsValid)
            {
                var type = minion.DetermineType();

                GetExecutor(minion, @new)(
                    type.IsMinion()
                        ? Minions
                        : type == AIMinionType.Ward
                        ? Wards
                        : type.IsJungle()
                        ? JungleMinions
                        : type == AIMinionType.Pet
                        ? Pets
                        : OtherMinions);
            }
            else
            {
                GetExecutor(minion, @new)(Minions);
                GetExecutor(minion, @new)(Wards);
                GetExecutor(minion, @new)(JungleMinions);
                GetExecutor(minion, @new)(Pets);
                GetExecutor(minion, @new)(OtherMinions);
            }
        }

        /// <summary>
        ///     Returns a matched list
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <param name="container">The original list</param>
        /// <param name="flags">The provided team flags</param>
        /// <param name="moreChecks">Determines whether to perform additional checks</param>
        /// <param name="validityCheck">The additional predicate</param>
        /// <param name="predicate">The function to check if the unit meets the condition</param>
        /// <returns></returns>
        private static List<TGameObject> Selector<TGameObject>(List<TGameObject> container, ObjectTeam flags, bool moreChecks, bool validityCheck, Predicate<TGameObject> predicate) where TGameObject : GameObject
        {
            return container.FindAll(o =>
                {
                    if (validityCheck && !o.IsValid())
                        return false;

                    var team = o.Team();

                    if ((flags & team) == 0)
                        return false;

                    if (!moreChecks)
                        return true;

                    if (predicate != null && !predicate(o) || !o.IsVisible || o.IsDead)
                        return false;

                    var attackable = o as AttackableUnit;

                    return attackable == null || !attackable.IsInvulnerable && !attackable.IsZombie && (team == ObjectTeam.Ally || attackable.IsTargetable || team == ObjectTeam.Unknown);
                });
        }

        #endregion
    }
}