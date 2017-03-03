namespace SparkTech.SDK.Executors
{
    using System;

    using EloBuddy;

    public static class CodeFlow
    {
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
        public class UnsafeAttribute : Attribute
        {
            
        }

        public static void Secure(Action action)
        {
            GameUpdate update = null;

            update = delegate
                {
                    Game.OnUpdate -= update;

                    action();
                };

            Game.OnUpdate += update;
        }
    }
}