namespace SparkTech.SDK.Utils
{
    using System;
    using System.Drawing;

    using EloBuddy;

    /// <summary>
    /// The <see cref="ObjectTextEntry"/> class
    /// </summary>
    public class ObjectTextEntry
    {
        /// <summary>
        /// The <see cref="Func{TResult}"/> whether to draw on the specified <see cref="GameObject"/>
        /// </summary>
        public readonly Predicate<GameObject> Draw;

        /// <summary>
        /// The <see cref="System.Drawing.Color"/> <see cref="Func{TResult}"/>
        /// </summary>
        public readonly Func<GameObject, Color> Color;

        /// <summary>
        /// The condition <see cref="Func{TResult}"/>
        /// </summary>
        public readonly Predicate Condition;

        /// <summary>
        /// The drawing <see cref="E:id"/>
        /// </summary>
        public readonly ushort Id;

        /// <summary>
        /// The text to be drawn
        /// </summary>
        public readonly Func<GameObject, string> DrawnText;

        /// <summary>
        /// The text to appear in the Menu
        /// </summary>
        public readonly string MenuText;

        /// <summary>
        /// Indicates whether this item should be enabled by default
        /// </summary>
        public readonly bool OnByDefault;

        /// <summary>
        /// Responsible for delivering the Ids
        /// </summary>
        private static ushort id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectTextEntry"></see> class
        /// </summary>
        /// <param name="drawOnObject">The <see cref="Func{TResult}"/> whether to draw on the specified <see cref="GameObject"/></param>
        /// <param name="color">The <see cref="System.Drawing.Color"/> <see cref="Func{TResult}"/></param>
        /// <param name="condition">The condition <see cref="Func{TResult}"/></param>
        /// <param name="drawnText">The text to be drawn</param>
        /// <param name="menuText">The text to appear in the Menu</param>
        /// <param name="onByDefault">Indicates whether this item should be enabled by default</param>
        public ObjectTextEntry(
            Predicate<GameObject> drawOnObject, 
            Func<GameObject, Color> color, 
            Predicate condition, 
            Func<GameObject, string> drawnText, 
            string menuText, 
            bool onByDefault = true)
        {
            this.Draw = drawOnObject ?? (o => false);

            this.Color = color ?? (o => System.Drawing.Color.Gold);

            this.Condition = condition ?? (() => false);

            this.DrawnText = drawnText ?? (o => null);

            this.MenuText = menuText ?? "Unnamed item";

            this.OnByDefault = onByDefault;

            this.Id = id++;
        }
    }
}