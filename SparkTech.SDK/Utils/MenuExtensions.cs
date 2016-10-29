namespace SparkTech.SDK.Utils
{
    using System;

    using EloBuddy.SDK.Menu.Values;

    using SparkTech.SDK.Cache;

    public static class MenuExtensions
    {
        /// <summary>
        /// Quickly retrieves the selected enum value
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <param name="base">The menu item</param>
        /// <returns></returns>
        public static TEnum GetValue<TEnum>(this ValueBase @base) where TEnum : struct, IConvertible
        {
            return EnumCache<TEnum>.Parse(((ComboBox)@base).SelectedText);
        }

        public static bool Bool(this ValueBase @base)
        {
            return ((ValueBase<bool>)@base).CurrentValue;
        }

        public static int Slider(this ValueBase @base)
        {
            return ((Slider)@base).CurrentValue;
        }
    }
}