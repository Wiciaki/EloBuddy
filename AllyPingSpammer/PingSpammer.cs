namespace AllyPingSpammer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using EloBuddy;
    using EloBuddy.SDK;

    using SparkTech.SDK;
    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Utils;

    /// <summary>
    /// The logic class
    /// </summary>
    [Trigger]
    public static class PingSpammer
    {
        /// <summary>
        /// The .cctor invoked by internal bootstrap methods in the SDK
        /// </summary>
        [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure", Justification = "These object should not be a subject to garbage collection.")]
        static PingSpammer()
        {
            var allies = ObjectCache.GetNative<AIHeroClient>().FindAll(h => h.Team() == ObjectTeam.Ally);
            allies.RemoveAt(allies.FindIndex(champ => champ.IsMe));

            if (allies.Count == 0)
            {
                Chat.Print(GenerateTranslations(Creator.Language)["st_ping_notify_noallies"]);
                return;
            }

            var root = new MainMenu("st.ping", "st_ping", GenerateTranslations);
            var hero = root.Add("st.ping.hero", new MenuItem("st_ping_spam_who", allies.ConvertAll(ally => ally.UniqueName())));
            root.AddSeparator(30);
            var active = root.Add("st.ping.active", new MenuItem("st_ping_spam_active", false));
            var advanced = root.AddSubMenu("st.ping.advanced", "st_ping_advanced");
            var delay = advanced.Add("st.ping.delay", new MenuItem("st_ping_advanced_delay", 3000, 200, 10000));
            var delayRandomizer = advanced.Add("st.ping.randomize1", new MenuItem("st_ping_advanced_delay_randmize", false));
            advanced.AddSeparator();
            var ping = advanced.Add("st.ping.pingtype", new MenuItem("st_ping_advanced_pingcategory", EnumCache<PingCategory>.Names));
            var categoryRandomizer = advanced.Add("st.ping.randomize2", new MenuItem("st_ping_advanced_pingcategory_randomize", false));
            advanced.AddSeparator();
            var difference = advanced.Add("st.ping.difference", new MenuItem("st_ping_advanced_difference", 200, 20, 800));
            advanced.AddSeparator();
            var hider = advanced.Add("st.ping.hide", new MenuItem("st_ping_advanced_hide", false));
            advanced.AddLabel("st_ping_advanced_hide_text");

            var selectedHero = allies.Find(champ => champ.UniqueName() == hero.String);
            var r = new Random();
            var lastPing = 0;

            hero.PropertyChanged += p => selectedHero = allies.Find(champ => champ.UniqueName() == hero.String);

            Game.OnTick += delegate
                {
                    if (!active)
                    {
                        return;
                    }

                    var time = Game.Time.ToTicks();

                    if (lastPing > time - delay)
                    {
                        return;
                    }

                    lastPing = time;

                    TacticalMap.SendPing(ping.Enum<PingCategory>(), Randomization.Randomize(selectedHero.Position.To2D(), difference));

                    if (delayRandomizer)
                    {
                        delay.Int = r.Next(200, 10000);
                    }

                    if (categoryRandomizer)
                    {
                        ping.StringIndex = r.Next(0, EnumCache<PingCategory>.Count - 1);
                    }
                };

            Chat.OnClientSideMessage += arg =>
                {
                    if (hider.Bool && arg.Message == "You must wait before issuing more pings.")
                    {
                        arg.Process = false;
                    }
                };

            CodeFlow.Secure(() => active.Bool = false);
        }

        /// <summary>
        /// The generator of the translations
        /// </summary>
        /// <param name="language">The specified language</param>
        /// <returns></returns>
        private static Dictionary<string, string> GenerateTranslations(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["st_ping_notify_noallies"] = "[AllyPingSpammer] No allies detected, the addon will now quit...",
                                   ["st_ping"] = "AllyPingSpammer",
                                   ["st_ping_spam_who"] = "Ally to be spammed",
                                   ["st_ping_spam_active"] = "TILT THE SHIT OUT OF MOTHERFUCKER",
                                   ["st_ping_advanced"] = "Advanced",
                                   ["st_ping_advanced_delay"] = "Delay between pings",
                                   ["st_ping_advanced_delay_randmize"] = "^ Randomize delay",
                                   ["st_ping_advanced_pingcategory"] = "Ping category",
                                   ["st_ping_advanced_pingcategory_randomize"] = "^ Randomize ping category",
                                   ["st_ping_advanced_difference"] = "Maximal click point randomization",
                                   ["st_ping_advanced_hide"] = "Chat blocker active",
                                   ["st_ping_advanced_hide_text"] = "Prevents \"You have to wait before issuing more pings.\" from displaying in your chat"
                    };
                case Language.Polish:
                    return new Dictionary<string, string>
                               {
                                   ["st_ping_notify_noallies"] = "[AllyPingSpammer] Nie wykryto sojuszników, addon się nie ładuje...",
                                   ["st_ping_spam_who"] = "Sojusznik do spamowania",
                                   ["st_ping_spam_active"] = "WKURWIAJ TEGO DEBILA",
                                   ["st_ping_advanced"] = "Zaawansowane",
                                   ["st_ping_advanced_delay"] = "Opóźnienie pomiędzy pingami",
                                   ["st_ping_advanced_delay_randmize"] = "^ Losowe opóźnienie",
                                   ["st_ping_advanced_pingcategory"] = "Rodzaj pingu",
                                   ["st_ping_advanced_pingcategory_randomize"] = "^ Losowy rodzaj pingu",
                                   ["st_ping_advanced_difference"] = "Maksymalna różnica kliknięć",
                                   ["st_ping_advanced_hide"] = "Blokuj chat",
                                   ["st_ping_advanced_hide_text"] = "Blokuje \"Musisz poczekać, by wyświetlić więcej pingów.\" przed wyświetlaniem się na chacie"
                    };
            }
        }
    }
}