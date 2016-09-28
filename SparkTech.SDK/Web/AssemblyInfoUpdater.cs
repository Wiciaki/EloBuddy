namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>
    /// <para>A child of the <see cref="Updater"/> class, uses a method which makes it change a thread, unfortunately (leading to possible bugsplats on drawings drawn in event method etc.)</para>
    /// All of the invokable methods exposed by me are knows to be thread-safe and here I mean mostly "args.Notify()"
    /// </summary>
    public class AssemblyInfoUpdater : Updater
    {
        /// <summary>
        /// The regular expression
        /// </summary>
        private static readonly Regex Regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoUpdater"/> class
        /// </summary>
        /// <param name="fullRawAssemblyInfoLink">The link to the assemblyinfo.cs file that can be found in your github. This has to be a raw link</param>
        /// <param name="localVersion">The local version of the assembly to compare</param>
        /// <param name="assemblyName">The name of the assembly</param>
        public AssemblyInfoUpdater(string fullRawAssemblyInfoLink, Version localVersion, string assemblyName) : base(fullRawAssemblyInfoLink)
        {
            var low = this.Link.AbsoluteUri.ToLower();

            if (!low.Contains("assemblyinfo.cs") || !low.Contains("raw.githubusercontent.com"))
            {
                return;
            }

            new Action(async () =>
                {
                    Match match;

                    using (var client = new WebClient())
                    {
                        match = Regex.Match(await client.DownloadStringTaskAsync(this.Link).ConfigureAwait(false));
                    }

                    this.RaiseEvent(match.Success ? new Version(match.Groups[0].Value) : null, localVersion, assemblyName);
                })();
        }
    }
}