namespace SparkTech.SDK.Executors
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using EloBuddy.SDK.Events;

    using SparkTech.SDK.Utils;

    /// <summary>
    /// The initializator
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// The current assembly
        /// </summary>
        public static readonly Assembly Assembly = Assembly.GetAssembly(typeof(Bootstrap));

        /// <summary>
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        static Bootstrap()
        {
            HandleTrigger(Assembly);
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Handle(this string[] args)
        {
            Array.ForEach(args, Console.WriteLine);

            HandleTrigger(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Invokes .cctors of types with the TriggerAttribute in the specified asembly
        /// </summary>
        /// <param name="assembly">The assembly to be invoked</param>
        private static void HandleTrigger(Assembly assembly)
        {
            Loading.OnLoadingComplete += delegate
                {
                    Array.ForEach(assembly.GetTypes(), type =>
                            {
                                if (type.GetCustomAttributes(typeof(TriggerAttribute), false).Length > 0)
                                {
                                    try
                                    {
                                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                                    }
                                    catch (TypeInitializationException ex)
                                    {
                                        ex.Catch();
                                    }
                                }
                            });
                };
        }
    }
}