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

    using Color = System.Drawing.Color;

    [Trigger]
    public class DamageIndicator
    {
        public Predicate Predicate;

        public Func<AIHeroClient, float> Damage;

        private Color color;

        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = Color.FromArgb(150, value);
            }
        }

        public DamageIndicator()
        {
            Drawable.Add(this);
        }

        private static readonly List<DamageIndicator> Drawable = new List<DamageIndicator>();

        private static readonly Vector2 Offset = new Vector2(1.3f, 9.25f);

        private const int Width = 104, Height = 9;

        static DamageIndicator()
        {
            Drawing.OnEndScene += delegate
                {
                    if (EloBuddy.SDK.Menu.MainMenu.IsOpen)
                    {
                        return;
                    }

                    var enemies = ObjectCache.Get<AIHeroClient>(ObjectTeam.Enemy, h => h.VisibleOnScreen && h.IsHPBarRendered);

                    if (enemies.Count == 0)
                    {
                        return;
                    }

                    var sources = Drawable.FindAll(s => s.Damage != null && (s.Predicate == null || s.Predicate()));

                    foreach (var enemy in enemies)
                    {
                        var position = enemy.HPBarPosition;
                        position.X += enemy.HPBarXOffset;
                        position.Y += enemy.HPBarYOffset;
                        position += Offset;

                        var health = enemy.TotalShieldHealth();
                        var maxHealth = enemy.TotalShieldMaxHealth();

                        foreach (var source in sources)
                        {
                            if (health <= 0)
                            {
                                break;
                            }

                            var start = position;
                            start.X += Width * (health / maxHealth);

                            health -= source.Damage(enemy);
                            var damagePercent = Math.Max(health, 0f) / maxHealth;

                            var end = position;
                            end.X += Width * damagePercent;

                            Drawing.DrawLine(start, end, Height, source.Color);
                        }
                    }
                };
        }
    }
}