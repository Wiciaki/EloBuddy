namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;

    using SystemColor = System.Drawing.Color;

    [Trigger]
    public class DamageIndicator
    {
        public bool Draw = true;

        public Func<AIHeroClient, float> Damage;

        public Func<AIHeroClient, SystemColor> Color;

        public DamageIndicator(SystemColor color) : this()
        {
            color = SystemColor.FromArgb(150, color);

            this.Color = h => color;
        }

        public DamageIndicator()
        {
            Drawable.Add(this);
        }

        private static readonly List<DamageIndicator> Drawable = new List<DamageIndicator>();

        private static readonly Vector2 DefaultOffset = new Vector2(2f, 9.3f);

        private const int Width = 106, Height = 9;

        static DamageIndicator()
        {
            Drawing.OnEndScene += delegate
                {
                    if (!Creator.MainMenu.GetMenu("features")["indicator"])
                    {
                        return;
                    }

                    if (EloBuddy.SDK.Menu.MainMenu.IsOpen)
                    {
                        return;
                    }

                    var enemies = ObjectCache.Get<AIHeroClient>(ObjectTeam.Enemy, h => h.IsHPBarRendered && h.VisibleOnScreen);

                    if (enemies.Count == 0)
                    {
                        return;
                    }

                    var sources = Drawable.FindAll(s => s.Draw && s.Damage != null && s.Color != null);

                    if (sources.Count == 0)
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

                        var health = enemy.TotalShieldHealth();
                        var maxHealth = enemy.TotalShieldMaxHealth();

                        foreach (var source in sources)
                        {
                            if (health <= 0)
                            {
                                break;
                            }

                            var end = position;
                            end.X += Width * (health / maxHealth);

                            health -= source.Damage(enemy);
                            var damagePercent = Math.Max(health, 0f) / maxHealth;

                            var start = position;
                            start.X += Width * damagePercent;

                            Drawing.DrawLine(start, end, Height, source.Color(enemy));
                        }
                    }
                };
        }
    }
}