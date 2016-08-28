namespace SparkTech.SDK.Executors
{
    using System;

    public static class Bootstrap
    {
        static Bootstrap()
        {
            
        }

        /// <summary>
        /// Handles the application entry point arguments
        /// </summary>
        /// <param name="args">The empty, non-null string array</param>
        public static void Handle(this string[] args)
        {
            Array.ForEach(args, Console.WriteLine);
        }
    }
}