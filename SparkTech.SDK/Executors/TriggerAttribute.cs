namespace SparkTech.SDK.Executors
{
    using System;

    using EloBuddy;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TriggerAttribute : Attribute
    {
        public bool Eligible;

        public TriggerAttribute(params Champion[] champions)
        {
            this.Eligible = champions.Length == 0 || Array.IndexOf(champions, Shortcuts.MyHero.Hero) != -1;
        }
    }
}