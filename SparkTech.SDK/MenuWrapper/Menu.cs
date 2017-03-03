namespace SparkTech.SDK.MenuWrapper
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

        internal readonly Dictionary<string, ValueBase> PreAssing = new Dictionary<string, ValueBase>();

        public MenuItem this[string name]
        {
            get
            {
                MenuItem item;
                return this.Items.TryGetValue(name, out item) ? item : null;
            }
            set
            {
                if (this.Root != null)
                {
                    value.Root = this.Root;
                }

                this.Items[name] = value;

                if (this.Instance != null)
                {
                    this.Instance.Add(name, value.Instance);
                }
                else
                {
                    this.PreAssing.Add(name, value.Instance);
                }
            }
        }

        public Menu(string name, string translationKey) : base(translationKey)
        {
            this.Name = name;
        }

        public MenuItem Add(string name, MenuItem item)
        {
            if (this.Items.ContainsKey(name))
            {
                throw new InvalidOperationException("Item already present");
            }

            return this[name] = item;
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