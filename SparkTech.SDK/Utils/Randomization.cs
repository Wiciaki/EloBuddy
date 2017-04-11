namespace SparkTech.SDK.Utils
{
    using System;

    using SharpDX;

    /// <summary>
    /// The randomization class
    /// </summary>
    public static class Randomizer
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Randomizes a <see cref="Vector2"/>
        /// </summary>
        /// <param name="input">The specified input</param>
        /// <param name="maxDiff">The maximal difference</param>
        /// <returns></returns>
        public static Vector2 Randomize(Vector2 input, int maxDiff)
        {
            return new Vector2(Randomize(input.X, maxDiff), Randomize(input.Y, maxDiff));
        }

        /// <summary>
        /// Randomizes a <see cref="float"/>
        /// </summary>
        /// <param name="input">The specified input</param>
        /// <param name="maxDiff">The maximal difference</param>
        /// <returns></returns>
        public static float Randomize(float input, int maxDiff)
        {
            var random = Random.Next(0, maxDiff);

            return NextBool() ? input + random : input - random;
        }

        /// <summary>
        /// Generates a random condition
        /// </summary>
        /// <returns></returns>
        public static bool NextBool()
        {
            return Random.Next(0, 1) == 0;
        }
    }
}