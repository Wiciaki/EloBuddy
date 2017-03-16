﻿namespace SparkTech.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Windows;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Utils;
    using SparkTech.SDK.Web;

    using LangCache = SparkTech.SDK.Cache.EnumCache<Enumerations.Language>;

    /// <summary>
    /// The variable storage and menu initializer
    /// </summary>
    [Trigger]
    public static class Creator
    {
        /// <summary>
        /// Determines whether this run is a first one in the specified environment
        /// </summary>
        public static readonly bool FirstRun;

        /// <summary>
        /// Determines whether this assembly is eligible for the premium features
        /// </summary>
        public static readonly bool Licensed;

        /// <summary>
        /// The SDK menu
        /// </summary>
        public static readonly MainMenu MainMenu;

        /// <summary>
        /// The language used by the machine
        /// </summary>
        public static readonly Language SystemLanguage;

        /// <summary>
        /// The currently used target language
        /// </summary>
        public static Language Language { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="Creator"/> class
        /// </summary>
        static Creator()
        {
            var licensing = new LicenseLink("146f7c3c-e5aa-4529-84a7-cf2cf648f69d");

            Licensed = licensing.IsOwned("SparkTech.SDK");

            SystemLanguage = LangCache.Values.Find(lang => LangCache.Description(lang) == CultureInfo.InstalledUICulture.Name);

            var replacements = new ReservedCollection
                                   {
                                       ["licenseStatus"] = Licensed.ToString
                                   };

            MainMenu = new MainMenu("st.sdk", "st_sdk", GetTranslations, replacements)
                           {
                               new QuickMenu("st.sdk.about")
                                   {
                                       ["st.sdk.about.language"] = new MenuItem("st_sdk_about_language", LangCache.Names),
                                       ["st.sdk.about.shop"] = new MenuItem("st_sdk_about_shop", false),
                                       ["st.sdk.about.license"] = new MenuItem("st_sdk_about_license")
                                   }
                           };

            #region FirstInit
            {
                var first = new MenuItem("error", false) { Instance = { IsVisible = false } };
                MainMenu.Add("st.sdk.first", first);
                FirstRun = first;
                first.Bool = false;
            }
            #endregion

            var languageItem = MainMenu.GetMenu("st.sdk.about")["st.sdk.about.language"];

            if (FirstRun)
            {
                languageItem.StringIndex = LangCache.Values.IndexOf(SystemLanguage);
            }

            Language = languageItem.Enum<Language>();

            languageItem.PropertyChanged += delegate
                {
                    Language = languageItem.Enum<Language>();
                    
                    MainMenu.Refresh();
                };

            MainMenu.GetMenu("st.sdk.about")["st.sdk.about.shop"].PropertyChanged += args =>
                {
                    args.Process = false;

                    var thread = new Thread(() =>
                        {
                            var path = licensing.GetShopLink();

                            CodeFlow.Secure(delegate
                                    {
                                        if (path == null)
                                        {
                                            Comms.Print(MainMenu.GetTranslation("st_sdk_license_token_fail"));
                                            return;
                                        }
                                        
                                        Clipboard.SetText(path);
                                        Comms.Print(MainMenu.GetTranslation("st_sdk_license_token_success"));
                                    });
                        });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                };

            Console.WriteLine("FirstRun: " + FirstRun);
            Console.WriteLine("Language: " + Language);
            Console.WriteLine("SystemLanguage: " + SystemLanguage);
            Console.WriteLine("Licensed: " + Licensed);

            CodeFlow.Secure(Bootstrap.Release);
        }

        private static Dictionary<string, string> GetTranslations(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["error"] = "ERROR",
                                   ["st_sdk_license_token_success"] = "Link copied to clipboard!",
                                   ["st_sdk_license_token_fail"] = "Failed to obtain a token!",
                                   ["st_sdk"] = "SparkTech.SDK",
                                   ["st_sdk_about"] = "About",
                                   ["st_sdk_about_language"] = "Language",
                                   ["st_sdk_about_shop"] = "Copy shop link",
                                   ["st_sdk_about_license"] = "Subscription owned: {licenseStatus}"
                    };
                case Language.Polish:
                    return new Dictionary<string, string>
                               {
                                   ["st_sdk_license_token_success"] = "Link skopiowany do schowka!",
                                   ["st_sdk_license_token_fail"] = "Wystąpił błąd przy generowaniu tokena!",
                                   ["st_sdk_about"] = "Informacje",
                                   ["st_sdk_about_language"] = "Język",
                                   ["st_sdk_about_shop"] = "Skopiuj link do sklepu",
                                   ["st_sdk_about_license"] = "Status subskrypcji: {licenseStatus}"
                    };
            }
        }

        /*

#region Version
var version = MainMenu.AddSubMenu("Version", "st.sdk.version");
var allow = new MenuItem("Allow update checks", true);
version.Add("st.sdk.info.version.check", allow);
var sdkVerion = new MenuItem(allow ? "Version - Checking failure!" : "Update checks disabled");
version.Add("st.sdk.info.version", sdkVerion);

if (allow)
{


    var assemblyName = Assembly.GetName();

    new SparkTechUpdater(assemblyName.Version, assemblyName.Name, "SDK").CheckPerformed += args =>
        {
            if (!args.Success)
            {
                return;
            }

            sdkVerion.DisplayName = args.IsUpdated
                                        ? "Your copy of SparkTech.SDK is up to date"
                                        : "A new update is available!";

            args.Notify();
        };


}
#endregion

*/

        /*

        #region Info
        {
            var info = MainMenu.AddSubMenu("About", "st.sdk.info");
            info.AddLabel($"Welcome, \"{SandboxConfig.Username}\" :)");
            info.AddLabel($"License type: {(SandboxConfig.IsBuddy ? "Buddy" : "Pleb")}");
            info.AddSeparator();
            info.AddLabel("Please report any bugs or suggestions at:");
            info.AddLabel("Skype: \"wiktorsharp\"");
            info.AddLabel("Discord: @spark");
        }
        #endregion

        */
    }
}