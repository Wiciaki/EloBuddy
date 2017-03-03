namespace Bootstrap
{
    using System;
    using System.Drawing;
    using System.Globalization;

    using EloBuddy;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    public static class Program
    {
        private static void Main(string[] args)
        {
            args.Init();

            EloBuddy.SDK.Events.Loading.OnLoadingComplete += delegate
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

                            Drawing.DrawText(Drawing.WorldToScreen(ObjectCache.Player.Position), Color.Magenta, CultureInfo.InstalledUICulture.DisplayName, 25);
                        };
                };
        }
    }
}