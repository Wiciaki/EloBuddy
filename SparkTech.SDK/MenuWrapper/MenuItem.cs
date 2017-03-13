namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy.SDK.Menu.Values;
    
    using SparkTech.SDK.Cache;
    using SparkTech.SDK.EventData;

    public sealed class MenuItem : MenuBase
    {
        public enum Type
        {
            Label,

            Bool,

            KeyBind,

            Int,

            StringList
        }

        public readonly ValueBase Instance;

        public readonly Type MenuItemType;

        public override void UpdateText()
        {
            this.Instance.DisplayName = this.GetText();
        }

        public event EventDataHandler<ValueChangedEventArgs> PropertyChanged;

        private int skipping;
      
        private ValueChangedEventArgs OnPropertyChanged(string name)
        {
            var args = new ValueChangedEventArgs(this, name);

            if (this.skipping-- > 0)
            {
                return args;
            }

            this.PropertyChanged?.Invoke(args);

            return args;
        }

        #region Label

        public MenuItem(string translationName) : base(translationName)
        {
            this.Instance = new Label("PLACEHOLDER");

            this.MenuItemType = Type.Label;
        }

        #endregion

        #region Bool

        private bool boolVal;

        public MenuItem(string translationName, bool defValue) : base(translationName)
        {
            var item = new CheckBox("PLACEHOLDER", defValue);

            this.Instance = item;

            this.boolVal = item.CurrentValue;

            this.MenuItemType = Type.Bool;

            item.OnValueChange += (sender, args) =>
                {
                    this.boolVal = args.NewValue;
                    
                    if (!this.OnPropertyChanged(nameof(this.Bool)).Process)
                    {
                        this.skipping = 2;
                        item.CurrentValue = args.OldValue;
                    }
                };
        }

        public bool Bool
        {
            get
            {
                return this;
            }
            set
            {
                if (this.boolVal == value)
                {
                    return;
                }

                this.boolVal = value;

                ((ValueBase<bool>)this.Instance).CurrentValue = value;
            }
        }

        public static implicit operator bool(MenuItem item)
        {
            switch (item.MenuItemType)
            {
                case Type.Bool:
                case Type.KeyBind:
                    return item.boolVal;
                default:
                    throw new InvalidOperationException("Invalid property called on a MenuItem!");
            }
        }

        #endregion

        #region Keybind

        public MenuItem(string translationName, bool defVal, KeyBind.BindTypes bindType, char defKey, char defKey2) : base(translationName)
        {
            var item = new KeyBind("PLACEHOLDER", defVal, bindType, defKey, defKey2);

            this.Instance = item;

            this.boolVal = item.CurrentValue;

            this.MenuItemType = Type.KeyBind;

            item.OnValueChange += (sender, args) =>
                {
                    this.boolVal = args.NewValue;

                    if (!this.OnPropertyChanged(nameof(this.Bool)).Process)
                    {
                        this.skipping = 2;
                        item.CurrentValue = args.OldValue;
                    }
                };
        }

        #endregion

        #region Int

        private int intVal;

        public MenuItem(string translationName, int defValue, int minValue = 0, int maxValue = 100) : base(translationName)
        {
            var item = new Slider("PLACEHOLDER", defValue, minValue, maxValue);

            this.Instance = item;

            this.intVal = item.CurrentValue;

            this.MenuItemType = Type.Int;

            item.OnValueChange += (sender, args) =>
                {
                    this.intVal = args.NewValue;

                    if (!this.OnPropertyChanged(nameof(this.Int)).Process)
                    {
                        this.skipping = 2;
                        item.CurrentValue = args.OldValue;
                    }
                };
        }

        public int Int
        {
            get
            {
                if (this.MenuItemType != Type.Int)
                {
                    throw new InvalidOperationException("Invalid property called on a MenuItem!");
                }

                return this.intVal;
            }
            set
            {
                if (this.intVal == value)
                {
                    return;
                }

                this.intVal = value;

                ((Slider)this.Instance).CurrentValue = value;
            }
        }

        public static implicit operator int(MenuItem item)
        {
            return item.Int;
        }

        #endregion

        #region StringList

        private int stringIndex;

        private string stringVal;

        private readonly List<string> stringTextValues;

        public MenuItem(string translationName, IEnumerable<string> textValues, int defaultIndex = 0) : base(translationName)
        {
            var values = textValues.ToList();

            var item = new ComboBox("PLACEHOLDER", values, defaultIndex);

            this.Instance = item;

            this.stringTextValues = values;

            this.stringIndex = item.SelectedIndex;

            this.stringVal = values[this.stringIndex];

            this.MenuItemType = Type.StringList;

            item.OnValueChange += (sender, args) =>
                {
                    var n = args.NewValue;

                    this.stringIndex = n;

                    this.stringVal = this.stringTextValues[n];

                    if (!this.OnPropertyChanged(nameof(this.StringIndex)).Process)
                    {
                        this.skipping = 2;
                        item.CurrentValue = args.OldValue;
                    }
                };
        }

        public int StringIndex
        {
            get
            {
                if (this.MenuItemType != Type.StringList)
                {
                    throw new InvalidOperationException("Invalid property called on a MenuItem!");
                }

                return this.stringIndex;
            }
            set
            {
                if (this.stringIndex == value)
                {
                    return;
                }

                this.stringIndex = value;
                this.stringVal = this.stringTextValues[value];
                ((ComboBox)this.Instance).SelectedIndex = this.stringIndex;
            }
        }

        public List<string> StringValues
        {
            get
            {
                if (this.MenuItemType != Type.StringList)
                {
                    throw new InvalidOperationException("Invalid property called on a MenuItem!");
                }

                return this.StringValues;
            }
        }

        public string String
        {
            get
            {
                if (this.MenuItemType != Type.StringList)
                {
                    throw new InvalidOperationException("Invalid property called on a MenuItem!");
                }

                return this.stringVal;
            }
        }
        
        public static implicit operator string(MenuItem item)
        {
            return item.String;
        }

        #endregion

        #region Enum

        public TEnum Enum<TEnum>() where TEnum : struct, IConvertible
        {
            return EnumCache<TEnum>.Parse(this.String);
        }

        #endregion
    }
}