﻿namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using EloBuddy.SDK.Menu.Values;

    public class Menu : MenuBase, IEnumerable<MenuItem>
    {
        public EloBuddy.SDK.Menu.Menu Instance { get; internal set; }

        public readonly string Name;

        public readonly Dictionary<string, MenuItem> Items = new Dictionary<string, MenuItem>();

        internal readonly Dictionary<string, ValueBase> PreAssign = new Dictionary<string, ValueBase>();

        public MenuItem this[string name]
        {
            get
            {
                MenuItem item;
                return this.Items.TryGetValue(name, out item) ? item : null;
            }
            set
            {
                this.Add(name, value);
            }
        }

        public Menu(string name, string translationKey) : base(translationKey)
        {
            this.Name = name;
        }

        public MenuItem Add(string name, MenuItem item)
        {
            if (this.Root != null)
            {
                item.Root = this.Root;
            }

            this.Items.Add(name, item);

            if (this.Instance != null)
            {
                this.Instance.Add(name, item.Instance);
            }
            else
            {
                this.PreAssign.Add(name, item.Instance);
            }

            return item;
        }

        public override void UpdateText()
        {
            this.Instance.DisplayName = this.GetText();
        }

        public void AddSeparator(int height = 25)
        {
            this.Instance.AddSeparator(height);
        }

        public void AddLabel(string translationKey)
        {
            this.Add(translationKey.Replace('_', '.'), new MenuItem(translationKey));
        }

        IEnumerator<MenuItem> IEnumerable<MenuItem>.GetEnumerator()
        {
            return this.Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Items.Values.GetEnumerator();
        }
    }
}