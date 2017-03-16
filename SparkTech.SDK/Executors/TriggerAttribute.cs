namespace SparkTech.SDK.Executors
{
    using System;

    using EloBuddy;

    using static SparkTech.SDK.Shortcuts;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TriggerAttribute : Attribute
    {
        /// <summary>
        /// Determines whether the owner of this instance should be processed
        /// </summary>
        public bool Eligible;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerAttribute"></see> attribute
        /// </summary>
        /// <param name="champions">If one of these champions is the currently used one, the trigger will be proceeded. <para>If no champions are specified, it evaluates to <c>true</c></para></param>
        public TriggerAttribute(params Champion[] champions)
        {
            this.Eligible = champions.Length == 0 || Array.IndexOf(champions, PlayerInst.Hero) != -1;
        }
    }
}