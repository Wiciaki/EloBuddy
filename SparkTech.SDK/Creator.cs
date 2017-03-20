namespace SparkTech.SDK
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
        /// The main menu of the SDK
        /// </summary>
        public static readonly MainMenu MainMenu;

        /// <summary>
        /// The license server for the assembly
        /// </summary>
        public static readonly LicenseServer LicenseServer = new LicenseServer("146f7c3c-e5aa-4529-84a7-cf2cf648f69d");

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
            Console.Title = "Obtaining license...";

            Bootstrap.Release();

            Licensed = LicenseServer.CheckOwned("SparkTech.SDK");

            Console.Title = "SparkTech.SDK";

            SystemLanguage = LangCache.Values.Find(lang => LangCache.Description(lang) == CultureInfo.InstalledUICulture.Name);

            var replacements = new ReservedCollection
                                   {
                                       ["licenseStatus"] = Licensed.ToString
                                   };

            MainMenu = new MainMenu("st.sdk", "st_sdk", GetTranslations, replacements)
                           {
                               new QuickMenu("update"),

                               new QuickMenu("license")
                                   {
                                       ["license.shop"] = new MenuItem("license_shop", false),
                                       ["separator1"] = new MenuItem(10),
                                       ["license.status"] = new MenuItem("license_status", null, true),
                                       ["separator2"] = new MenuItem(),
                                       ["license.note"] = new MenuItem("license_note")
                                   },

                               { "language", new MenuItem("language", LangCache.Names) },
                               { "separator1", new MenuItem() },
                               { "contact", new MenuItem("contact") },
                               { "comms.note", new MenuItem("i_dont_speak_spaghetti", () => Language != Language.English && Language != Language.Polish) }
                           };

            #region FirstInit
            {
                var first = new MenuItem("error", false) { Instance = { IsVisible = false } };
                MainMenu.Add("first", first);
                FirstRun = first;
                first.Bool = false;
            }
            #endregion

            var languageItem = MainMenu["language"];

            if (FirstRun)
            {
                languageItem.StringIndex = LangCache.Values.IndexOf(SystemLanguage);
            }

            Language = languageItem.Enum<Language>();

            languageItem.PropertyChanged += args =>
                {
                    Language = args.Sender.Enum<Language>();

                    MainMenu.Rebuild();
                };

            MainMenu.GetMenu("license")["license.shop"].PropertyChanged += args =>
                {
                    args.Process = false;

                    var thread = new Thread(() =>
                        {
                            var path = LicenseServer.GetShopLink();

                            CodeFlow.Secure(delegate
                                {
                                    if (path == null)
                                    {
                                        Comms.Print(MainMenu.GetTranslation("token_fail"));
                                        return;
                                    }

                                    Clipboard.SetText(path);
                                    Comms.Print(MainMenu.GetTranslation("token_success"));
                                });
                        });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                };

            Console.WriteLine();
            Console.WriteLine("|====== SparkTech.SDK variables ======|");
            Console.WriteLine("|          FirstRun: " + FirstRun + "            |");
            Console.WriteLine("|         Language: " + Language + "           |");
            Console.WriteLine("|     SystemLanguage: " + SystemLanguage + "         |");
            Console.WriteLine("|         Licensed: " + Licensed + "             |");
            Console.WriteLine("|=====================================|");
            Console.WriteLine();
        }

        private static Dictionary<string, string> GetTranslations(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["error"] = "ERROR",

                                   ["st_sdk"] = "SparkTech.SDK",

                                   ["token_success"] = "Link copied to clipboard!",
                                   ["token_fail"] = "Failed to obtain a token!",

                                   #region Updater

                                   ["update"] = "Updates",

                                   ["update_note_sdk"] = "SparkTech.SDK version status:",
                                   ["update_note_allypingspammer"] = "Pinging capabilities of AllyPingSpammer:",

                                   ["updated_yes_sdk"] = "You are using the updated version, which is {sdk}",
                                   ["updated_no_sdk"] = "A new update is available! Please update it in the loader! {sdk}",

                                   ["updated_yes_allypingspammer"] = "Feel free to ping to your limits. Version is {allypingspammer}",
                                   ["updated_no_allypingspammer"] = "Not enough pings, please update! {allypingspammer}",

                                   ["update_available"] = "Updates are available. Check the menu for more details.",

                                   #endregion

                                   ["license"] = "Subscription",
                                   ["license_shop"] = "Press to generate an unique shop link",
                                   ["license_status"] = "Subscription owned: {licenseStatus}",
                                   ["license_note"] = "A subscription allows you to use premium features like an exclusive orbwalker, target selector,\nas well as allows early access to beta addons. It's also a nice way to keep me motivated.\nPlease visit the shop website to find our more.",

                                   ["language"] = "Language",
                                   ["contact"] = "Thank you for using my software.\nIf you encounter any bugs or have any suggestions, please contact me at:\nDiscord: \"Spark\"\nSkype: \"wiktorsharp\"",
                                   ["i_dont_speak_spaghetti"] = "You shouldn't be seeing this.",
                               };
                case Language.Polish:
                    return new Dictionary<string, string>
                               {
                                   ["token_success"] = "Link skopiowany do schowka!",
                                   ["token_fail"] = "Wystapil blad przy generowaniu tokena!",

                                   #region Updater

                                   ["update"] = "Aktualizacje",

                                   ["update_note_sdk"] = "Status wersji SparkTech.SDK:",
                                   ["update_note_allypingspammer"] = "Moc pingowania AllyPingSpammera:",

                                   ["updated_yes_sdk"] = "Używasz aktualnej wersji ({sdk})",
                                   ["updated_no_sdk"] = "Nowa wersja dostępna, proszę zaktualizować w loaderze {sdk}",

                                   ["updated_yes_allypingspammer"] = "Pinguj bez ograniczeń. Wersja: {allypingspammer}",
                                   ["updated_no_allypingspammer"] = "Niedobór pingów! Proszę, zaktualizuj: {allypingspammer}",

                                   ["update_available"] = "Aktualizacje są dostępne. Sprawdź menu, by poznać szczegóły.",

                                   #endregion

                                   ["license"] = "Subskrypcja",
                                   ["license_shop"] = "Kliknij, by utworzyć unikalny link do sklepu",
                                   ["license_status"] = "Status subskrypcji: {licenseStatus}",
                                   ["license_note"] = "Subskrypcja pozwala na używanie funkcji premium, takich jak dedykowany orbwalker,\ntarget selector, czy też dostęp do addonów w fazie testowej. Pomaga mi też utrzymać motywację,\njak i wysoką jakość addonów.\nOdwiedź stronę sklepu, by dowiedzieć się więcej",

                                   ["language"] = "Język",
                                   ["contact"] = "Dziękuję za używanie mojego oprogramowania.\nJeśli zauważysz bugi lub masz sugestie, napisz:\nDiscord: \"Spark\"\nSkype: \"wiktorsharp\"",
                    };
            }
        }
    }
}