namespace SparkTech.SDK.Utils
{
    using EloBuddy;

    using SparkTech.SDK.Executors;

    /// <summary>
    /// The class responsible for delivering messages to the Player
    /// </summary>
    [Trigger]
    public static class Comms
    {
        /// <summary>
        /// Displays the specified message
        /// </summary>
        /// <param name="message">The message to be printed</param>
        public static void Print(string message)
        {
            Chat.Print("<font color='#9EB9D4'>SparkTech.SDK:</font> " + message);
        }
    }
}