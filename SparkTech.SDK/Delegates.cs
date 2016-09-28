namespace SparkTech.SDK
{
    using System;

    /// <summary>
    /// The delegate used for passing argumentless Boolean pointers
    /// </summary>
    /// <returns></returns>
    public delegate bool Predicate();

    /// <summary>
    /// The main event delegate used for handling most of the event data instances
    /// </summary>
    /// <typeparam name="TEventArgs">The destination event args</typeparam>
    /// <param name="args">The event data</param>
    public delegate void EventDataHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;
}