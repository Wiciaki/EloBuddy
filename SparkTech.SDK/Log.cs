namespace SparkTech.SDK
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using EloBuddy.SDK.Utils;

    using SparkTech.SDK.Utils;

    public static class Log
    {
        public static string CurrentTime => DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture);

        private static readonly StreamWriter Writer;

        static Log()
        {
            var domain = AppDomain.CurrentDomain;

            domain.DomainUnload += delegate
                {
                    Console.Title = "SparkTech unloaded";
                    Verbose("Unloading...");
                    Writer.Write(CurrentTime);
                    Writer.Dispose();
                };

            domain.ProcessExit += delegate
                {
                    Verbose("Exiting the process...");
                    Writer.Write(CurrentTime);
                    Writer.Dispose();
                };

            var folder = FileManager.WorkingDirectory.GetFolder("Logs");
            var date = DateTime.Now;

            foreach (var fileInfo in Directory.GetFiles(folder).Select(file => new FileInfo(file)).Where(fileInfo => (date - fileInfo.CreationTime).Days > 3 && fileInfo.Name.StartsWith("Game")))
            {
                fileInfo.Delete();
            }

            var i = 0;
            string fileName;

            do
            {
                fileName = folder.GetFile($"Game_{date.Month}_{date.Day}_{++i}.txt");
            }
            while (File.Exists(fileName));

            Writer = File.CreateText(fileName);

            Writer.WriteLine("CLR: " + Environment.Version);
            Writer.WriteLine("SDK: " + typeof(Log).Assembly.GetName().Version);
            Verbose(CurrentTime + "\nInjection routine started...");
        }

        public static void Verbose(string message)
        {
            Writer.WriteLine("\nSparkTech.SDK: " + message);
        }

        public static void Warn(string message)
        {
            message = "SparkTech.SDK: " + message;

            Logger.Warn(message);

            Writer.WriteLine();
            Writer.WriteLine("========== WARNING ==========");
            Writer.WriteLine(message);
            Writer.WriteLine("=============================");
        }

        public static void Exception(Exception ex, string message)
        {
            message = "SparkTech.SDK: " + message;

            Logger.Error(message);

            Writer.WriteLine();
            Writer.WriteLine("========== EXCEPTION ==========");
            Writer.WriteLine(message);
            Writer.WriteLine();
            Writer.WriteLine(ex);
            Writer.WriteLine("===============================");
        }
    }
}