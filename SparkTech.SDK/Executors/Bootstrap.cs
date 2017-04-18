namespace SparkTech.SDK.Executors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Timers;

    using EloBuddy.SDK.Events;

    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Utils;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The component initializer
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        /// <exception cref="ArgumentNullException">The array was equal to null</exception>
        [CodeFlow.Unsafe]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Init(this string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length != 1 || args[0] != null)
            {
                Console.WriteLine("This executable must not be opened manually!");

                while (true)
                {
                    Console.ReadKey(true);
                }
            }

            Process(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Invokes a remote assembly
        /// </summary>
        /// <param name="link">The link to download the file</param>
        /// <param name="versionLink">The link to download the version string</param>
        [CodeFlow.Unsafe]
        public static async void WebLoad(string link, string versionLink = null)
        {
            if (link.Last() == '/')
            {
                link = link.Remove(link.Length - 1);
            }

            var name = link.Split('/').Last().Remove("?raw=true");
            var path = FileManager.WorkingDirectory.GetFolder("Addons").GetFile(name);

            using (var client = new WebClient())
            {
                Uri uri;

                if (File.Exists(path) && Uri.TryCreate(versionLink, UriKind.Absolute, out uri))
                {
                    var download = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);

                    var match = Regex.Match(download, @"\d+.\d+.\d+.\d+");

                    if (!match.Success)
                    {
                        Log.Warn($"Version file couldn't be matched. {name} will not be loaded.");
                        return;
                    }

                    var assembly = Assembly.LoadFile(path);
                    var local = assembly.GetName().Version;
                    var remote = new Version(match.Value);

                    if (local >= remote)
                    {
                        Process(assembly);
                        return;
                    }
                }

                await client.DownloadFileTaskAsync(link, path).ConfigureAwait(false);
            }

            Process(Assembly.LoadFile(path));
        }

        /// <summary>
        /// The task for the downloaded github data
        /// </summary>
        private static readonly Task<string> DataTask;

        /// <summary>
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        [CodeFlow.Unsafe]
        static Bootstrap()
        {
            var timer = new Timer(230d);
            timer.Elapsed += Elapsed;
            timer.Start();

            Loading.OnLoadingComplete += delegate
                {
                    ExecuteConstructor(typeof(Creator));

                    timer.Stop();
                    timer.Elapsed -= Elapsed;
                    timer.Dispose();
                    Flips.Clear();
                    Flips.TrimExcess();
                    flipIndex = 0;

                    GC.Collect();

                    Console.Title = "SparkTech.SDK";
                };

            DataTask = Task.Run(async () =>
                    {
                        using (var client = new WebClient())
                        {
                            return await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/VersionInfo.cs").ConfigureAwait(false);
                        }
                    });

            ExecuteConstructor(typeof(Log));

            Process(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// The current index of the flips
        /// </summary>
        private static int flipIndex;

        /// <summary>
        /// The list of the flips used
        /// </summary>
        private static readonly List<string> Flips = new List<string>(4)
                                                         {
                                                             @"\", "|", "/", "-"
                                                         };

        /// <summary>
        /// Invokes every time a timer has been called
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
        private static void Elapsed(object sender, ElapsedEventArgs args)
        {
            Console.Title = "SparkTech load... " + Flips[flipIndex++ % (Flips.Count + 1)];
        }

        /// <summary>
        /// Determined whether the notification message has been printed
        /// </summary>
        private static bool notified;

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
                        Log.Exception(ex.InnerException, $"Couldn't invoke \"{type.FullName}\"!");
                    }
                }

                var assemblyName = assembly.GetName();
                var split = assemblyName.Name.Split('.');
                var name = split[split.Length - 1];

                var match = Regex.Match(DataTask.Result, $@"Version{name} = ""(\d+.\d+.\d+.\d+)""");

                if (!match.Success)
                {
                    return;
                }

                name = name.ToLower();

                var webVersion = new Version(match.Groups[1].Value);
                var local = assemblyName.Version;
                var update = webVersion > local;
                var updateText = update ? "no" : "yes";

                Creator.MainMenu.Replacements.Add(name + "Version", () => update ? $"{local} => {webVersion}" : $"{local}");

                var menu = Creator.MainMenu.GetMenu("update");
                menu["note." + name] = new MenuItem("update_note_" + name, MenuItem.LabelType.GroupLabel);
                menu["info." + name] = new MenuItem("updated_" + updateText + "_" + name);

                if (update && !notified)
                {
                    notified = true;

                    Creator.MainMenu.Print("update_available");
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