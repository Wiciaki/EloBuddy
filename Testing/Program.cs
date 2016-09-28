namespace Testing
{
    using EloBuddy;
    using EloBuddy.SDK.Events;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    using Color = System.Drawing.Color;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            args.Handle();

            Loading.OnLoadingComplete += delegate
                {
                    Drawing.OnDraw += delegate
                        {
                            foreach (var minion in ObjectCache.Get<Obj_AI_Minion>())
                            {
                                Drawing.DrawText(Drawing.WorldToScreen(minion.ServerPosition), Color.Magenta, minion.DetermineType().ToString(), 10);
                            }
                        };
                };
        }
    }
}