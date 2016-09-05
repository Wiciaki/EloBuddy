namespace SparkTech.SDK.Executors
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using EloBuddy.SDK.Events;

    public static class Bootstrap
    {
        static Bootstrap()
        {
            Loading.OnLoadingComplete += delegate
                {
                    foreach (var handle in Variables.Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TriggerAttribute), false).Length > 0).Select(type => type.TypeHandle))
                    {
                        RuntimeHelpers.RunClassConstructor(handle);
                    }
                };
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        public static void Handle(this string[] args)
        {
            Array.ForEach(args, Console.WriteLine);
        }
    }
}