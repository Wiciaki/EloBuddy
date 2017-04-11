namespace DevMinionAnalyzer
{
    using System.Drawing;

    using EloBuddy;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    public static class Program
    {
        private static void Main(string[] args)
        {
            //Bootstrap.WebLoad("https://github.com/Wiciaki/EloBuddy/blob/master/RemoteAssemblies/SparkTech.Lissandra.dll?raw=true", "https://github.com/Wiciaki/EloBuddy/raw/master/RemoteAssemblies/SampleVersion.txt");

            args.Init();
        }
    }

    [Trigger]
    public static class Analyzer
    {
        static Analyzer()
        {
            Drawing.OnDraw += delegate
                {
                    ObjectCache.Get<Obj_AnimatedBuilding>();
                    ObjectCache.Get<AIHeroClient>();
                    ObjectCache.Get<Obj_AI_Turret>();
                    ObjectCache.Get<GameObject>();
                    ObjectCache.Get<MissileClient>();
                    ObjectCache.GetMinions();
                    ObjectCache.GetMinions(ObjectCache.Player.Position.ToVector2(), 500f);
                    ObjectCache.Get<Obj_GeneralParticleEmitter>();

                    foreach (var minion in ObjectCache.Get<Obj_AI_Minion>())
                    {
                        var t = minion.DetermineType();

                        var text = t.ToString();

                        if (t == AIMinionType.Unknown)
                        {
                            text += " | Name: " + minion.Name + " | BaseName: " + minion.BaseSkinName;
                        }

                        var pos = Drawing.WorldToScreen(minion.Position);

                        Drawing.DrawText(pos.X - 5, pos.Y + 10, Color.Gold, text);
                    }
                };
        }
    }
}