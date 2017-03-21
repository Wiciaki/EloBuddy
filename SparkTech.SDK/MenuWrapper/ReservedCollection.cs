namespace SparkTech.SDK.MenuWrapper
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ReservedCollection : Dictionary<string, Func<string>>
    {
        public ReservedCollection(params string[] args)
        {
            Array.ForEach(args, key => this.Add(key, () => "PLACEHOLDER"));
        }
    }
}