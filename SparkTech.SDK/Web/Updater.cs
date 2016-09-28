namespace SparkTech.SDK.Web
{
    using System;
    
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.Utils;

    /// <summary>
    /// The base class for the future updater engines
    /// </summary>
    public abstract class Updater : Executable
    {
        /// <summary>
        /// The <see cref="Uri"/> representation of the user provided link
        /// </summary>
        protected readonly Uri Link;

        /// <summary>
        /// Determines whether the provided link was valid
        /// </summary>
        protected readonly bool IsLinkValid;

        /// <summary>
        /// The <see cref="E:CheckPerformed"/> invokator
        /// </summary>
        protected void RaiseEvent(Version gitVersion, Version localVersion, string assemblyName)
        {
            this.CheckPerformed?.Invoke(new CheckPerformedEventArgs(gitVersion, localVersion, assemblyName));

            this.Dispose(true);
        }

        /// <summary>
        /// Fired when the check has finished. Doesn't fire if there was no correct data provided.
        /// </summary>
        public event EventDataHandler<CheckPerformedEventArgs> CheckPerformed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Updater"/> base class
        /// </summary>
        /// <param name="link">The <see cref="string"/> representation of an user-provided link</param>
        protected Updater(string link)
        {
            if (!(this.IsLinkValid = Utility.IsLinkValid(link, out this.Link)))
            {
                new ArgumentException("[ST] Updater - The provided link was invalid!").Catch();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="managed">Determines whether managed sources should be cleaned</param>
        protected override void Dispose(bool managed)
        {
            this.CheckPerformed = null;
        }
    }
}