namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;

    using SharpDX;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;

    using SystemColor = System.Drawing.Color;

    [Trigger]
    public class DamageIndicator
    {
        public bool Draw = true;

        public readonly Func<AIHeroClient, float> GetDamage;

        private SystemColor color;

        public SystemColor Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = SystemColor.FromArgb(150, value);
            }
        }

        public DamageIndicator(Func<AIHeroClient, float> getDamage)
        {
            this.GetDamage = getDamage;

            this.Color = SystemColor.Gold;

            Drawable.Add(this);
        }

        private static readonly List<DamageIndicator> Drawable = new List<DamageIndicator>();

        private static readonly Vector2 DefaultOffset = new Vector2(2f, 9.3f);

        private const int Width = 106, Height = 9;

        static DamageIndicator()
        {
            Drawing.OnEndScene += delegate
                {
                    if (!Creator.MainMenu.GetMenu("features")["indicator"] || MainMenu.IsVisible)
                    {
                        return;
                    }

                    var drawable = Drawable.FindAll(s => s.Draw);

                    if (drawable.Count == 0)
                    {
                        return;
                    }

					var enemies = ObjectCache.Get<AIHeroClient>(ObjectTeam.Enemy, h => h.VisibleOnScreen && h.IsHPBarRendered);

		            if (enemies.Count == 0)
		            {
			            return;
		            }

					foreach (var enemy in enemies)
                    {
                        var position = enemy.HPBarPosition;
                        position.X += enemy.HPBarXOffset;
                        position.Y += enemy.HPBarYOffset;

                        switch (enemy.Hero)
                        {
                            case Champion.Annie:
                            case Champion.Jhin:
                              // TODO offset
                                break;
                            default:
                                position += DefaultOffset;
                                break;
                        }

                        var current = enemy.TotalShieldHealth();
                        var max = enemy.TotalShieldMaxHealth();

                        foreach (var source in drawable)
                        {
                            if (current <= 0f)
                            {
                                break;
                            }

                            var end = position;
                            end.X += Width * (current / max);

                            current -= source.GetDamage(enemy);
                            var damagePercent = Math.Max(current, 0f) / max;

                            var start = position;
                            start.X += Width * damagePercent;

                            Drawing.DrawLine(start, end, Height, source.Color);
                        }
                    }
                };
        }
    }
}