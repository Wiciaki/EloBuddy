namespace SparkTech.SDK.Web
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using SparkTech.SDK.Executors;
    
    public static class Versioning
    {
        /// <summary>
        /// The version for the current assembly
        /// </summary>
        public const string VersionSDK = "0.2.0.1";

        /// <summary>
        /// The version for the <see cref="E:AllyPingSpammer"/>
        /// </summary>
        public const string VersionAllyPingSpammer = "2.0.0.1";

        #region Internal

        /// <summary>
        /// The path to web version of the current class
        /// </summary>
        private const string CurrentClassWebPath = "https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/Web/Versioning.cs";

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
        static Versioning()
        {
            return;

            Connection.PermissionChanged += b => Match();

            Match();
        }

        [CodeFlow.Unsafe]
        private static async void Download()
        {
            data = await Connection.WebClient.DownloadStringTaskAsync(CurrentClassWebPath).ConfigureAwait(false);

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

            Assembly assembly;

            while (Assemblies.TryDequeue(out assembly))
            {
                ExecuteImpl(assembly);
            }
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

        #endregion
    }
}