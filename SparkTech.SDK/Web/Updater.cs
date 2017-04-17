namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Executors;

	/// <summary>
	/// The class useful for performing update checks
	/// </summary>
    public static class Updater
    {
		/// <summary>
		/// Performs an update check and executes the requested action
		/// </summary>
		/// <param name="link">The specified link</param>
		/// <param name="action">The optional, specified action</param>
        public static async void Check(string link, Action<CheckPerformedEventArgs> action = null)
        {
            if (link == null || !link.ToLower().Contains("raw.githubusercontent.com"))
            {
                return;
            }

            if (!link.EndsWith("properties/assemblyinfo.cs", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

	        Uri uri;

			if (!Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                return;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                return;
            }

	        var name = Assembly.GetCallingAssembly().GetName();

			string data;

            using (var client = new WebClient())
            {
                data = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
            }

            var match = Regex.Match(data, @"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]");
            var gitVersion = match.Success ? new Version(match.Groups[1].Value) : null;

            var args = new CheckPerformedEventArgs(gitVersion, name.Version, name.Name);

	        if (action != null)
	        {
		        CodeFlow.Secure(() => action(args));
	        }
	        else
	        {
		        CodeFlow.Secure(args.Notify);
	        }
        }
	}
}