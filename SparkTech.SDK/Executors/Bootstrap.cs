namespace SparkTech.SDK.Executors
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Timers;
    
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Web;

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
            AppDomain.CurrentDomain.DomainUnload += delegate
                {
                    Console.Title = "SparkTech reload...";
                };

            Timer = new Timer(225d);

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

            using (var client = new WebClient())
            {
                Data = client.DownloadString(VersioningWebPath);
            }

            HandleTrigger(Assembly);
        }
        
        internal static void Release()
        {
            MainMenu.Refresh();

            Timer.Stop();
            Timer.Dispose();
            Flips.Clear();
            Flips.TrimExcess();
            
            GC.Collect();

            Console.Title = "SparkTech.SDK";
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        [CodeFlow.Unsafe]
        public static void Init(this string[] args)
        {
            Array.ForEach(args, Console.WriteLine);

            var assembly = Assembly.GetCallingAssembly();

            HandleTrigger(assembly);
        }

        /// <summary>
        /// Invokes .cctors of types with the <see cref="TriggerAttribute"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to have its types invoked</param>
        private static void HandleTrigger(Assembly assembly)
        {
            Loading.OnLoadingComplete += delegate
                {
                    //Versioning.Handle(assembly);

                    foreach (var type in assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TriggerAttribute), false).Cast<TriggerAttribute>().Any(trigger => trigger.Eligible)).OrderBy(type => type.Name))
                    {
                        try
                        {
                            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                        }
                        catch (TypeInitializationException ex)
                        {
                            Logger.Exception($"Couldn't invoke \"{type.FullName}\"!", ex);
                        }
                    }
                };
        }

        private static readonly ConcurrentQueue<Assembly> Assemblies = new ConcurrentQueue<Assembly>();

        private static string data;

        [CodeFlow.Unsafe]
        internal static void Handle(Assembly assembly)
        {
            return;

            Assemblies.Enqueue(assembly);

            Match();
        }

        [CodeFlow.Unsafe]
        private static async void Download()
        {
            data = await Connection.WebClient.DownloadStringTaskAsync(VersioningWebPath).ConfigureAwait(false);

            CodeFlow.Secure(Match);
        }

        [CodeFlow.Unsafe]
        private static void Match()
        {
            if (data == null)
            {
                if (Connection.IsAllowed)
                {
                    new Task(Download).Start();
                }

                return;
            }

            var menu = Creator.MainMenu.GetMenu("sdk.versioning");
            const string SDKItemName = "sdk.version";

            if (Assemblies.Count > 0)
            {
                if (menu[SDKItemName] != null)
                {
                    menu.Instance.Remove(SDKItemName);
                }
            }
            else if (menu[SDKItemName] == null)
            {
                ExecuteImpl(Bootstrap.Assembly);
            }
            /*
            while (Assemblies.TryDequeue(out var assembly))
            {
                ExecuteImpl(assembly);
            }*/
        }

        private static void ExecuteImpl(Assembly assembly, string assemblyName = null)
        {
            var name = assembly.GetName();

            if (assemblyName == null)
            {
                assemblyName = name.Name;
            }

            var match = new Regex($@"Version{assemblyName} = ""(\d.\d.\d.\d)""").Match(data);

            if (!match.Success)
            {
                return;
            }

            var webVersion = new Version(match.Groups[1].Value);
            var local = name.Version;

        }
    }
}