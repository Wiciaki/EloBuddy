namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Executors;

    public static class Updater
    {
        public static void Check(string username, string repository, string folderName, Action<CheckPerformedEventArgs> action = null)
        {
            Check($"https://raw.githubusercontent.com/{username}/{repository}/master/{folderName}/Properties/AssemblyInfo.cs", action, Assembly.GetCallingAssembly());
        }

        public static void Check(string link, Action<CheckPerformedEventArgs> action = null)
        {
            Check(link, action, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// The regular expression used for matching the assembly info file
        /// </summary>
        private static readonly Regex Regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]");
        
        private static async void Check(string link, Action<CheckPerformedEventArgs> action, Assembly callingAssembly)
        {
            Uri uri;

            if (link == null || !link.ToLower().Contains("raw.githubusercontent.com") || !link.EndsWith("properties/assemblyinfo.cs", StringComparison.CurrentCultureIgnoreCase) || !Uri.TryCreate(link, UriKind.Absolute, out uri) || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Invalid link provided!");
            }

            string data;

            using (var client = new WebClient())
            {
                data = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
            }

            var match = Regex.Match(data);
            var gitVersion = match.Success ? new Version(match.Groups[1].Value) : null;

            var assemblyName = callingAssembly.GetName();
            var args = new CheckPerformedEventArgs(gitVersion, assemblyName.Version, assemblyName.Name);

            CodeFlow.Secure(() =>
                    {
                        if (action != null)
                        {
                            action(args);
                        }
                        else
                        {
                            args.Notify();
                        }
                    });
        }
    }
}