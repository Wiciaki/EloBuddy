﻿namespace AllyPingSpammer
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    using SparkTech.SDK;
    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;
    using SparkTech.SDK.Utils;

    using static SparkTech.SDK.Shortcuts;

    using Color = System.Drawing.Color;

    /// <summary>
    /// The logic class
    /// </summary>
    [Trigger]
    public static class PingSpammer
    {
        /// <summary>
        /// The main menu for this addon
        /// </summary>
        private static readonly MainMenu MainMenu;

        /// <summary>
        /// The current target to be pinged
        /// </summary>
        private static AIHeroClient targetHero;

        /// <summary>
        /// The list of allies heroes (subjects to spammming)
        /// </summary>
        private static readonly List<AIHeroClient> Allies;

        /// <summary>
        /// The time of the last ping
        /// </summary>
        private static int lastPingTime;

        /// <summary>
        /// The length of the last delay set
        /// </summary>
        private static int delay;

        /// <summary>
        /// The entry point for an addon
        /// </summary>
        static PingSpammer()
        {
            Allies = ObjectCache.GetNative<AIHeroClient>().FindAll(h => h.Team() == ObjectTeam.Ally);
            Allies.RemoveAt(Allies.FindIndex(champ => champ.IsMe));

            if (Allies.Count == 0)
            {
                Comms.Print(GenerateTranslations(Creator.Language)["notify_no_allies"]);
                return;
            }

            MainMenu = new MainMenu("st.ping", "st_ping", GenerateTranslations)
                           {
                               ["hero"] = new MenuItem("spam_who", Allies.ConvertAll(ally => ally.UniqueName())),
                               ["separator1"] = new MenuItem(),
                               ["pingtype"] = new MenuItem("pingtype", EnumCache<PingCategory>.Names),
                               ["separator2"] = new MenuItem(10),
                               ["pingtype.rand"] = new MenuItem("pingtype_rand", false),
                               ["polygon"] = new MenuItem("polygon", false),
                               ["separator3"] = new MenuItem(),
                               ["active"] = new MenuItem("active", false),
                               ["separator4"] = new MenuItem(),
                               ["button"] = new MenuItem("button", false, KeyBind.BindTypes.HoldActive, 'H', 'J')
            };

            MainMenu.Add(new QuickMenu("advanced")
                         {
                             ["delay"] = new MenuItem("delay", 3000, 200, 10000),
                             ["delay.rand"] = new MenuItem("delay_rand", false),
                             ["separator1"] = new MenuItem(),
                             ["difference"] = new MenuItem("difference", 200, 20, 800),
                             ["hide"] = new MenuItem("hide", false),
                             ["hide.text"] = new MenuItem("hide_text")
            });

            MainMenu["hero"].PropertyChanged += args => AssignHero();

            Chat.OnClientSideMessage += arg =>
                {
                    if (MainMenu.GetMenu("advanced")["hide"] && arg.Message == "You must wait before issuing more pings.")
                    {
                        arg.Process = false;
                    }
                };

            CodeFlow.Secure(delegate
                    {
                        MainMenu["active"].Bool = false;

                        Game.OnTick += OnTick;
                    });

            Drawing.OnDraw += OnDraw;
        }

        private static void OnTick(EventArgs eventArgs)
        {
            if (MainMenu["button"])
            {
                Illuminati(Game.CursorPos.To2D());
                return;
            }

            if (!MainMenu["active"])
            {
                return;
            }

            if (MainMenu["polygon"])
            {
                Illuminati(targetHero.Position.To2D());
                return;
            }

            var time = Game.Time.ToTicks();

            if (lastPingTime + delay > time)
            {
                return;
            }

            OnPinged(time);

            TacticalMap.SendPing(MainMenu["pingtype"].Enum<PingCategory>(), Randomization.Randomize(targetHero.Position.To2D(), MainMenu.GetMenu("advanced")["difference"]));

            if (MainMenu.GetMenu("advanced")["delay.rand"])
            {
                MainMenu["delay"].Int = RandomInst.Next(200, 10000);
            }
        }

        private static void AssignHero()
        {
            targetHero = Allies.Find(hero => hero.UniqueName() == MainMenu["hero"]);
        }

        private static void OnPinged(int time)
        {
            if (MainMenu["pingtype.rand"])
            {
                MainMenu["pingtype"].StringIndex = RandomInst.Next(0, EnumCache<PingCategory>.Count - 1);
            }

            lastPingTime = time;
        }

        /// <summary>
        /// Performs an illuminati ping at the specified location
        /// </summary>
        /// <param name="pos"></param>
        private static void Illuminati(Vector2 pos)
        {
            var time = Game.Time.ToTicks();

            if (lastPingTime + delay > time + 10)
            {
                return;
            }

            delay = RandomInst.Next(6000, 20000);

            OnPinged(time);

            var type = MainMenu["pingtype"].Enum<PingCategory>();
            var diff = MainMenu.GetMenu("advanced")["difference"] / 10;

            for (var i = 0; i < 6; i++)
            {
                var target = new Vector2(pos.X + 500f * (float)Math.Cos(i), pos.Y + 500f * (float)Math.Sin(i));

                TacticalMap.SendPing(type, Randomization.Randomize(target, diff));
            }
        }

        /// <summary>
        /// Executes every time a frame is being drawn
        /// </summary>
        /// <param name="args"></param>
        private static void OnDraw(EventArgs args)
        {
            if (!MainMenu["button"])
            {
                return;
            }

            var pos = Drawing.WorldToScreen(Game.CursorPos);
            var time = (int)((lastPingTime + delay - Game.Time.ToTicks()) / 1000f);

            Drawing.DrawText(pos.X - 30, pos.Y + 50, Color.Gold, MainMenu.GetTranslation("time_to_ping").Replace("[TIME]", $"{time}"));
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
                                   ["st_ping"] = "AllyPingSpammer",

                                   ["notify_no_allies"] = "[AllyPingSpammer] No allies detected, the addon will now quit...",
                                   ["time_to_ping"] = "Pinging in: [TIME]",

                                   ["spam_who"] = "Ally to be spammed",
                                   ["pingtype"] = "Ping category",
                                   ["pingtype_rand"] = "^ Randomize ping category",
                                   ["polygon"] = "Use illuminati polygon pings",
                                   ["active"] = "TILT THE SHIT OUT OF MOTHERFUCKER",
                                   ["button"] = "Press for an illuminati ping at mouse location",

                                   ["advanced"] = "Advanced",
                                   ["delay"] = "Delay between single pings",
                                   ["delay_rand"] = "^ Randomize delay",
                                   ["difference"] = "Maximal click point randomization",
                                   ["hide"] = "Chat blocker active",
                                   ["hide_text"] = "Prevents \"You have to wait before issuing more pings.\" from displaying in your chat\n"
                    };
                case Language.Polish:
                    return new Dictionary<string, string>
                               {
                                   ["notify_no_allies"] = "[AllyPingSpammer] Nie wykryto sojuszników, addon się nie ładuje...",
                                   ["time_to_ping"] = "Pingi za: [TIME]",

                                   ["spam_who"] = "Sojusznik do spamowania",
                                   ["pingtype"] = "Rodzaj pingu",
                                   ["pingtype_rand"] = "^ Losowy rodzaj pingu",
                                   ["polygon"] = "Używaj sześciokątnych illuminati pingów",
                                   ["active"] = "WKURWIAJ TEGO DEBILA",
                                   ["button"] = "Wciśnij, by pingować illuminati na żądanie",

                                   ["advanced"] = "Zaawansowane",
                                   ["delay"] = "Opóźnienie pomiędzy pojedyńczymi pingami",
                                   ["delay_rand"] = "^ Losowe opóźnienie",
                                   ["difference"] = "Maksymalna odległość od celu",
                                   ["hide"] = "Blokuj chat",
                                   ["hide_text"] = "Blokuje \"Musisz poczekać, by wyświetlić więcej pingów.\" przed wyświetlaniem się na czacie"
                    };
            }
        }
    }
}