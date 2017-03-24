namespace AllyPingSpammer
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;

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
        /// The translation obtainer
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
                                   ["notify_no_allies"] = "[AllyPingSpammer] Nie wykryto sojusznikow, addon sie nie laduje...",
                                   ["time_to_ping"] = "Pinguje za: [TIME]",

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
        private static int delay = 8000;

        /// <summary>
        /// The entry point for an addon
        /// </summary>
        static PingSpammer()
        {
            Allies = ObjectCache.GetNative<AIHeroClient>().FindAll(h => h.Team() == ObjectTeam.Ally);
            Allies.RemoveAll(h => h.IsMe);

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
                               ["button"] = new MenuItem("button", false, KeyBindType.Hold, 'H', 'J')
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

            AssignHero();

            MainMenu["hero"].PropertyChanged += args => AssignHero();
            MainMenu["active"].Bool = false;

            Chat.OnClientSideMessage += OnClientSideMessage;
            Game.OnTick += OnTick;
            Drawing.OnDraw += OnDraw;
        }

        /// <summary>
        /// Executes everytime a local chat message is being processed
        /// </summary>
        /// <param name="args">The event data</param>
        private static void OnClientSideMessage(ChatClientSideMessageEventArgs args)
        {
            if (MainMenu.GetMenu("advanced")["hide"] && args.Message == "You must wait before issuing more pings.")
            {
                args.Process = false;
            }
        }

        /// <summary>
        /// Executes every time the core sends a tick request
        /// </summary>
        /// <param name="args">The empty event data</param>
        private static void OnTick(EventArgs args)
        {
            if (MainMenu["button"])
            {
                Illuminati(Game.CursorPos);
                return;
            }

            if (!MainMenu["active"])
            {
                return;
            }

            if (MainMenu["polygon"])
            {
                Illuminati(targetHero.Position);
                return;
            }

            var time = Game.Time.ToTicks();

            if (lastPingTime + MainMenu.GetMenu("advanced")["delay"] > time)
            {
                return;
            }

            Pinged(time);

            TacticalMap.SendPing(MainMenu["pingtype"].Enum<PingCategory>(), Randomization.Randomize(targetHero.Position.ToVector2(), MainMenu.GetMenu("advanced")["difference"]));

            if (MainMenu.GetMenu("advanced")["delay.rand"])
            {
                MainMenu.GetMenu("advanced")["delay"].Int = RandomInst.Next(2000, 10000);
            }
        }

        /// <summary>
        /// Assigns a new hero to the targetHero variable
        /// </summary>
        private static void AssignHero()
        {
            targetHero = Allies.Find(hero => hero.UniqueName() == MainMenu["hero"]);
        }

        /// <summary>
        /// Sets up variables, delays and randomization after when the ping is executed
        /// </summary>
        /// <param name="time"></param>
        private static void Pinged(int time)
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
        private static void Illuminati(Vector3 pos)
        {
            var time = Game.Time.ToTicks();

            if (lastPingTime + delay > time + 10)
            {
                return;
            }

            delay = RandomInst.Next(6000, 20000);

            Pinged(time);

            var type = MainMenu["pingtype"].Enum<PingCategory>();
            var diff = MainMenu.GetMenu("advanced")["difference"] / 10;

            for (var i = 0; i < 6; i++)
            {
                TacticalMap.SendPing(type, Randomization.Randomize(new Vector2(pos.X + 500f * (float)Math.Cos(i), pos.Y + 500f * (float)Math.Sin(i)), diff));
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
            var time = (lastPingTime + delay - Game.Time.ToTicks()) / 1000f;

            Drawing.DrawText(pos.X - 30, pos.Y + 50, Color.Gold, MainMenu.GetTranslation("time_to_ping").Replace("[TIME]", $"{time:F1}"));
        }
    }
}