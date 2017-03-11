namespace SparkTech.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using EloBuddy.Sandbox;
    
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;

    using LangCache = SparkTech.SDK.Cache.EnumCache<Enumerations.Language>;

    /// <summary>
    /// The delegate used for passing argument-less Boolean pointers
    /// </summary>
    /// <returns></returns>
    public delegate bool Predicate();

    /// <summary>
    /// The delegate used to handle PropertyChanged events
    /// </summary>
    /// <param name="propertyName"></param>
    public delegate void PropertyChanged(string propertyName);

    /// <summary>
    /// The main event delegate used for handling most of the event data instances
    /// </summary>
    /// <typeparam name="TEventArgs">The destination event arguments</typeparam>
    /// <param name="args">The actual event data</param>
    public delegate void EventDataHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;

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
            SystemLanguage = LangCache.Values.Find(lang => LangCache.Description(lang) == CultureInfo.InstalledUICulture.Name);

            MainMenu = new MainMenu("st.sdk", "st_sdk", GetTranslations)
                           {
                               new Menu("st.sdk.about", "st_sdk_about")
                                   {
                                       ["st.sdk.about.language"] = new MenuItem("st_sdk_about_language", LangCache.Names)
                                   },
                /*
                               {
                                   "st.sdk.item1", new MenuItem("st_sdk_item1")
                               },
                               
                               new Menu("st.sdk.web", "st_sdk_web")
                               {
                                   ["random.label"] = new MenuItem("st_sdk_randomlabel")
                               },

                               new Menu("st_sdk_menu2", "st.sdk.menu2")
                               {
                                   ["random.label"] = new MenuItem("st_sdk_randomlabel")
                               },

                               {
                                   "st.sdk.item1", new MenuItem("st_sdk_item1")
                               },
                               {
                                   "st.sdk.item2", new MenuItem("st_sdk_item2")
                               },
                               {
                                   "st.sdk.item3", new MenuItem("st_sdk_item3")
                               }*/
                           };

            #region FirstInit
            {
                var first = new MenuItem("error", true) { Instance = { IsVisible = false } };
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
                    
                    MainMenu.GetAllComponents().ForEach(m => m.UpdateText());
                };

            Console.WriteLine("FirstRun: " + FirstRun);
            Console.WriteLine("Language: " + Language);
            Console.WriteLine("SystemLanguage: " + SystemLanguage);

            CodeFlow.Secure(Bootstrap.Release);

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

        private static Dictionary<string, string> GetTranslations(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["st_sdk"] = "SparkTech.SDK",
                                   ["error"] = "ERROR",
                                   ["st_sdk_about"] = "About",
                                   ["st_sdk_about_language"] = "Language",
                    };
                case Language.Polish:
                    return new Dictionary<string, string>
                               {
                                   ["st_sdk_about"] = "Informacje",
                                   ["st_sdk_about_language"] = "Język",
                    };
            }
        }
    }
}