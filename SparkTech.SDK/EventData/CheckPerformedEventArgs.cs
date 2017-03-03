﻿namespace SparkTech.SDK.EventData
{
    using System;

    using EloBuddy;

    using SparkTech.SDK.Enumerations;
    using SparkTech.SDK.Utils;

    using Version = System.Version;

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
            Chat.Print(this.StatusMessage);

            // ex. "You are using the latest version of [NAME]"
            // Comms.Print(Translations.GetTranslation(("updater_" + (this.Success ? this.IsUpdated ? "updated" : "available" : "error")).Replace("[NAME]", this.assemblyName), this.MessageLanguage));
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
        /// Gets the current status message
        /// </summary>
        public string StatusMessage => this.IsUpdated ? $"{this.AssemblyName} is up to date!" : $"A new update for {this.AssemblyName} is available";

        /// <summary>
        /// The message <see cref="Language"/>
        /// </summary>
        public Language MessageLanguage /*= Translations.CurrentLanguage*/;

        /// <summary>
        /// The locally saved assembly name
        /// </summary>
        private readonly string AssemblyName;

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

            this.AssemblyName = assemblyName;
        }
    }
}