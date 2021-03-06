﻿namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;

    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.EventData;

    public sealed class MenuItem : MenuBase
    {
        public enum Type
        {
            Label,

            Separator,

            Bool,

            KeyBind,

            Int,

            StringList
        }

	    public enum LabelType
	    {
		    Label,

			GroupLabel
	    }

        public readonly ValueBase Instance;

        public readonly Type MenuItemType;

        private Predicate predicate;

        public Predicate Predicate
        {
            get
            {
                return this.predicate;
            }
            set
            {
                if (this.predicate == value)
                {
                    return;
                }

                this.predicate = value;

	            if (value != null)
	            {
		            this.IsVisible = value();
	            }
            }
        }

		public bool IsVisible
		{
			get
			{
				return this.Instance.IsVisible;
			}
			set
			{
				this.Instance.IsVisible = value;
			}
		}

        public override void UpdateText()
        {
            if (this.MenuItemType == Type.Separator)
            {
                return;
            }

            this.Instance.DisplayName = this.GetText();

            if (this.predicate != null)
            {
                this.IsVisible = this.predicate();
            }
        }

        /// <summary>
        /// Asserts that the value matches
        /// </summary>
        /// <param name="menuItemType"></param>
        /// <exception cref="InvalidOperationException">The assert failed</exception>
        private void Assert(Type menuItemType)
        {
            if (this.MenuItemType != menuItemType)
            {
                throw new InvalidOperationException($"Invalid property called on a MenuItem! Got {menuItemType}, expected {this.MenuItemType}");
            }
        }

        public event EventDataHandler<ValueChangedEventArgs> PropertyChanged;

        private int skipping;

        private bool InvokeAndDetermine(string name)
        {
            if (this.skipping-- > 0)
            {
                return false;
            }

            var args = new ValueChangedEventArgs(this, name);

            this.PropertyChanged?.Invoke(args);

            if (!args.Process)
            {
                this.skipping = 2;
                return true;
            }

            return false;
        }

        #region Separator

        public MenuItem(int height = 25) : base(null)
        {
            this.Instance = new Separator(height);

            this.MenuItemType = Type.Separator;
        }

        #endregion

        #region Label

        public MenuItem(string translationName, LabelType labelType = LabelType.Label) : base(translationName)
        {
            this.Instance = labelType == LabelType.Label ? new Label("PLACEHOLDER") : new GroupLabel("PLACEHOLDER");

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

                    if (this.InvokeAndDetermine(nameof(this.Bool)))
                    {
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
                this.Assert(Type.Bool);

                if (this.boolVal == value)
                {
                    return;
                }

                this.boolVal = value;

                this.Instance.Cast<ValueBase<bool>>().CurrentValue = value;
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

        private static KeyBind.BindTypes ConvertEnum(KeyBindType bindType)
        {
            switch (bindType)
            {
                case KeyBindType.Hold:
                    return KeyBind.BindTypes.HoldActive;
                case KeyBindType.Toggle:
                    return KeyBind.BindTypes.PressToggle;
            }

            throw new ArgumentOutOfRangeException(nameof(bindType));
        }

        public MenuItem(string translationName, bool defVal, KeyBindType bindType, char defKey, char defKey2) : base(translationName)
        {
            var item = new KeyBind("PLACEHOLDER", defVal, ConvertEnum(bindType), defKey, defKey2);

            this.Instance = item;

            this.boolVal = item.CurrentValue;

            this.MenuItemType = Type.KeyBind;

            item.OnValueChange += (sender, args) =>
                {
                    this.boolVal = args.NewValue;

                    if (this.InvokeAndDetermine(nameof(this.Bool)))
                    {
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
                    if (args.NewValue == args.OldValue)
                    {
                        return;
                    }

                    this.intVal = args.NewValue;

                    if (this.InvokeAndDetermine(nameof(this.Int)))
                    {
                        item.CurrentValue = args.OldValue;
                    }
                };
        }

        public int Int
        {
            get
            {
                this.Assert(Type.Int);

                return this.intVal;
            }
            set
            {
                this.Assert(Type.Int);

                if (this.intVal == value)
                {
                    return;
                }

                this.intVal = value;
                this.Instance.Cast<Slider>().CurrentValue = value;
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

        public MenuItem(string translationName, List<string> values, int defaultIndex = 0) : base(translationName)
        {
            var item = new ComboBox("PLACEHOLDER", values, defaultIndex);

            this.Instance = item;

            this.stringTextValues = values;

            this.stringIndex = item.SelectedIndex;

            this.stringVal = values[this.stringIndex];

            this.MenuItemType = Type.StringList;

            item.OnValueChange += (sender, args) =>
                {
                    if (args.NewValue == args.OldValue)
                    {
                        return;
                    }

                    var n = args.NewValue;

                    this.stringIndex = n;

                    this.stringVal = this.stringTextValues[n];

                    if (this.InvokeAndDetermine(nameof(this.StringIndex)))
                    {
                        item.CurrentValue = args.OldValue;
                    }
                };
        }

        /// <summary>
        /// Gets or sets the current string index
        /// </summary>
        public int StringIndex
        {
            get
            {
                this.Assert(Type.StringList);

                return this.stringIndex;
            }
            set
            {
                this.Assert(Type.StringList);

                if (this.stringIndex == value)
                {
                    return;
                }

                this.stringIndex = value;
                this.stringVal = this.stringTextValues[value];
                this.Instance.Cast<ComboBox>().SelectedIndex = this.stringIndex;
            }
        }

        /// <summary>
        /// Gets the list of string values used for this MenuItem
        /// </summary>
        public List<string> StringValues
        {
            get
            {
                this.Assert(Type.StringList);

                return this.stringTextValues;
            }
        }

        /// <summary>
        /// The string obtainer
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="MenuItem"/> was not a stringlist, or the setter value not present</exception>
        public string String
        {
            get
            {
                this.Assert(Type.StringList);

                return this.stringVal;
            }
            set
            {
                this.Assert(Type.StringList);

                if (this.stringVal == value)
                {
                    return;
                }

                var i = this.stringTextValues.IndexOf(value);

                if (i == -1)
                {
                    throw new InvalidOperationException($"Item \"{value}\" doesn't exist in this StringList.");
                }

                this.StringIndex = i;
            }
        }

        /// <summary>
        /// Implicitly obtains a string from the current instance
        /// </summary>
        /// <param name="item"></param>
        public static implicit operator string(MenuItem item)
        {
            return item.String;
        }

        #endregion

        #region Enum

        /// <summary>
        /// Gets the current enum value
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type</typeparam>
        /// <returns>The enum instance</returns>
        public TEnum Enum<TEnum>() where TEnum : struct, IConvertible
        {
            return EnumCache<TEnum>.Parse(this);
        }

        #endregion
    }
}