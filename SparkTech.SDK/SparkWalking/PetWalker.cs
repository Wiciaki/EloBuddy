namespace SparkTech.SDK.SparkWalking
{
    using EloBuddy;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;

    /// <summary>
    /// The orbwalker where the <see cref="SparkWalker.Unit"/> is <see cref="ObjectManager.Player"/>'s pet
    /// </summary>
    public class PetWalker : SparkWalker
    {
        /// <summary>
        /// Gets the orbwalking unit
        /// </summary>
        public override Obj_AI_Base Unit => (Obj_AI_Base)ObjectCache.Player.Pet;

        /// <summary>
        /// Determines whether a provided <see cref="Obj_AI_Base"/> instance equals the <see cref="SparkWalker.Unit"/> of this instance.
        /// </summary>
        /// <param name="base">The provided <see cref="Obj_AI_Base"/> instance</param>
        /// <returns></returns>
        protected override bool Comparison(Obj_AI_Base @base)
        {
            var unit = this.Unit;

            return unit.IsValid() && unit.NetworkId == @base.NetworkId;
        }

        /// <summary>
        /// Gets the attack order
        /// </summary>
        protected override GameObjectOrder AttackOrder => GameObjectOrder.MovePet;

        /// <summary>
        /// Gets the move order
        /// </summary>
        protected override GameObjectOrder MoveOrder => GameObjectOrder.MovePet;

        /// <summary>
        /// Gets the stop order
        /// </summary>
        protected override GameObjectOrder StopOrder => GameObjectOrder.MovePet;

        /// <summary>
        /// Determines whether this spell is an auto-attack reset
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns></returns>
        protected override bool IsReset(GameObjectProcessSpellCastEventArgs args)
        {
            // Pets don't cast spells, do they?
            return false;
        }

        public override Mode Mode
        {
            get
            {
                return Targeting["pet.combo"] ? Mode.Combo : base.Mode;
            }
            set
            {
                base.Mode = value;
            }
        }
    }
}