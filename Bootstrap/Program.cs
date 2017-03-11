namespace Bootstrap
{
    using System.Drawing;

    using EloBuddy;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    public static class Program
    {
        private static void Main(string[] args) => args.Init();
    }

    [Trigger]
    public static class Bootstrap
    {
        static Bootstrap()
        {
            Drawing.OnDraw += delegate
                {
                    foreach (var minion in ObjectCache.GetNative<Obj_AI_Minion>())
                    {
                        var t = minion.DetermineType();

                        var text = t.ToString();

                        if (t == AIMinionType.Unknown)
                        {
                            text += " | Name: " + minion.Name + " | BaseName: " + minion.BaseSkinName;
                        }

                        Drawing.DrawText(Drawing.WorldToScreen(minion.Position), Color.Magenta, text, 25);
                    }
                };
        }
    }
}