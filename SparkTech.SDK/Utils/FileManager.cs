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

        static FileManager()
        {
            BaseDirectory = new Folder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EloBuddy"));

            WorkingDirectory = BaseDirectory.GetSubFolder("SparkTech");
        }

        public class Folder
        {
            private readonly string directory;

            private readonly Dictionary<string, Folder> subFolders = new Dictionary<string, Folder>();

            public Folder(string directory)
            {
                Directory.CreateDirectory(directory);

                this.directory = directory;
            }

            public string GetFile(string fileName)
            {
                return Path.Combine(this, fileName);
            }

            public Folder GetSubFolder(string name)
            {
                if (!this.subFolders.ContainsKey(name))
                {
                    this.subFolders.Add(name, new Folder(this.GetFile(name)));
                }

                return this.subFolders[name];
            }

            public static implicit operator string(Folder folder)
            {
                return folder.directory;
            }
        }
    }
}