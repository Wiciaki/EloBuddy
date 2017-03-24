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
        /// The path to web version of the version data class
        /// </summary>
        private const string VersioningWebPath = "https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/VersionInfo.cs";

        /// <summary>
        /// The downloaded version data to be matched
        /// </summary>
        private static readonly string Data;

        /// <summary>
        /// The object representation of the currently executing assembly
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        [CodeFlow.Unsafe]
        static Bootstrap()
        {
            Console.Title = "Connecting to GitHub...";

            AppDomain.CurrentDomain.DomainUnload += delegate
                {
                    Console.Title = "SparkTech reload...";
                };

            var flips = new List<string>(4)
                            {
                                @"\", "|", "/", "-"
                            };

            var timer = new Timer(230d);

            var index = 0;

            timer.Elapsed += delegate // TODO: Escape implicitly captured closure
                {
                    if (++index == flips.Count)
                    {
                        index = 0;
                    }

                    Console.Title = "SparkTech load... " + flips[index];
                };

            timer.Start();

            using (var client = new WebClient())
            {
                Data = client.DownloadString(VersioningWebPath);
            }

            Loading.OnLoadingComplete += delegate
                {
                    timer.Stop();
                    timer.Dispose();
                    flips.Clear();
                    flips.TrimExcess();

                    Console.Title = "SparkTech.SDK";

                    ExecuteConstructor(typeof(Creator));
                };

            Process(Assembly = Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        /// <exception cref="ArgumentException">The array wasn't empty</exception>
        /// <exception cref="ArgumentNullException">The array was equal to null</exception>
        [CodeFlow.Unsafe]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Init(this string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length != 0)
            {
                throw new ArgumentException("args.Length != 0");
            }

            Process(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Invokes .cctors of types with the <see cref="TriggerAttribute"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to have its trigger types invoked</param>
        [CodeFlow.Unsafe]
        private static void Process(Assembly assembly) => Loading.OnLoadingComplete += delegate
            {
                foreach (var type in assembly.GetTypes().Where(HasActiveTrigger).OrderBy(type => type.Name))
                {
                    try
                    {
                        ExecuteConstructor(type);
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

                Creator.MainMenu.Replacements.Add(name + "Version", () => update ? $"{local} => {webVersion}" : $"{local}");

                menu[$"note.{name}"] = new MenuItem($"update_note_{name}", null, true);
                menu[$"info.{name}"] = new MenuItem($"updated_{(!update ? "yes" : "no")}_{name}");

                if (update)
                {
                    Comms.Print(Creator.MainMenu.GetTranslation("update_available"));
                }
            };

        /// <summary>
        /// Run the static constructor of the specified type
        /// </summary>
        /// <param name="type">The provided type</param>
        private static void ExecuteConstructor(Type type) => RuntimeHelpers.RunClassConstructor(type.TypeHandle);

        /// <summary>
        /// Determines whether the specified type should be initialized
        /// </summary>
        /// <param name="type">The type to be inspected</param>
        /// <returns>A value determining whether the type should be invoked</returns>
        private static bool HasActiveTrigger(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(TriggerAttribute), false);

            return attributes.Length == 1 && ((TriggerAttribute)attributes[0]).Eligible;
        }
    }
}