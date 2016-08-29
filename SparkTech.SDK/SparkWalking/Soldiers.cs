/*namespace SparkTech.SparkWalking
{
    using System;
    using System.Collections.Generic;

    using SharpDX;

    using SparkTech.Cache;

    /// <summary>
    ///     Tracks the soldiers if the used champion is <see cref="E:Azir" />
    /// </summary>
    public static class Soldiers
    {
        /// <summary>
        /// Invokes everytime a soldier has been spawned
        /// </summary>
        public static event SoldierSpawnEventHandler Spawn;

        #region Constants

        /// <summary>
        /// The full attack range of the soldier
        /// </summary>
        public const int WarriorRange = 350;

        /// <summary>
        /// The full attack range of the soldier, squared
        /// </summary>
        public const int WarriorRangeSqr = WarriorRange * WarriorRange;

        /// <summary>
        /// The possible range of W, squared
        /// </summary>
        public const int PossibleRangeSqr = 640000;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Soldiers" /> class
        /// </summary>
        static Soldiers()
        {
            if (Core.ChampionName != "Azir")
            {
                All = new List<Obj_AI_Minion>(0);
                return;
            }

            All = new List<Obj_AI_Minion>(3);

            var process = false;

            GameObject.OnCreate += (sender, args) =>
                {
                    if (!process || !sender.Name.Equals("azirsoldier", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    process = false;

                    var soldier = (Obj_AI_Minion)sender;

                    All.Add(soldier);
                    Spawn?.Invoke(soldier);
                };

            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                {
                    if (sender.IsMe && string.Equals(args.SData.Name, "azirw", StringComparison.OrdinalIgnoreCase))
                    {
                        process = true;
                    }
                };

            Obj_AI_Base.OnPlayAnimation += (sender, args) =>
                {
                    if (!sender.Name.Equals("azirsoldier", StringComparison.OrdinalIgnoreCase) || args.Animation != "Death")
                    {
                        return;
                    }

                    var index = All.FindIndex(soldier => soldier.NetworkId == sender.NetworkId);

                    if (index >= 0)
                    {
                        All.RemoveAt(index);
                    }
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the soldiers owned by the <see cref="E:Player" />
        /// </summary>
        public static List<Obj_AI_Minion> All { get; }

        /// <summary>
        ///     Gets a list of soldiers that are prepared to strike
        /// </summary>
        public static List<Obj_AI_Minion> Ready
        {
            get
            {
                var playerPos = ObjectCache.Player.ServerPosition.ToVector2();

                return All.FindAll(soldier => !soldier.IsMoving && Vector2.DistanceSquared(playerPos, soldier.ServerPosition.ToVector2()) <= 640000F);
            }
        }

        #endregion

        #region Extensions

        public static List<Obj_AI_Minion> GetSoldiers(this Obj_AI_Base @base)
        {
            // Assumptions for the player to attack a target using soldiers:
            // Player is Azir
            // The target can be attacked
            // Target is a hero, a lane or a jungle minion
            // Target is no more than 1000 units from player
            // Target is no more than 350 units from the soldier
            // Soldier is no more than 800 units from player
            // Soldier isn't moving
            
            if (Core.ChampionName != "Azir" || !@base.IsValidTarget())
            {
                return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
            }

            var position = @base.ServerPosition.ToVector2();

            if (Vector2.DistanceSquared(ObjectCache.Player.ServerPosition.ToVector2(), position) > 1000000F)
            {
                return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
            }

            var minion = @base as Obj_AI_Minion;

            if (minion != null)
            {
                var type = minion.GetMinionType();

                if (type.HasFlag(MinionTypes.Ward) || type.HasFlag(MinionTypes.Unknown) && minion.GetJungleType() == JungleType.Unknown)
                {
                    return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
                }
            }
            else if (!(@base is Obj_AI_Hero))
            {
                return MiscallenousCache.GetEmptyList<Obj_AI_Minion>();
            }

            return Ready.FindAll(soldier => Vector2.DistanceSquared(position, soldier.ServerPosition.ToVector2()) <= WarriorRangeSqr);
        }

        #endregion
    }
}*/