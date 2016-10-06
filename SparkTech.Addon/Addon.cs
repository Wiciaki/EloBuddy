namespace SparkTech.Addon
{
    using System;

    using EloBuddy;

    using SparkTech.SDK.Executors;

    /// <summary>
    /// The main class
    /// </summary>
    [Trigger]
    public static class Addon
    {
        /// <summary>
        /// Initializes static members of the <see cref="Addon"/> class
        /// </summary>
        static Addon()
        {
            // Here is the right place to put your code, this is safe context.
            Chat.Print("Loaded addon!");

            Game.OnTick += OnTick;
        }

        /// <summary>
        /// The <see cref="E:OnTick"/> action
        /// </summary>
        /// <param name="args">The empty <see cref="EventArgs"/> instance</param>
        private static void OnTick(EventArgs args)
        {

        }
    }
}