namespace SparkTech.SDK.Utils
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.Executors;

    /// <summary>
    ///     Executes an operation each set amount of ticks.
    /// </summary>
    public class TickOperation : Executable
    {
        #region Fields

        /// <summary>
        ///     Contains the next tick value that Action should be executed.
        /// </summary>
        private int nextTick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Constructor for a new Tick Operation instance, auto-starts by default.
        /// </summary>
        /// <param name="tickDelay">
        ///     A set delay between ticks the action should be executed.
        /// </param>
        /// <param name="action">
        ///     The executed action.
        /// </param>
        public TickOperation(int tickDelay, Action action)
        {
            this.action = action;
            this.TickDelay = tickDelay;

            this.IsRunning = true;
            Game.OnUpdate += this.OnTick;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The Executed Action.
        /// </summary>
        private readonly Action action;

        /// <summary>
        ///     Gets or sets a value indicating whether is the Tick Operation is currently running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Gets or sets a delay between ticks that action should be executed.
        /// </summary>
        public int TickDelay;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="managed">Determines whether managed sources should be cleaned</param>
        protected override void Dispose(bool managed)
        {
            if (this.IsRunning)
            {
                Game.OnUpdate -= this.OnTick;
            }

            if (!managed)
            {
                return;
            }

            this.TickDelay = 0;
            this.nextTick = 0;
            this.IsRunning = false;
        }

        /// <summary>
        ///     Starts the tick operation.
        /// </summary>
        /// <returns>Tick Operation instance.</returns>
        public void Start()
        {
            if (!this.IsRunning)
            {
                Game.OnUpdate += this.OnTick;
            }
        }

        /// <summary>
        ///     Stops the tick operation.
        /// </summary>
        /// <returns>Tick Operation instance.</returns>
        public void Stop()
        {
            if (this.IsRunning)
            {
                Game.OnUpdate -= this.OnTick;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Notified function per game tick by Game.OnGameUpdate event.
        ///     Executes the action if met tick requirements.
        /// </summary>
        /// <param name="args"><see cref="System.EventArgs" /> event data</param>
        private void OnTick(EventArgs args)
        {
            var time = Game.Time.ToTicks();

            if (this.nextTick > time)
            {
                return;
            }

            this.action();

            this.nextTick = time + this.TickDelay;
        }

        public static void ExecuteOnNextTick(Action action)
        {
            GameUpdate @delegate = null;

            @delegate = delegate
            {
                Game.OnUpdate -= @delegate;

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Logger.Exception(LogLevel.Error, "ExecuteOnNextTick:", ex);
                }
            };

            Game.OnUpdate += @delegate;
        }

        #endregion
    }
}