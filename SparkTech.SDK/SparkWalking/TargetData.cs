namespace SparkTech.SDK.SparkWalking
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

    /// <summary>
    /// Provides informations about the target
    /// </summary>
    public class TargetData
    {
        /// <summary>
        /// The default <see cref="TargetData"/> instance
        /// </summary>
        public static TargetData Empty;

        /// <summary>
        /// The <see cref="AttackableUnit"/> instance
        /// </summary>
        public readonly AttackableUnit Target;

        /// <summary>
        /// Indicates whether the player should wait for a better target to be found
        /// </summary>
        public readonly bool ShouldWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="shouldWait">Determines whether this instance should wait</param>
        public TargetData(bool shouldWait)
        {
            this.Target = null;

            this.ShouldWait = shouldWait;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="target">The target</param>
        public TargetData(AttackableUnit target)
        {
            this.Target = target;

            this.ShouldWait = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="orderedEnumerable">The sorted targets</param>
        public TargetData(IEnumerable<AttackableUnit> orderedEnumerable) : this(orderedEnumerable.FirstOrDefault())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        private TargetData()
        {
            
        }

        /// <summary>
        /// Initializes static members of the <see cref="TargetData"/> class
        /// </summary>
        static TargetData()
        {
            Empty = new TargetData();
        }
    }
}