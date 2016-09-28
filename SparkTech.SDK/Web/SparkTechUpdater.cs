namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;

    using SparkTech.SDK.Utils;

    public class SparkTechUpdater : Updater
    {
        private static string data;

        private static bool downloading;

        private readonly string assemblyName;

        private readonly Version version;

        private readonly Regex regex;

        public SparkTechUpdater(Version version, string assemblyName, string internalName = null) : base(Variables.RawVariablesPath)
        {
            var pattern = "Version" + (internalName ?? assemblyName) + @" = ""(\d.\d.\d.\d)""";

            this.regex = new Regex(pattern, RegexOptions.CultureInvariant);

            this.version = version;

            this.assemblyName = assemblyName;

            this.PerformCheck();
        }

        private async void PerformCheck()
        {
            if (downloading)
            {
                TickOperation.ExecuteOnNextTick(this.PerformCheck);
                return;
            }

            if (data == null)
            {
                downloading = true;

                using (var client = new WebClient())
                {
                    data = await client.DownloadStringTaskAsync(this.Link).ConfigureAwait(false);
                }

                downloading = false;
            }

            TickOperation.ExecuteOnNextTick(delegate
            {
                var match = this.regex.Match(data);

                this.RaiseEvent(
                    match.Success ? new Version(match.Groups[1].Value) : null,
                    this.version,
                    this.assemblyName);
            });
        }
    }
}