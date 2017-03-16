namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;

    public class ReservedCollection : Dictionary<string, Func<string>>
    {
        public ReservedCollection(params string[] args)
        {
            foreach (var key in args)
            {
                this.Add(key, () => "PLACEHOLDER");
            }
        }
    }
}