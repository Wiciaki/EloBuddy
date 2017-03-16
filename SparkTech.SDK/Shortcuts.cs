namespace SparkTech.SDK
{
    using System;

    using EloBuddy;

    using SparkTech.SDK.Cache;

    /// <summary>
    /// The delegate used for passing argument-less Boolean pointers
    /// </summary>
    /// <returns></returns>
    public delegate bool Predicate();

    /// <summary>
    /// The main event delegate used for handling most of the event data instances
    /// </summary>
    /// <typeparam name="TEventArgs">The destination event arguments</typeparam>
    /// <param name="args">The actual event data</param>
    public delegate void EventDataHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;

    public static class Shortcuts
    {
        public static Random RandomInst = new Random();

        public static AIHeroClient PlayerInst => ObjectCache.Player;
    }
}