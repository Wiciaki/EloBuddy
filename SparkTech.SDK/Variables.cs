namespace SparkTech.SDK
{
    using System;
    using System.Reflection;

    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Web;

    [Trigger]
    public static class Variables
    {
        public const string VersionSDK = "0.1.1.3";

        public const string VersionAllyPingSpammer = "1.2.1.2";

        public const string RawVariablesPath = "https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/Variables.cs";

        public static readonly bool FirstRun;

        public static Assembly Assembly => Bootstrap.Assembly;

        public static readonly Menu SDKMenu;

        static Variables()
        {
            SDKMenu = MainMenu.AddMenu("SparkTech.SDK", "st.sdk", "Spark's Software Development Kit");

            #region FirstInit
            {
                var first = new CheckBox("ERROR") { IsVisible = false };
                SDKMenu.Add("st.sdk.firstrun", first);
                FirstRun = first.CurrentValue;
                first.CurrentValue = false;
            }
            #endregion

            #region Info
            {
                var info = SDKMenu.AddSubMenu("About", "st.sdk.info");
                info.AddGroupLabel("Version");
                var allow = new CheckBox("Perform update checks");
                info.Add("st.sdk.info.version.check", allow);
                var version = new Label(allow.CurrentValue ? "Version - Checking..." : "Update checks disabled");
                info.Add("st.sdk.info.version", version);
                info.AddSeparator();
                info.AddGroupLabel("Please report any bugs or suggestions at:");
                info.AddLabel("Skype: \"wiktorsharp\"");
                info.AddLabel("Discord: @spark");

                /*
                info.AddGroupLabel("Donations:");
                info.AddLabel("paypal.me/Wiciaki");
                info.AddLabel("Bitcoins or any other methods, just message me"); // TODO
                info.AddLabel("If you've donated, message me please, so I can thank you for that again :)");
                */

                if (allow.CurrentValue)
                {
                    var assemblyName = Assembly.GetName();

                    new SparkTechUpdater(assemblyName.Version, assemblyName.Name, "SDK").CheckPerformed += args => version.DisplayName = args.Success ? args.IsUpdated ? "Your copy of SDK is up to date." : "A new update is available!" : "Download failure";
                }
            }
            #endregion
        }
    }
}