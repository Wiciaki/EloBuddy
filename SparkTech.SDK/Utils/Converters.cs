namespace SparkTech.SDK.Utils
{
    using System;

    using EloBuddy.SDK;

    using SharpDX;

    public static class Converters
    {
        public static int ToTicks(this float seconds)
        {
            return (int)(seconds * 1000f);
        }

        public static float ToSeconds(this int ticks)
        {
            return ticks / 1000f;
        }

        /// <summary>
        /// Rounds the float to the nearest <see cref="int"/> value
        /// </summary>
        /// <param name="value">The floating point-number</param>
        /// <returns></returns>
        public static int Round(this float value)
        {
            return Round((double)value);
        }

        /// <summary>
        /// Rounds the double to the nearest <see cref="int"/> value
        /// </summary>
        /// <param name="value">The floating point-number</param>
        /// <returns></returns>
        public static int Round(this double value)
        {
            return (int)Math.Round(value, MidpointRounding.ToEven);
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