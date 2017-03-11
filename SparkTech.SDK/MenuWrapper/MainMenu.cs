namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.SDK.Enumerations;

    public sealed class MainMenu : Menu
    {
        /// <summary>
        /// Gets a list of all main menus
        /// </summary>
        public static readonly List<MainMenu> Instances = new List<MainMenu>();

        public MainMenu(string name, string translationKey, Func<Language, Dictionary<string, string>> translationObtainer) : base(name, translationKey)
        {
            this.obtainer = translationObtainer;

            this.Root = this;

            this.Instance = EloBuddy.SDK.Menu.MainMenu.AddMenu(this.GetText(), name);

            Instances.Add(this);
        }

        #region Menu Stuff

        public readonly Dictionary<string, Menu> Menus = new Dictionary<string, Menu>();

        public Menu AddSubMenu(string uniqueSubMenuId, string translationName)
        {
            return this.Add(new Menu(uniqueSubMenuId, translationName));
        }

        public Menu Add(Menu menu)
        {
            menu.Root = this;

            foreach (var m in menu.Items.Values)
            {
                m.Root = this;
                m.UpdateText();
            }

            menu.Instance = this.Instance.AddSubMenu(menu.GetText(), menu.Name);

            foreach (var pair in menu.PreAssing)
            {
                menu.Instance.Add(pair.Key, pair.Value);
            }

            menu.PreAssing.Clear();

            this.Menus.Add(menu.Name, menu);

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

        public List<MenuBase> GetComponents()
        {
            var components = new List<MenuBase>();
            
            components.AddRange(this.Items.Values);
            components.AddRange(this.Menus.Values);
            components.AddRange(this.Menus.Values.SelectMany(m => m.Items.Values));

            return components;
        }

        public static List<MenuBase> GetAllComponents()
        {
            var components = new List<MenuBase>();

            components.AddRange(Instances);
            components.AddRange(Instances.SelectMany(c => c.GetComponents()));

            return components;
        }

        #endregion

        #region Translation Stuff

        /// <summary>
        /// Contains the pointers to current values of the keys
        /// </summary>
        public readonly Dictionary<string, Func<string>> Replacements = new Dictionary<string, Func<string>>();

        private readonly Func<Language, Dictionary<string, string>> obtainer;

        internal readonly Dictionary<string, string> Translations = new Dictionary<string, string>();

        internal override string GetText()
        {
            this.Translations.Clear();

            foreach (var pair in this.obtainer(Creator.Language))
            {
                this.Translations.Add(pair.Key, pair.Value);
            }

            if (Creator.Language != Language.English)
            {
                foreach (var pair in this.obtainer(Language.English))
                {
                    if (!this.Translations.ContainsKey(pair.Key))
                    {
                        this.Translations.Add(pair.Key, pair.Value);
                    }
                }
            }

            return base.GetText();
        }

        #endregion
    }
}