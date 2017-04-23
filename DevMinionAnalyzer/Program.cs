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
	        new DamageIndicator(h => 300f);

            Drawing.OnDraw += delegate
                {
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