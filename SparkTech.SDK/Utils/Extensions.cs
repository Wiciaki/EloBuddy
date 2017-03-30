namespace SparkTech.SDK.Utils
{
    using System;
    using System.Text.RegularExpressions;

    using EloBuddy;

    using SharpDX;

    /// <summary>
    /// Provides static methods and extensions of various sort
    /// </summary>
    public static class Extensions
    {
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
        /// Puts spaces before the capital letters in a string
        /// </summary>
        /// <param name="input">The string to be spaced</param>
        /// <param name="ignoreAcronyms">If <c>true</c>, ignore acronyms</param>
        /// <returns></returns>
        public static string Space(this string input, bool ignoreAcronyms = true)
        {
            // Thank you, Stack Overflow

            return ignoreAcronyms
                       ? Regex.Replace(input, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0")
                       : Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        }

        /// <summary>
        /// Removes parts of a string
        /// </summary>
        /// <param name="string">The original instance</param>
        /// <param name="text">The desired text to be removed</param>
        /// <returns></returns>
        public static string Remove(this string @string, string text)
        {
            return @string.Replace(text, string.Empty);
        }

        /// <summary>
        /// Converts this string instance to be usable for the menu
        /// </summary>
        /// <param name="input">The <see cref="string"/> instance</param>
        /// <returns></returns>
        public static string ToMenuUse(this string input)
        {
            return input.Space().ToLower().Replace('\'', ' ').Trim().Replace(' ', '.');
        }

        public static int ToTicks(this float seconds)
        {
            return (int)(seconds * 1000f);
        }

        public static float ToSeconds(this int ticks)
        {
            return ticks / 1000f;
        }

        public static double Root(this double value, double level = 2d)
        {
            return Math.Pow(value, 1d / level);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }
    }
}