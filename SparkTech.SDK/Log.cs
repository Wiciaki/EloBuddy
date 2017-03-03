namespace SparkTech.SDK
{
    using System;
    using System.Runtime.CompilerServices;

    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.MenuWrapper;

    public static class Log
    {
        private static readonly MenuItem VerboseItem;

        static Log()
        {
            return;

            var menu = Creator.MainMenu.GetMenu("sdk.about");

            menu.Add("sdk.about.logging", new MenuItem("sdk_about_logging"));
            //VerboseItem = menu.Add("sdk.about.logging.verbose", new MenuItem("sdk_about_logging_verbose", false));
        }

        public static void Exception(Exception ex, string message = null, [CallerMemberName] string caller = null)
        {
            Logger.Exception(message ?? caller, ex);
        }

        public static void Verbose(string message)
        {
            if (VerboseItem)
            {
                Logger.Info(message);
            }
        }
    }
}