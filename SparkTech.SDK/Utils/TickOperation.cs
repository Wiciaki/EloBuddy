namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.Executors;

    /// <summary>
    /// The tick utility
    /// </summary>
    [Trigger]
    public class TickOperation
    {
        /// <summary>
        /// The list containing the operations
        /// </summary>
        private static readonly List<TickOperation> Operations = new List<TickOperation>();

        /// <summary>
        /// The current time
        /// </summary>
        private static int currentTime;

        /// <summary>
        /// Initializes static members of the <see cref="TickOperation"/> class
        /// </summary>
        static TickOperation()
        {
            Game.OnUpdate += delegate
                {
                    currentTime = Game.Time.ToTicks();

                    foreach (var operation in Operations.FindAll(o => o.Active && o.lastTick + o.TickDelay > currentTime))
                    {
                        operation.lastTick = currentTime;

                        try
                        {
                            operation.action();
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception("TickOperation action failed!", ex);
                        }
                    }
                };
        }

        /// <summary>
        /// The action to be executed
        /// </summary>
        private readonly Action action;

        /// <summary>
        /// Determines whether the <see cref="TickOperation"/> will be executing
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// The tick delay
        /// </summary>
        public int TickDelay;

        /// <summary>
        /// The last tick time
        /// </summary>
        private int lastTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickOperation"/> class
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TickOperation(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.action = action;

            this.lastTick = currentTime;

            Operations.Add(this);
        }
    }
}