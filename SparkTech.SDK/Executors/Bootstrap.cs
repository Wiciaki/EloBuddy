namespace SparkTech.SDK.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Timers;

    using EloBuddy.SDK.Events;

    using SparkTech.SDK.Web;

    /// <summary>
    /// The component initializer
    /// </summary>
    public static class Bootstrap
    {
        private static readonly Timer Timer;

        private static readonly List<string> Flips = new List<string>(4)
                                                       {
                                                           @"\", "|", "/", "-"
                                                       };

        /// <summary>
        /// The object representation of the currently executing assembly
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        static Bootstrap()
        {
            var i = 0;

            Timer = new Timer(250d) { Enabled = true };

            Timer.Elapsed += delegate
                {
                    if (++i == Flips.Count)
                    {
                        i = 0;
                    }

                    Console.Title = "SparkTech init... " + Flips[i];
                };

            HandleTrigger(Assembly = Assembly.GetExecutingAssembly());
        }

        internal static void Notify()
        {
            Timer.Stop();
            Timer.Dispose();
            Flips.Clear();
            Flips.TrimExcess();

            Console.Title = "SparkTech.SDK";
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        public static void Init(this string[] args)
        {
            Array.ForEach(args, Console.WriteLine);

            HandleTrigger(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Invokes .cctors of types with the <see cref="TriggerAttribute"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to have its types invoked</param>
        private static void HandleTrigger(Assembly assembly)
        {
            Loading.OnLoadingComplete += delegate
                {
                    Versioning.Handle(assembly);

                    foreach (var type in assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TriggerAttribute), false).Length > 0).OrderBy(type => type.Name))
                    {
                        try
                        {
                            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                        }
                        catch (TypeInitializationException ex)
                        {
                            Log.Exception(ex);
                        }
                    }
                };
        }
    }
}