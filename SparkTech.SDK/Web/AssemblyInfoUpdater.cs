/*

namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Reflection;

    using SparkTech.EventData;

    /// <summary>
    /// <para>A child of the <see cref="Updater"/> class, uses a method which makes it change a thread, unfortunately (leading to possible bugsplats on drawings drawn in event method etc.)</para>
    /// All of the invokable methods exposed by me are knows to be thread-safe and here I mean mostly "args.Notify()"
    /// </summary>
    public class AssemblyInfoUpdater : Updater
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoUpdater"/> class with the specified link
        /// </summary>
        public AssemblyInfoUpdater(string fullRawAssemblyInfoLink) : this(fullRawAssemblyInfoLink, Assembly.GetCallingAssembly())
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoUpdater"/> class with the specified link and a calling <see cref="Assembly"/>
        /// </summary>
        /// <param name="fullRawAssemblyInfoLink">he link to the assemblyinfo.cs file that can be found in your github. This has to be a raw link</param>
        /// <param name="callingAssembly">The <see cref="Assembly"/> you are retrieving data for</param>
        public AssemblyInfoUpdater(string fullRawAssemblyInfoLink, Assembly callingAssembly) : this(fullRawAssemblyInfoLink, callingAssembly.GetName().Version, callingAssembly.GetName().Name)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoUpdater"/> class
        /// </summary>
        /// <param name="fullRawAssemblyInfoLink">The link to the assemblyinfo.cs file that can be found in your github. This has to be a raw link</param>
        /// <param name="localVersion">The local version of the assembly to compare</param>
        /// <param name="assemblyName">The name of the <see cref="Assembly"/></param>
        public AssemblyInfoUpdater(string fullRawAssemblyInfoLink, Version localVersion, string assemblyName) : base(fullRawAssemblyInfoLink, @"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]")
        {
            var low = this.Link.AbsolutePath.ToLower();

            if (low.Contains("assemblyinfo.cs") && low.Contains("raw.githubusercontent.com"))
                new Action(
                    async delegate
                        {
                            using (var client = new WebClient())
                            {
                                var match = this.Regex.Match(await client.DownloadStringTaskAsync(this.Link));

                                this.RaiseEvent(
                                    new CheckPerformedEventArgs(
                                        match.Success ? new Version(match.Groups[1].Value) : null,
                                        localVersion,
                                        assemblyName));
                            }
                        })();
        }
    }
}

*/