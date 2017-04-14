namespace SparkTech.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Web;

    using LangCache = SparkTech.SDK.Cache.EnumCache<Enumerations.Language>;

    /// <summary>
    /// The variable storage and menu initializer
    /// </summary>
    public static class Creator
    {
        /// <summary>
        /// Determines whether this run is a first one in the specified environment
        /// </summary>
        public static readonly bool FirstRun;

        /// <summary>
        /// The main menu of the SDK
        /// </summary>
        public static readonly MainMenu MainMenu;

        /// <summary>
        /// The currently used target language
        /// </summary>
        public static Language Language { get; private set; }

        /// <summary>
        /// The language used by the machine
        /// </summary>
        public static readonly Language SystemLanguage;

        /// <summary>
        /// Determines whether this assembly is eligible for the premium features
        /// </summary>
        public static readonly bool Licensed;

        /// <summary>
        /// Determines 
        /// </summary>
        public static readonly DateTime SubscriptionExpiry;

        /// <summary>
        /// The license server for the assembly
        /// </summary>
        public static readonly LicenseServer LicenseServer = new LicenseServer("146f7c3c-e5aa-4529-84a7-cf2cf648f69d");

        /// <summary>
        /// Initializes static members of the <see cref="Creator"/> class
        /// </summary>
        static Creator()
        {
            Licensed = LicenseServer.GetSubscription("SparkTech.SDK", out SubscriptionExpiry);

            var replacements = new ReservedCollection
                                   {
                                       ["licenseStatus"] = () => Licensed ? "✔" : "✘",
                                       ["subExpiry"] = delegate
                                           {
                                               if (MainMenu == null)
                                               {
                                                   return "null";
                                               }

                                               if (!Licensed)
                                               {
                                                   return "-/-";
                                               }

                                               var span = SubscriptionExpiry - DateTime.Now;
                                               var days = span.Days;

                                               if (days > 3650)
                                               {
                                                   return "∞";
                                               }

                                               if (days > 0)
                                               {
                                                   return days + " " + MainMenu.GetTranslation("days");
                                               }

                                               return span.Hours + " " + MainMenu.GetTranslation("hours");
                                           }
                                   };

            MainMenu = new QuickMainMenu("st_sdk", GetTranslations, replacements, "༼ つ ◕_◕ ༽つ")
                           {
                               new QuickMenu("features")
                                   {
                                       ["premium_label"] = new MenuItem("features_premium"),
                                       ["orbwalker"] = new MenuItem("use_xorbwalker", true),
                                       ["targetselector"] = new MenuItem("use_xtargetselector", true),
                                       ["common_label"] = new MenuItem("features_common"),
                                       ["indicator"] = new MenuItem("use_indicator", true)
                                   },

                               new QuickMenu("update"),

                               new QuickMenu("license")
                                   {
                                       ["shop"] = new MenuItem("license_shop", false),
                                       ["separator1"] = new MenuItem(10),
                                       ["status"] = new MenuItem("license_status", null, true),
                                       ["separator2"] = new MenuItem(),
                                       ["note"] = new MenuItem("license_note")
                                   },

                               { "language", new MenuItem("language", LangCache.Names) },
                               { "separator1", new MenuItem(8) },
                               { "bugs.notice", new MenuItem("bugs_notice") },
                               { "separator2", new MenuItem(10) },
                               { "contact", new MenuItem("contact") },
                               { "separator3", new MenuItem(10) },
                               { "lang.notice", new MenuItem("i_dont_speak_spaghetti", IsLanguageUnknown) }
                           };

            var first = new MenuItem("error", true) { Instance = { IsVisible = false } };
            MainMenu.Add("first", first);
            FirstRun = first;
            first.Bool = false;

            var languageItem = MainMenu["language"];

            var substring = CultureInfo.InstalledUICulture.Name.Substring(0, 2);

            SystemLanguage = LangCache.Values.Find(lang => LangCache.Description(lang) == substring);

            if (FirstRun)
            {
                languageItem.StringIndex = LangCache.Values.IndexOf(SystemLanguage);
            }

            Language = languageItem.Enum<Language>();

            // This is necessary due to Language variable being assigned after the menu is actually built
            if (Language == default(Language))
            {
                MainMenu.GetMenu("license")["status"].UpdateText();
            }
            else
            {
                MainMenu.Rebuild();
            }

            languageItem.PropertyChanged += args =>
                {
                    Language = languageItem.Enum<Language>();

                    MainMenu.Rebuild();
                };

            MainMenu.GetMenu("license")["shop"].PropertyChanged += args =>
                {
                    args.Process = false;

                    var path = LicenseServer.GetShopLink();

                    if (path == null)
                    {
                        MainMenu.Print("token_fail");
                        return;
                    }

                    Clipboard.SetText(path);

                    MainMenu.Print("token_success");
                };

            var features = MainMenu.GetMenu("features");

            if (!Licensed)
            {
                features["orbwalker"].Bool = false;
                features["orbwalker"].PropertyChanged += OnBlocked;

                features["targetselector"].Bool = false;
                features["targetselector"].PropertyChanged += OnBlocked;
            }
            else
            {
                
            }

            if (FirstRun)
            {
                MainMenu.Print("welcome");
            }
        }

        /// <summary>
        /// Executes when a MenuItem is being blocked
        /// </summary>
        /// <param name="args">The event data</param>
        private static void OnBlocked(ValueChangedEventArgs args)
        {
            args.Process = false;

            MainMenu.Print("premium_required");
        }

        /// <summary>
        /// Determines whether the current language is unknown
        /// </summary>
        /// <returns>Value determining whether the current language I don't speak :)</returns>
        private static bool IsLanguageUnknown()
        {
            switch (Language)
            {
                case Language.English:
                case Language.Polish:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Generates translations for the specified language
        /// </summary>
        /// <param name="language">The specified language</param>
        /// <returns>A dictionary with all the translations available</returns>
        private static Dictionary<string, string> GetTranslations(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["error"] = "ERROR",
                                   ["st_sdk"] = "SparkTech.SDK",

                                   ["days"] = "days",
                                   ["hours"] = "hours",

                                   ["token_success"] = "Link copied to clipboard!",
                                   ["token_fail"] = "Failed to obtain a token!",

                                   ["updater_failure"] = "Couldn't get update data for [NAME]",
                                   ["updater_updated"] = "You're using the updated version of [NAME].",
                                   ["updater_outdated"] = "New update for [NAME] is available!",

                                   ["update"] = "Updates",

                                   ["update_note_sdk"] = "SparkTech.SDK version status:",
                                   ["updated_yes_sdk"] = "You are using the updated version, which is {sdkVersion}",
                                   ["updated_no_sdk"] = "A new update is available! Please update it in the loader! {sdkVersion}",

                                   ["update_note_allypingspammer"] = "Pinging capabilities of AllyPingSpammer:",
                                   ["updated_yes_allypingspammer"] = "Feel free to ping to your limits. Version is {allypingspammerVersion}",
                                   ["updated_no_allypingspammer"] = "Not enough pings, please update! {allypingspammerVersion}",

                                   ["update_note_lissandra"] = "Lissandra version:",
                                   ["updated_yes_lissandra"] = "FREEEEZE! ({lissandraVersion})",
                                   ["updated_no_lissandra"] = "Need more ice cubes... Update is available! {lissandraVersion}",

                                   ["update_available"] = "Updates are available. Check the menu for more details.",

                                   ["license"] = "Subscription",
                                   ["license_shop"] = "Press to generate an unique shop link",
                                   ["license_status"] = "SDK Subscription owned: {licenseStatus}\nExpires in: {subExpiry}",
                                   ["license_note"] = "Subscription allows you to use premium features like an exclusive orbwalker, target selector,\nas well as allows early access to beta addons. It's also a nice way of keeping me motivated.\nPlease visit the shop website to find our more.",

                                   ["features"] = "Features",
                                   ["features_premium"] = "Premium features (subscribers only)",
                                   ["features_common"] = "Common features",
                                   ["use_xorbwalker"] = "Enable SparkWalker",
                                   ["use_xtargetselector"] = "Enable XTargetSelector",
                                   ["use_indicator"] = "Draw damage on enemies' health bars",

                                   ["language"] = "Language",
                                   ["bugs_notice"] = "Thank you for using my software.\nIf you encounter any bugs or have any suggestions, please contact me at:",
                                   ["contact"] = "Discord: \"Spark#7596\"\nSkype: \"wiktorsharp\"",
                                   ["i_dont_speak_spaghetti"] = "Please note I don't speak this language.\nTranslation credits: (...)",

                                   ["welcome"] = "Welcome! Please, make yourself familiar with the menu first",
                                   ["premium_required"] = "This is a premium feature and requires you to subscribe"
                               };
                case Language.Polish:
                    return new Dictionary<string, string>
                               {
                                   ["days"] = "dni",
                                   ["hours"] = "godziny",

                                   ["token_success"] = "Link skopiowany do schowka!",
                                   ["token_fail"] = "Wystapil blad przy generowaniu tokena!",

                                   ["updater_failure"] = "Nie mozna sprawdzic aktualizacji dla [NAME]",
                                   ["updater_updated"] = "Uzywasz aktualnej wersji [NAME].",
                                   ["updater_outdated"] = "Nowsza wersja [NAME] jest dostepna!",

                                   ["update"] = "Aktualizacje",

                                   ["update_note_sdk"] = "Status wersji SparkTech.SDK:",
                                   ["updated_yes_sdk"] = "Używasz aktualnej wersji ({sdkVersion})",
                                   ["updated_no_sdk"] = "Nowa wersja dostępna, proszę zaktualizować w loaderze! {sdkVersion}",

                                   ["update_note_allypingspammer"] = "Moc pingowania AllyPingSpammera:",
                                   ["updated_yes_allypingspammer"] = "Pinguj bez ograniczeń. Wersja: {allypingspammerVersion}",
                                   ["updated_no_allypingspammer"] = "Niedobór pingów! Proszę, zaktualizuj: {allypingspammerVersion}",

                                   ["update_note_lissandra"] = "Wersja Lissandry:",
                                   ["updated_yes_lissandra"] = "Aktualna. ({lissandraVersion})",
                                   ["updated_no_lissandra"] = "Deficyt kostek lodu.... aktualizacja potrzebna! {lissandraVersion}",

                                   ["update_available"] = "Aktualizacje sa dostepne. Sprawdz menu, by poznac szczegoly.",

                                   ["license"] = "Subskrypcja",
                                   ["license_shop"] = "Kliknij, by utworzyć unikalny link do sklepu",
                                   ["license_status"] = "Status subskrypcji SDK: {licenseStatus}\nWygasa za: {subExpiry}",
                                   ["license_note"] = "Subskrypcja pozwala na używanie funkcji premium, takich jak dedykowany orbwalker,\ntarget selector, czy też dostęp do addonów w fazie testowej. Pomaga mi też utrzymać motywację,\njak i wysoką jakość addonów.\nOdwiedź stronę sklepu, by dowiedzieć się więcej",

                                   ["features"] = "Funkcje",
                                   ["features_premium"] = "Funkcje premium (tylko dla subskrybentów)",
                                   ["features_common"] = "Zwykłe funkcje",
                                   ["use_xorbwalker"] = "Aktywuj SparkWalker",
                                   ["use_xtargetselector"] = "Aktywuj XTargetSelector",
                                   ["use_indicator"] = "Pasek obrażeń na zdrowiu przeciwnika",

                                   ["language"] = "Język",
                                   ["bugs_notice"] = "Dziękuję za używanie mojego oprogramowania.\nJeśli zauważysz bugi lub masz sugestie, napisz:",

                                   ["welcome"] = "Witaj! Prosze, zapoznaj sie najpierw z menu",
                                   ["premium_required"] = "To jest funkcja premium i wymaga posiadania subskrypcji"
                    };
                case Language.German:
                    return new Dictionary<string, string>
                               {
                                   ["days"] = "days",
                                   ["hours"] = "hours",

                                   ["token_success"] = "Link copied to clipboard!",
                                   ["token_fail"] = "Failed to obtain a token!",

                                   ["updater_failure"] = "Couldn't get update data for [NAME]",
                                   ["updater_updated"] = "You're using the updated version of [NAME].",
                                   ["updater_outdated"] = "New update for [NAME] is available!",

                                   ["update"] = "Updates",

                                   ["update_note_sdk"] = "SparkTech.SDK version status:",
                                   ["updated_yes_sdk"] = "You are using the updated version, which is {sdkVersion}",
                                   ["updated_no_sdk"] = "A new update is available! Please update it in the loader! {sdkVersion}",

                                   ["update_note_allypingspammer"] = "Pinging capabilities of AllyPingSpammer:",
                                   ["updated_yes_allypingspammer"] = "Feel free to ping to your limits. Version is {allypingspammerVersion}",
                                   ["updated_no_allypingspammer"] = "Not enough pings, please update! {allypingspammerVersion}",

                                   ["update_note_lissandra"] = "Lissandra version:",
                                   ["updated_yes_lissandra"] = "FREEEEZE! ({lissandraVersion})",
                                   ["updated_no_lissandra"] = "Need more ice cubes... Update is available! {lissandraVersion}",

                                   ["update_available"] = "Updates are available. Check the menu for more details.",

                                   ["license"] = "Subscription",
                                   ["license_shop"] = "Press to generate an unique shop link",
                                   ["license_status"] = "SDK Subscription owned: {licenseStatus}\nExpires in: {subExpiry}",
                                   ["license_note"] = "Subscription allows you to use premium features like an exclusive orbwalker, target selector,\nas well as allows early access to beta addons. It's also a nice way of keeping me motivated.\nPlease visit the shop website to find our more.",

                                   ["features"] = "Features",
                                   ["features_premium"] = "Premium features (subscribers only)",
                                   ["features_common"] = "Common features",
                                   ["use_xorbwalker"] = "Enable SparkWalker",
                                   ["use_xtargetselector"] = "Enable XTargetSelector",
                                   ["use_indicator"] = "Draw damage on enemies' health bars",

                                   ["language"] = "Language",
                                   ["bugs_notice"] = "Thank you for using my software.\nIf you encounter any bugs or have any suggestions, please contact me at:",
                                   ["contact"] = "Discord: \"Spark#7596\"\nSkype: \"wiktorsharp\"",
                                   ["i_dont_speak_spaghetti"] = "Please note I don't speak this language.\nTranslation credits: (...)",

                                   ["welcome"] = "Welcome! Please, make yourself familiar with the menu first",
                                   ["premium_required"] = "This is a premium feature and requires you to subscribe"
                    };
            }
        }
    }
}