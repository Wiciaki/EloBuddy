namespace AllyPingSpammer
{
    using System;
    using System.Linq;

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
                // var allies = ObjectCache.Get<AIHeroClient>(ObjectTeam.Ally).FindAll(champ => !champ.IsMe); 
                // fucking sandbox doesn't let me use own sdk :D
                var allies = ObjectManager.Get<AIHeroClient>().Where(ally => ally.IsAlly && !ally.IsMe).ToList();

                if (allies.Count == 0)
                {
                    Chat.Print("[AllyPingSpammer] No allies detected, aborting...");
                    return;
                }

                var r = new Random();
                const string Header = "st_ally_ping";
                var shouldRandomize1 = false;
                var shouldRandomize2 = false;
                var shouldAssign = false;
                var root = MainMenu.AddMenu("AllyPingSpammer", Header);
                var hero = root.Add(Header + "_hero", new ComboBox("Ally to be spammed", allies.ConvertAll(ally => ally.ChampionName())));
                root.AddSeparator(30);
                var active = root.Add(Header + "_active", new CheckBox("TILT THE SHIT OUT OF MOTHERFUCKER"));
                root.AddSeparator(30);
                root.AddGroupLabel("Made by Spark");
                TickOperation.ExecuteOnNextTick(() => active.CurrentValue = false);
                var pingSettings = root.AddSubMenu("Advanced");
                var delay = pingSettings.Add(Header + "_delay", new Slider("Delay between attempts", 3000, 200, 10000));
                var randomizer1 = pingSettings.Add(Header + "_randomize1", new CheckBox("^ Randomize delay"));
                pingSettings.AddSeparator();
                var ping = pingSettings.Add(Header + "_pingtype", new ComboBox("PingCategory", EnumCache<PingCategory>.Names));
                var randomizer2 = pingSettings.Add(Header + "_randomize2", new CheckBox("^ Randomize PingCategory"));
                pingSettings.AddSeparator();
                var difference = pingSettings.Add(Header + "_difference", new Slider("Maximal click point randomization", 200, 20, 800));
                pingSettings.AddSeparator();
                var hider = pingSettings.Add(Header + "_hide", new CheckBox("Chat blocker active", false));
                pingSettings.AddLabel("Prevents \"You have to wait before issuing more pings\" from displaying in your chat");

                var operation = new TickOperation(
                    delay.CurrentValue,
                    delegate
                    {
                        if (!active.CurrentValue)
                            return;

                        if (randomizer1.CurrentValue)
                            shouldRandomize1 = true;

                        if (randomizer2.CurrentValue)
                            shouldRandomize2 = true;

                        var pingType = (PingCategory)Enum.Parse(typeof(PingCategory), ping.SelectedText);
                        var position = Randomizer.Randomize(allies.Find(champ => champ.ChampionName() == hero.SelectedText).ServerPosition.To2D(), difference.CurrentValue);

                        TacticalMap.SendPing(pingType, position);
                    });

                delay.OnValueChange += delegate
                {
                    shouldAssign = true;
                };

                Game.OnUpdate += delegate
                {
                    if (shouldRandomize1)
                    {
                        shouldRandomize1 = false;
                        operation.TickDelay = delay.CurrentValue = r.Next(3000, 10000);
                    }

                    if (shouldRandomize2)
                    {
                        shouldRandomize2 = false;
                        ping.SelectedIndex = Randomizer.Instance.Next(0, EnumCache<PingCategory>.Count - 1);
                    }

                    if (!shouldAssign)
                        return;

                    shouldAssign = false;
                    operation.TickDelay = delay.CurrentValue;
                };

                var name = Player.Instance.Name;

                Chat.OnClientSideMessage += arg =>
                {
                    if (!hider.CurrentValue)
                    {
                        return;
                    }

                    var message = arg.Message;

                    if (message.Contains(name) || message.Contains("ping"))
                    {
                        arg.Process = false;
                    }
                };
            };
        }
    }
}