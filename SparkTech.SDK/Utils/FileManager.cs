namespace SparkTech.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class FileManager
    {
        /// <summary>
        /// The base EloBuddy directory
        /// </summary>
        public static readonly Folder BaseDirectory;

        /// <summary>
        /// The working directory for the executing assembly and its dependencies
        /// </summary>
        public static readonly Folder WorkingDirectory;

        private static readonly Dictionary<string, Folder> Folders = new Dictionary<string, Folder>();

        /// <summary>
        /// Gets a folder with the specified name
        /// </summary>
        /// <param name="name">The requested folder name</param>
        /// <returns></returns>
        public static Folder GetFolder(string name)
        {
            if (!Folders.ContainsKey(name))
            {
                Folders.Add(name, new Folder(Path.Combine(WorkingDirectory, name)));
            }

            return Folders[name];
        }

        static FileManager()
        {
            BaseDirectory = new Folder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy"));

            WorkingDirectory = new Folder(Path.Combine(BaseDirectory, "SparkTech"));
        }
    }

    public class Folder
    {
        private readonly string directory;

        public Folder(string directory)
        {
            Directory.CreateDirectory(directory);

            this.directory = directory;
        }

        public string GetFile(string fileName)
        {
            return Path.Combine(this, fileName);
        }

        public static implicit operator string(Folder folder)
        {
            return folder.directory;
        }
    }
}