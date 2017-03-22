namespace SparkTech.Lissandra
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;

    [Trigger(Champion.Lissandra)]
    public static class Lissandra
    {
        private static Dictionary<string, string> TranslationGenerator(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["lissandra"] = "XLissandra",
                               };
            }
        }

        private static readonly MainMenu MainMenu;

        static Lissandra()
        {
            Console.WriteLine("Lissandra load...");

            MainMenu = new MainMenu("st.lissandra", "st_lissandra", TranslationGenerator);
        }
    }
}