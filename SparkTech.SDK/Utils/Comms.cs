﻿namespace SparkTech.SDK.Utils
{
    using EloBuddy;

    using SparkTech.SDK.Executors;

    [Trigger]
    public static class Comms
    {
        static Comms()
        {
            
        }

        public static void Print(string message)
        {
            Chat.Print(message);
        }
    }
}
