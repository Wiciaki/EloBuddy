namespace SparkTech.SDK
{
    using System;

    using EloBuddy;

    using SparkTech.SDK.Cache;

    public static class Shortcuts
    {
        public static Random RandomInst = new Random();

        public static AIHeroClient MyHero => ObjectCache.Player;
    }
}