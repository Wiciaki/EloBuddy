namespace SparkTech.SDK.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Timers;

    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Utils;
    
    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Utils;

    /// <summary>
    /// The component initializer
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// The path to web version of the versioning class
        /// </summary>
        private const string VersioningWebPath = "https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/Web/Versioning.cs";

        /// <summary>
        /// The downloaded version data to be matched
        /// </summary>
        private static readonly string Data;

        /// <summary>
        /// The timer
        /// </summary>
        private static readonly Timer Timer;

        /// <summary>
        /// The flip list
        /// </summary>
        private static readonly List<string> Flips = new List<string>(4)
                                                       {
                                                           @"\", "|", "/", "-"
                                                       };

        /// <summary>
        /// The object representation of the currently executing assembly
        /// </summary>
        public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        [CodeFlow.Unsafe]
        static Bootstrap()
        {
            Console.Title = "Connecting to GitHub...";

            Timer = new Timer(230d);

            var index = 0;

            Timer.Elapsed += delegate
                {
                    if (++index == Flips.Count)
                    {
                        index = 0;
                    }

                    Console.Title = "SparkTech load... " + Flips[index];
                };

            Timer.Start();

            AppDomain.CurrentDomain.DomainUnload += delegate
                {
                    Console.Title = "SparkTech reload...";
                };

            using (var client = new WebClient())
            {
                Data = client.DownloadString(VersioningWebPath);
            }

            Loading.OnLoadingComplete += delegate
                {
                    Timer.Stop();
                    Timer.Dispose();
                    Flips.Clear();
                    Flips.TrimExcess();

                    Console.Title = "Obtaining license...";

                    RuntimeHelpers.RunClassConstructor(typeof(Creator).TypeHandle);
                    CodeFlow.Secure(MainMenu.Rebuild);

                    Console.Title = "SparkTech.SDK";
                };

            Process(Assembly);
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        [CodeFlow.Unsafe]
        public static void Init(this string[] args)
        {
            Array.ForEach(args, Console.WriteLine);

            Process(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Invokes .cctors of types with the <see cref="TriggerAttribute"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to have its trigger types invoked</param>
        [CodeFlow.Unsafe]
        private static void Process(Assembly assembly) => Loading.OnLoadingComplete += delegate
            {
                foreach (var type in assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TriggerAttribute), false).Cast<TriggerAttribute>().Any(trigger => trigger.Eligible)).OrderBy(type => type.Name))
                {
                    try
                    {
                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                    }
                    catch (TypeInitializationException ex)
                    {
                        Logger.Exception($"Couldn't invoke \"{type.FullName}\"!", ex.InnerException);
                    }
                }
                
                var assemblyName = assembly.GetName();
                var split = assemblyName.Name.Split('.');
                var name = split[split.Length - 1];

                var match = new Regex($@"Version{name} = ""(\d.\d.\d.\d)""").Match(Data);

                if (!match.Success)
                {
                    return;
                }

                name = name.ToLower();

                var menu = Creator.MainMenu.GetMenu("update");
                var webVersion = new Version(match.Groups[1].Value);
                var local = assemblyName.Version;
                var update = webVersion > local;

                Creator.MainMenu.Replacements.Add(name, () => update ? $"{local} => {webVersion}" : local.ToString());

                menu[$"note.{name}"] = new MenuItem($"update_note_{name}", null, true);
                menu[$"info.{name}"] = new MenuItem($"updated_{(!update ? "yes" : "no")}_{name}");

                if (update)
                {
                    Comms.Print(Creator.MainMenu.GetTranslation("update_available"));
                }
            };
    }
}