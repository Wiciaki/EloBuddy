namespace SparkTech.SDK.SparkWalking
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

    /// <summary>
    /// Provides informations about the target
    /// </summary>
    public struct TargetData
    {
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
        /// Initializes a new instance of the <see cref="TargetData"/> struct
        /// </summary>
        /// <param name="target">The target</param>
        public TargetData(AttackableUnit target)
        {
            this.Target = target;

            this.ShouldWait = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> struct
        /// </summary>
        /// <param name="orderedEnumerable">The sorted targets</param>
        public TargetData(IEnumerable<AttackableUnit> orderedEnumerable) : this(orderedEnumerable?.FirstOrDefault())
        {
            
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is TargetData && this == (TargetData)obj;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            if (this.ShouldWait)
            {
                return -1;
            }

            return this.Target?.NetworkId ?? 0;
        }

        public static bool operator ==(TargetData left, TargetData right)
        {
            return left.GetHashCode() == right.GetHashCode();
        }

        public static bool operator !=(TargetData left, TargetData right)
        {
            return !(left == right);
        }
    }
}