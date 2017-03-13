namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class MenuBase
    {
        protected readonly string TranslationKey;

        public MainMenu Root { get; internal set; }

        public abstract void UpdateText();

        protected MenuBase(string translationKey)
        {
            this.TranslationKey = translationKey;
        }

        private static readonly Regex BracesAroundTextRegex = new Regex(@"{\w+}");

        private static readonly Regex BraceFinderRegex = new Regex(@"[{}]");

        protected internal virtual string GetText()
        {
            if (!this.Root.Translations.ContainsKey(this.TranslationKey))
            {
                Console.WriteLine("Not present: " + this.TranslationKey);
                return this.TranslationKey;
            }

            var translation = this.Root.Translations[this.TranslationKey];

            var matches = from Match match in BracesAroundTextRegex.Matches(translation) select match.Groups[0].Value;

            return matches.Aggregate(translation, (current, m) => current.Replace(m, this.Root.Replacements[BraceFinderRegex.Replace(m, string.Empty)]()));
        }
    }
}