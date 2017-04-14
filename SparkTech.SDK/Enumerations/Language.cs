namespace SparkTech.SDK.Enumerations
{
    using System.ComponentModel;

    /// <summary>
    /// Contains available translations
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// The <see cref="English"/> language
        /// </summary>
        [Description("en")]
        English = 0,

        /// <summary>
        /// The <see cref="German"/> language
        /// </summary>
        [Description("de")]
        German = 1,

        /// <summary>
        /// The <see cref="Polish"/> language
        /// </summary>
        [Description("pl")]
        Polish = 2,
    }
}