namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Utils;

    public class MainMenu : Menu
    {
        /// <summary>
        /// Gets a list of all main menus
        /// </summary>
        private static readonly List<MainMenu> Instances = new List<MainMenu>();

        public MainMenu(string name, string translationKey, Func<Language, Dictionary<string, string>> translationGenerator, ReservedCollection replacements = null, string customHeader = null) : this(name, translationKey)
        {
            this.generator = translationGenerator;

            this.Replacements = replacements;

            this.Instance = EloBuddy.SDK.Menu.MainMenu.AddMenu(customHeader ?? this.GetText(), name);

            if (customHeader != null)
            {
                this.UpdateText();
            }
        }

        private MainMenu(string name, string translationKey) : base(name, translationKey)
        {
            this.Root = this;

            Instances.Add(this);
        }

        #region Menu Stuff

        public readonly Dictionary<string, Menu> Menus = new Dictionary<string, Menu>();

        public Menu AddSubMenu(string uniqueSubMenuId, string translationName)
        {
            return this.Add(new Menu(uniqueSubMenuId, translationName));
        }

        public Menu AddSubMenu(string translationName)
        {
            return this.Add(new QuickMenu(translationName));
        }

        public Menu Add(Menu menu)
        {
            menu.Root = this;

            menu.Instance = this.Instance.AddSubMenu("SparkTech.SDK", menu.Name);

            foreach (var pair in menu.PreAssign)
            {
                menu.Instance.Add(pair.Key, pair.Value);
            }

            menu.PreAssign.Clear();

            this.Menus.Add(menu.Name, menu);

            foreach (var m in menu.Items.Values)
            {
                m.Root = this;
                m.UpdateText();
            }

            menu.UpdateText();

            return menu;
        }

        public Menu GetMenu(string name)
        {
            return this.Menus[name];
        }

        public MenuItem GetItem(string name)
        {
            return this[name] ?? this.Menus.Values.Select(menu => menu[name]).FirstOrDefault(item => item != null);
        }

        public static MainMenu Get(string name)
        {
            return Instances.Find(main => main.Name == name);
        }

        public List<MenuBase> GetComponents()
        {
            var components = new List<MenuBase>();

            components.AddRange(this.Items.Values);
            components.AddRange(this.Menus.Values);
            components.AddRange(this.Menus.Values.SelectMany(m => m.Items.Values));

            return components;
        }

        #endregion

        #region Translation Stuff

        internal static void Rebuild()
        {
            Instances.ForEach(inst =>
		            {
						inst.UpdateText();

			            inst.GetComponents().ForEach(component => component.UpdateText());
		            });
        }

        /// <summary>
        /// Gets a translation for the specified key
        /// </summary>
        /// <param name="translationKey">The translation key</param>
        /// <returns></returns>
        public string GetTranslation(string translationKey)
        {
            string v;

            if (this.translations.TryGetValue(translationKey, out v))
            {
                return v;
            }

            Log.Warn($"Suitable translation string for \"{translationKey}\" was not provided.");

            return translationKey;
        }

        public void Print(string translationKey)
        {
            Comms.Print(this.GetTranslation(translationKey));
        }

        /// <summary>
        /// Contains the pointers to current values of the keys
        /// </summary>
        public readonly Dictionary<string, Func<string>> Replacements;

        private readonly Func<Language, Dictionary<string, string>> generator;

        private readonly Dictionary<string, string> translations = new Dictionary<string, string>();

        protected sealed override string GetText()
        {
            this.translations.Clear();

            foreach (var pair in this.generator(Creator.Language))
            {
                this.translations.Add(pair.Key, pair.Value);
            }

            if (Creator.Language != default(Language))
            {
                foreach (var pair in this.generator(default(Language)))
                {
                    if (!this.translations.ContainsKey(pair.Key))
                    {
                        this.translations.Add(pair.Key, pair.Value);
                    }
                }
            }

            return base.GetText();
        }

        public sealed override void UpdateText()
        {
            this.Instance.DisplayName = this.GetText().ToUpper();
        }

        #endregion
    }
}