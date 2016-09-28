namespace AllyPingSpammer
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    internal static class Program
    {
        /// <summary>
        /// The entry point of the assembly
        /// </summary>
        /// <param name="args">The empty string array</param>
        private static void Main(string[] args)
        {
            args.Handle();

            Loading.OnLoadingComplete += delegate
                {
                    var allies = ObjectCache.Get<AIHeroClient>(ObjectTeam.Ally);
                    allies.RemoveAt(allies.FindIndex(champ => champ.IsMe));

                    if (allies.Count == 0)
                    {
                        Chat.Print("[AllyPingSpammer] No allies detected, aborting...");
                        return;
                    }

                    const string Header = "st_ally_ping";
                    var root = MainMenu.AddMenu("AllyPingSpammer", Header);
                    var hero = root.Add(Header + "_hero", new ComboBox("Ally to be spammed", allies.ConvertAll(ally => ally.UniqueName())));
                    root.AddSeparator(30);
                    var active = root.Add(Header + "_active", new CheckBox("TILT THE SHIT OUT OF MOTHERFUCKER"));
                    TickOperation.ExecuteOnNextTick(() => active.CurrentValue = false);
                    var pingSettings = root.AddSubMenu("Advanced");
                    var delay = pingSettings.Add(Header + "_delay", new Slider("Delay between attempts", 3000, 200, 10000));
                    var delayRandomizer = pingSettings.Add(Header + "_randomize1", new CheckBox("^ Randomize delay"));
                    pingSettings.AddSeparator();
                    var ping = pingSettings.Add(Header + "_pingtype", new ComboBox("PingCategory", EnumCache<PingCategory>.Names));
                    var categoryRandomizer = pingSettings.Add(Header + "_randomize2", new CheckBox("^ Randomize PingCategory"));
                    pingSettings.AddSeparator();
                    var difference = pingSettings.Add(Header + "_difference", new Slider("Maximal click point randomization", 200, 20, 800));
                    pingSettings.AddSeparator();
                    var hider = pingSettings.Add(Header + "_hide", new CheckBox("Chat blocker active", false));
                    pingSettings.AddLabel("Prevents \"You have to wait before issuing more pings.\" from displaying in your chat");

                    var selectedHero = allies.Find(champ => champ.UniqueName() == hero.SelectedText);
                    var r = new Random();
                    var lastPing = 0;
                    
                    hero.OnValueChange += (s, arg) => selectedHero = allies.Find(champ => champ.UniqueName() == hero[arg.NewValue]);

                    Game.OnTick += delegate
                        {
                            if (!active.CurrentValue)
                            {
                                return;
                            }

                            var time = Game.Time.ToTicks();

                            if (lastPing > time - delay.CurrentValue)
                            {
                                return;
                            }

                            lastPing = time;

                            TacticalMap.SendPing(ping.GetValue<PingCategory>(), Randomizer.Randomize(selectedHero.Position.To2D(), difference.CurrentValue));

                            if (delayRandomizer.CurrentValue)
                            {
                                delay.CurrentValue = r.Next(3000, 10000);
                            }

                            if (categoryRandomizer.CurrentValue)
                            {
                                ping.SelectedIndex = Randomizer.Instance.Next(0, EnumCache<PingCategory>.Count - 1);
                            }
                        };

                    Chat.OnClientSideMessage += arg =>
                    {
                        if (hider.CurrentValue && arg.Message == "You must wait before issuing more pings.")
                        {
                            arg.Process = false;
                        }
                    };
                };
        }
    }
}