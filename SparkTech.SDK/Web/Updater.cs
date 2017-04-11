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
            Check($"https://raw.githubusercontent.com/{username}/{repository}/master/{folderName}/Properties/AssemblyInfo.cs", action, Assembly.GetCallingAssembly().GetName());
        }

        public static void Check(string link, Action<CheckPerformedEventArgs> action = null)
        {
            Check(link, action, Assembly.GetCallingAssembly().GetName());
        }

        /// <summary>
        /// The regular expression used for matching the assembly info file
        /// </summary>
        private static readonly Regex Regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]");
        
        private static async void Check(string link, Action<CheckPerformedEventArgs> action, AssemblyName name)
        {
            Uri uri;

            if (link == null || !link.ToLower().Contains("raw.githubusercontent.com"))
            {
                return;
            }

            if (!link.EndsWith("properties/assemblyinfo.cs", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (!Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                return;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                return;
            }

            string data;

            using (var client = new WebClient())
            {
                data = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
            }

            var match = Regex.Match(data);
            var gitVersion = match.Success ? new Version(match.Groups[1].Value) : null;

            var args = new CheckPerformedEventArgs(gitVersion, name.Version, name.Name);

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