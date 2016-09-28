﻿namespace SparkTech.SDK.Utils
{
    using System;
    using System.Text.RegularExpressions;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;

    public static class Extensions
    {
        public static TEnum GetValue<TEnum>(this ComboBox item) where TEnum : struct, IConvertible
        {
            return EnumCache<TEnum>.Parse(item.SelectedText);
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
        /// Gets the unique name
        /// </summary>
        /// <param name="hero">The <see cref="AIHeroClient"/> instance</param>
        /// <returns></returns>
        public static string UniqueName(this AIHeroClient hero)
        {
            return $"{hero.ChampionName()} ({hero.Name})";
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

        public static string Remove(this string @string, string text)
        {
            return @string.Replace(text, "");
        }

        public static void Catch(this Exception ex)
        {
            
        }
    }
}