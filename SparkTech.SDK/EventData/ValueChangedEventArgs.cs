namespace SparkTech.SDK.EventData
{
    using System;

    using SparkTech.SDK.MenuWrapper;

    public class ValueChangedEventArgs : EventArgs
    {
        public readonly MenuItem Sender;

        public readonly string PropertyName;

        public bool Process = true;

        public ValueChangedEventArgs(MenuItem item, string propName)
        {
            this.Sender = item;

            this.PropertyName = propName;
        }
    }
}