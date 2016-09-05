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
                            
                            //var color = Color.Blue;

                            //foreach (var minion in ObjectCache.GetMinions(ObjectTeam.Enemy))
                            //{
                            //    var position = minion.Position;
                            //    var bounding = minion.BoundingRadius;

                            //    var alpha = (int)((Prediction.Health.GetPrediction(minion, ProjectileTime(minion).ToTicks()) / ObjectCache.Player.GetAutoAttackDamage(minion, true)) * 255f);

                            //    Console.WriteLine(alpha);

                            //    if (alpha < 255 && alpha > 0)
                            //    {
                            //        Drawing.DrawCircle(position, bounding, Color.FromArgb(alpha, color.R, color.G, color.B));
                            //    }
                            //}
                        };
                };
        }

        //private static float ProjectileTime(Obj_AI_Base target)
        //{
        //    var unit = ObjectCache.Player;

        //    var time = unit.AttackCastDelay * 1000f - 100f + Game.Ping / 2f;
        //    var speed = unit.IsMelee ? float.MaxValue : unit.BasicAttack.MissileSpeed;

        //    if (float.MaxValue - speed > 0.1f)
        //    {
        //        time += (unit.Distance(target) - unit.BoundingRadius) * 1000f / speed;
        //    }

        //    return time;
        //}
    }
}