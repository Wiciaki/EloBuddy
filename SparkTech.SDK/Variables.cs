namespace SparkTech.SDK
{
    using System.Reflection;

    public static class Variables
    {
        public const string VersionSDK = "0.1.0.3";

        public const string VersionAllyPingSpammer = "1.0.1.2";

        public const string RawVariablesPath = "https://raw.githubusercontent.com/Wiciaki/EloBuddy/master/SparkTech.SDK/Variables.cs";

        public static readonly Assembly Assembly;

        static Variables()
        {
            Assembly = Assembly.GetAssembly(typeof(Variables));
        }
    }
}