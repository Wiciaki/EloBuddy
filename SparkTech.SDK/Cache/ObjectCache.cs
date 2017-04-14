namespace SparkTech.SDK.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using EloBuddy;

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
        #region Static Fields

        /// <summary>
        ///     Gets the <see cref="E:Player" />'s  instance
        /// </summary>
        public static AIHeroClient Player => ObjectManager.Player;

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
        ///     Contains all the <see cref="Obj_AI_Marker" /> instances
        /// </summary>
        private static List<Obj_AnimatedBuilding> Obj_AI_MarkerList;

        /// <summary>
        ///     Contains all the <see cref="AIHeroClient" /> instances
        /// </summary>
        private static List<AIHeroClient> AIHeroClientList;

#pragma warning restore 649
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

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the GameObjects of the specified type and team
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <param name="team">The specified object team</param>
        /// <param name="predicate">The function determining whether this instance is eligible</param>
        /// <returns></returns>
        public static List<TGameObject> Get<TGameObject>(ObjectTeam team = ObjectTeam.Ally | ObjectTeam.Enemy | ObjectTeam.Neutral, Predicate<TGameObject> predicate = null) where TGameObject : GameObject
        {
            var name = typeof(TGameObject).Name + "List";

            FieldInfo field;

            // Found it cached
            if (FieldData.TryGetValue(name, out field))
            {
                return Selector((List<TGameObject>)field.GetValue(null), team, predicate);
            }

            var container = GameObjectList.ConvertAll(o => o as TGameObject).FindAll(o => o != null);
            field = GetField(name);

            // This type of GameObject is supported however cache is being just initialized
            if (field != null)
            {
                field.SetValue(null, container);
                FieldData.Add(name, field);
            }

            return Selector(container, team, predicate);
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
            var container = new List<Obj_AI_Minion>();

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

            return Selector(container, team, inrange);
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
            var container = new List<Obj_AI_Base>();

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

            if (from.IsZero)
                from = Player.ServerPosition.ToVector2();

            range *= range;

            if (float.IsPositiveInfinity(range))
            {
                return Selector(container, team, null);
            }

            return Selector(container, team, @base => Vector2.DistanceSquared(from , @base.ServerPosition.ToVector2()) <= range);
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
        ///     Returns a matched list
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <param name="container">The original list</param>
        /// <param name="flags">The provided team flags</param>
        /// <param name="predicate">The function to check if the unit meets the condition</param>
        /// <returns></returns>
        private static List<TGameObject> Selector<TGameObject>(List<TGameObject> container, ObjectTeam flags, Predicate<TGameObject> predicate) where TGameObject : GameObject
        {
            return container.FindAll(o =>
                {
                    var team = o.Team();

                    if ((flags & team) == 0)
                        return false;

                    return predicate == null || predicate(o);
                });
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
            var engine = new ProcessingEngine(@object, @new);

            engine.Proc(GameObjectList);

            if (!engine.Proc(AttackableUnitList))
            {
                if (engine.Proc(Obj_GeneralParticleEmitterList))
                { }
                else if (engine.Proc(MissileClientList))
                { }

                return;
            }

            if (!engine.Proc(Obj_AI_BaseList))
            {
                if (engine.Proc(Obj_AnimatedBuildingList))
                {
                    if (engine.Proc(Obj_BarracksDampenerList))
                    { }
                    else if (engine.Proc(Obj_HQList))
                    { }
                }

                return;
            }

            if (engine.Proc(Obj_AI_MinionList))
            {
                var type = ((Obj_AI_Minion)@object).DetermineType();

                engine.Proc(
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
            else if (engine.Proc(Obj_AI_MarkerList))
            { }
            else if (engine.Proc(AIHeroClientList))
            { }
            else if (engine.Proc(Obj_AI_TurretList))
            { }
            else if (engine.Proc(Obj_AI_MarkerList))
            { }
            else
            {
                Log.Verbose("Unhandled type in cache: " + @object.GetType().Name);
            }

            /*
             
            bool Proc<T>(ICollection<T> a) where T: GameObject 
            {
                return false;
            }

            */
        }

        #endregion

        private class ProcessingEngine
        {
            internal ProcessingEngine(GameObject @object, bool @new)
            {
                this.@object = @object;

                this.@new = @new;
            }

            private readonly GameObject @object;

            private readonly bool @new;

            internal bool Proc<TGameObject>(ICollection<TGameObject> list) where TGameObject : GameObject
            {
                var o = this.@object as TGameObject;

                if (o == null)
                {
                    return false;
                }

                if (list != null)
                {
                    if (this.@new)
                    {
                        list.Add(o);
                    }
                    else
                    {
                        list.Remove(o);
                    }
                }

                return true;
            }
        }
    }
}