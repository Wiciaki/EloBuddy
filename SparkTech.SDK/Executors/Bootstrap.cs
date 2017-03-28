﻿namespace SparkTech.SDK.Executors
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

    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Utils;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The component initializer
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// The working directory for the executing assembly and its dependencies
        /// </summary>
        private static readonly string WorkingDirectory;

        /// <summary>
        /// The path to web version of the version data class
        /// </summary>
        private const string VersioningWebPath = "https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/VersionInfo.cs";

        /// <summary>
        /// The task for the downloaded data
        /// </summary>
        private static readonly Task<string> DataTask;

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

        public static void WebLoad(string link, string versionLink = null, string pattern = null) => Resolve(link, versionLink, pattern);

        /// <summary>
        /// Invokes a remote assembly
        /// </summary>
        /// <param name="link">The link to download the file</param>
        /// <param name="versionLink">The link to download the version string</param>
        /// <param name="pattern">The pattern to be searched in the version string for</param>
        [CodeFlow.Unsafe]
        private static async void Resolve(string link, string versionLink, string pattern)
        {
            var libPath = Path.Combine(WorkingDirectory, "Addons");
            Directory.CreateDirectory(libPath);

            var name = link.Split('/').Last().Remove("?raw=true");
            var path = Path.Combine(libPath, name);

            using (var client = new WebClient())
            {
                Uri uri;

                if (File.Exists(path) && Uri.TryCreate(versionLink, UriKind.Absolute, out uri))
                {
                    var download = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);

                    if (string.IsNullOrEmpty(pattern))
                    {
                        pattern = @"\d+.\d+.\d+.\d+";
                    }

                    var match = Regex.Match(download, pattern);

                    if (!match.Success)
                    {
                        Logger.Error($"SparkTech.SDK: Version file couldn't be matched. {name} will not be loaded.");
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
        /// Initializes static members of the <see cref="Bootstrap"/> class
        /// </summary>
        [CodeFlow.Unsafe]
        static Bootstrap()
        {
            var flips = new List<string>(4)
                            {
                                @"\", "|", "/", "-"
                            };

            var index = 0;
            var timer = new Timer(230d);

            timer.Elapsed += delegate // TODO: Escape implicitly captured closure
                {
                    if (++index == flips.Count)
                    {
                        index = 0;
                    }

                    Console.Title = "SparkTech load... " + flips[index];
                };

            timer.Start();

            AppDomain.CurrentDomain.DomainUnload += delegate
                {
                    Console.Title = "SparkTech unloaded";
                };

            WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy", "SparkTech");
            Directory.CreateDirectory(WorkingDirectory);

            Loading.OnLoadingComplete += delegate
                {
                    timer.Stop();
                    timer.Dispose();
                    flips.Clear();
                    flips.TrimExcess();

                    Console.Title = "SparkTech.SDK";

                    ExecuteConstructor(typeof(Creator));
                };

            DataTask = new WebClient().DownloadStringTaskAsync(VersioningWebPath);

            Process(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Invokes .cctors of types with the <see cref="TriggerAttribute"/> in the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly to have its trigger types invoked</param>
        [CodeFlow.Unsafe]
        private static void Process(Assembly assembly) => Loading.OnLoadingComplete += delegate
            {
                var triggered = false;

                foreach (var type in assembly.GetTypes().Where(HasActiveTrigger).OrderBy(type => type.Name))
                {
                    triggered = true;

                    try
                    {
                        ExecuteConstructor(type);
                    }
                    catch (TypeInitializationException ex)
                    {
                        Logger.Exception($"Couldn't invoke \"{type.FullName}\"!", ex.InnerException);
                    }
                }

                if (!triggered)
                {
                    return;
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