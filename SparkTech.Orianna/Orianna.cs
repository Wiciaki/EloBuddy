namespace SparkTech.Orianna
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;

    [Trigger(Champion.Orianna)]
    internal static class Orianna
    {
        private static readonly MainMenu Menu;

        private static Dictionary<string, string> GenerateTranslations(Language language)
        {
            switch (language)
            {
                default:
                    return new Dictionary<string, string>
                               {
                                   ["orianna"] = "Orianna",

                                   ["combo"] = "Combo"
                               };
                case Language.German:
                    return new Dictionary<string, string>();
                case Language.Polish:
                    return new Dictionary<string, string>();
            }
        }

        static Orianna()
        {
            Menu = new MainMenu("st.orianna", "orianna", GenerateTranslations)
                       {
                           new QuickMenu("combo")
                               {
                                   [""] = new MenuItem(),

                               }
                       };
        }

    }
}