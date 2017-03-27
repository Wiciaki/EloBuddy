namespace SparkTech.Lissandra
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;

    [Trigger(Champion.Lissandra)]
    internal static class Lissandra
    {
        private static Dictionary<string, string> TranslationGenerator(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["lissandra"] = "XLissandra",
                                   ["combo"] = "Combo"
                               };
            }
        }

        private static readonly MainMenu MainMenu;

        static Lissandra()
        {
            MainMenu = new MainMenu("st.lissandra", "lissandra", TranslationGenerator)
                           {
                               new QuickMenu("combo")
                           };
        }
    }
}