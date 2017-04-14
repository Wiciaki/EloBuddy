namespace SparkTech.SDK
{
    using System;

    /// <summary>
    /// The main event delegate used for handling most of the event data instances
    /// </summary>
    /// <typeparam name="TEventArgs">The destination event arguments</typeparam>
    /// <param name="args">The actual event data</param>
    public delegate void EventDataHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;
}