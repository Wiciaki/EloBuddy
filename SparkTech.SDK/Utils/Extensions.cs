namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;

    public static class Extensions
    {
        /// <summary>
        /// Returns a new list that will contain only instances of the requested type
        /// </summary>
        /// <typeparam name="TOld">The original type</typeparam>
        /// <typeparam name="TNew">The requested type</typeparam>
        /// <param name="source">The original collection</param>
        /// <param name="additional">The additional check on the new instance</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        public static List<TNew> OfType<TOld, TNew>(this List<TOld> source, Predicate<TNew> additional = null) where TOld : class where TNew : class, TOld
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var typed = source as List<TNew>;

            if (typed != null)
            {
                return additional != null ? typed.FindAll(additional) : typed;
            }

            var container = new List<TNew>();

            TNew temp;

            if (additional == null)
            {
                foreach (var item in source)
                {
                    if ((temp = item as TNew) != null)
                    {
                        container.Add(temp);
                    }
                }
            }
            else
            {
                foreach (var item in source)
                {
                    if ((temp = item as TNew) != null && additional(temp))
                    {
                        container.Add(temp);
                    }
                }
            }

            return container;
        }

        public static TEnum GetValue<TEnum>(this ComboBox item) where TEnum : struct, IConvertible
        {
            return EnumCache<TEnum>.Parse(item.SelectedText);
        }

        /// <summary>
        /// Gets the unique name
        /// </summary>
        /// <param name="hero">The <see cref="AIHeroClient"/> instance</param>
        /// <returns></returns>
        public static string UniqueName(this AIHeroClient hero)
        {
            return $"{hero.ChampionName()} ({hero.Name})";
        }

        /// <summary>
        /// Gets the real champion name
        /// </summary>
        /// <param name="hero">The <see cref="AIHeroClient"/> instance</param>
        /// <returns></returns>
        public static string ChampionName(this AIHeroClient hero)
        {
            var name = hero.ChampionName;

            switch (name.ToLower())
            {
                case "fiddlesticks":
                    return "Fiddlesticks";
                case "chogath":
                    return "Cho'Gath";
                case "drmundo":
                    return "Dr. Mundo";
                case "khazix":
                    return "Kha'Zix";
                case "kogmaw":
                    return "Kog'Maw";
                case "reksai":
                    return "Rek'Sai";
                case "velkoz":
                    return "Vel'Koz";
                default:
                    return name.Space();
            }
        }

        /// <summary>
        /// Spaces the input string
        /// </summary>
        /// <param name="input">The string to be spaced</param>
        /// <param name="ignoreAcronyms">If <c>true</c>, ignore acronyms</param>
        /// <returns></returns>
        public static string Space(this string input, bool ignoreAcronyms = true)
        {
            return ignoreAcronyms
                       ? Regex.Replace(input, "((?<=\\p{Ll})\\p{Lu})|((?!\\A)\\p{Lu}(?>\\p{Ll}))", " $0")
                       : Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        }
    }
}