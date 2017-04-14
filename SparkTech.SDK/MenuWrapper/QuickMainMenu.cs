namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;

    using SparkTech.SDK.Enumerations;

    class QuickMainMenu : MainMenu
    {
        public QuickMainMenu(
            string translationKey,
            Func<Language, Dictionary<string, string>> translationGenerator,
            ReservedCollection replacements = null,
            string customHeader = null)
            : base(translationKey.Replace('_', '.'), translationKey, translationGenerator, replacements, customHeader)
        {

        }
    }
}