namespace SparkTech.SDK.Web
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using SparkTech.SDK.EventData;

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
        /// The regular expression
        /// </summary>
        private static readonly Regex Regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]");

        public static void Check(string link, Action<CheckPerformedEventArgs> action, Assembly callingAssembly)
        {
            Uri uri;

            if (link == null || !link.ToLower().Contains("raw.githubusercontent.com") || !link.EndsWith("properties/assemblyinfo.cs", StringComparison.CurrentCultureIgnoreCase) || !Uri.TryCreate(link, UriKind.Absolute, out uri) || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Invalid link provided!");
            }

            var assemblyName = callingAssembly.GetName();

            Action req  = async delegate
                {
                    var match = Regex.Match(await Connection.WebClient.DownloadStringTaskAsync(uri).ConfigureAwait(false));
                    var gitVersion = match.Success ? new Version(match.Groups[1].Value) : null;
                    var args = new CheckPerformedEventArgs(gitVersion, assemblyName.Version, assemblyName.Name);

                    if (action != null)
                    {
                        action(args);
                    }
                    else
                    {
                        args.Notify();
                    }
                };

            if (Connection.IsAllowed)
            {
                new Task(req).Start();
            }
            else
            {
                // This case is supposed to delay the check until the menu item has been marked

#pragma warning disable RCS1127
                Connection.PermissionChange change = null;
#pragma warning restore RCS1127

                change = enabled =>
                    {
                        new Task(req).Start();
                        Connection.PermissionChanged -= change;
                    };

                Connection.PermissionChanged += change;
            }
        }
    }
}