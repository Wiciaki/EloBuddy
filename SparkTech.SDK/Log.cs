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

            var folder = FileManager.GetFolder("Logs");
            var date = DateTime.Today;

            foreach (var file in Directory.GetFiles(folder).Where(file => (date - File.GetCreationTime(file)).Days > 7 && Path.GetFileName(file).StartsWith("Game")))
            {
                File.Delete(file);
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
            Writer.WriteLine("SDK: " + typeof(Creator).Assembly.GetName().Version);
            Verbose(CurrentTime);
            Verbose("Injection routine started...");
        }

        public static void Verbose(string message) => Writer.WriteLine(Environment.NewLine + message);

        public static void Warn(string message)
        {
            Logger.Warn(message);

            Writer.WriteLine();
            Writer.WriteLine("========== WARNING ==========");
            Writer.WriteLine(message);
            Writer.WriteLine("=============================");
        }

        public static void Exception(Exception ex, string message)
        {
            try
            {
                Logger.Exception(message, ex);
            }
            catch
            {
                // Nice logger you got there ( ͡° ͜ʖ ͡°)
            }
            finally
            {
                Writer.WriteLine();
                Writer.WriteLine("========== EXCEPTION ==========");
                Writer.WriteLine(message);
                Writer.WriteLine();
                Writer.WriteLine(ex);
                Writer.WriteLine("===============================");
            }
        }
    }
}