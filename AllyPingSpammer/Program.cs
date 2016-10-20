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
            args.Handle(typeof(Program));

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
                    var active = root.Add(Header + "_active", new CheckBox("TILT THE SHIT OUT OF MOTHERFUCKER", false));
                    var advanced = root.AddSubMenu("Advanced");
                    var delay = advanced.Add(Header + "_delay", new Slider("Delay between attempts", 3000, 200, 10000));
                    var delayRandomizer = advanced.Add(Header + "_randomize1", new CheckBox("^ Randomize delay"));
                    advanced.AddSeparator();
                    var ping = advanced.Add(Header + "_pingtype", new ComboBox("PingCategory", EnumCache<PingCategory>.Names));
                    var categoryRandomizer = advanced.Add(Header + "_randomize2", new CheckBox("^ Randomize PingCategory"));
                    advanced.AddSeparator();
                    var difference = advanced.Add(Header + "_difference", new Slider("Maximal click point randomization", 200, 20, 800));
                    advanced.AddSeparator();
                    var hider = advanced.Add(Header + "_hide", new CheckBox("Chat blocker active", false));
                    advanced.AddLabel("Prevents \"You have to wait before issuing more pings.\" from displaying in your chat");

                    var selectedHero = allies.Find(champ => champ.UniqueName() == hero.SelectedText);
                    var r = new Random();
                    var lastPing = 0;
                    
                    hero.OnValueChange += (s, arg) => selectedHero = allies.Find(champ => champ.UniqueName() == hero[arg.NewValue]);

                    GameTick onTick = delegate
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
                                delay.CurrentValue = r.Next(200, 10000);
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

                    TickOperation.ExecuteOnNextTick(delegate
                            {
                                active.CurrentValue = false;

                                Game.OnTick += onTick;
                            });
                };
        }
    }
}