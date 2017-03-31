namespace SparkTech.SDK.EventData
{
    using System;
    
    using SparkTech.SDK.Utils;

    /// <summary>
    /// The instance containing event data regarding any of the updaters
    /// </summary>
    public class CheckPerformedEventArgs : EventArgs
    {
        /// <summary>
        /// This method prints the default check with respect to your assembly name, MessageLanguage and individual user settings
        /// </summary>
        public void Notify()
        {
            string key;

            if (!this.Success)
            {
                key = "updater_failure";
            }
            else if (this.IsUpdated)
            {
                key = "updater_updated";
            }
            else
            {
                key = "updater_outdated";
            }

            Comms.Print(Creator.MainMenu.GetTranslation(key).Replace("[NAME]", this.assemblyName));
        }

        /// <summary>
        /// The cloud <see cref="System.Version"/> of the assembly
        /// </summary>
        public readonly Version GitVersion;

        /// <summary>
        /// The local <see cref="Version"/> of the assembly
        /// </summary>
        public readonly Version LocalVersion;

        /// <summary>
        /// Determines whether the check was successful
        /// </summary>
        public bool Success => this.GitVersion != null;

        /// <summary>
        /// Determines whether this instance is updated
        /// </summary>
        public bool IsUpdated => this.LocalVersion >= this.GitVersion;

        /// <summary>
        /// The locally saved assembly name
        /// </summary>
        private readonly string assemblyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPerformedEventArgs"/> class with a cloud <see cref="Version"/>, a local <see cref="Version"/> and an assembly name
        /// </summary>
        /// <param name="gitVersion">The <see cref="Version"/> of the assembly on the cloud</param>
        /// <param name="localVersion">The local <see cref="Version"/> of the assembly</param>
        /// <param name="assemblyName">The assembly name</param>
        public CheckPerformedEventArgs(Version gitVersion, Version localVersion, string assemblyName)
        {
            this.LocalVersion = localVersion;

            this.GitVersion = gitVersion;

            this.assemblyName = assemblyName;
        }
    }
}