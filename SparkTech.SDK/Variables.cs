namespace SparkTech.SDK
{
    using System.Reflection;

    public static class Variables
    {
        public const string VersionSDK = "0.1.0.2";

        public const string VersionAllyPingSpammer = "1.0.1.1";

        public static readonly Assembly Assembly;

        static Variables()
        {
            Assembly = Assembly.GetAssembly(typeof(Variables));
        }
    }
}