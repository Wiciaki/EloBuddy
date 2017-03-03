namespace SparkTech.SDK.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;

    using SparkTech.SDK.Cache;
    using SparkTech.SDK.Executors;
    using SparkTech.SDK.MenuWrapper;

    /// <summary>
    /// This class offers you to draw text under the objects in an easy way
    /// </summary>
   // [Trigger]
    public class ObjectText
    {
        /// <summary>
        /// Determined the height difference between consecutive drawn texts
        /// </summary>
        private const int StepSize = 25;

        /// <summary>
        /// The <see cref="List{T}"/> of champion texts to be drawn
        /// </summary>
        private static readonly List<ObjectTextEntry> Entries;

        /// <summary>
        /// Holds the text Menu item
        /// </summary>
        private static readonly Menu Menu;

        /// <summary>
        /// Initializes static members of the <see cref="ObjectText"/> class
        /// </summary>
        static ObjectText()
        {
            Menu = Creator.MainMenu.AddSubMenu("Text below units", "st_core_drawings_text");
            Menu.Add("text_enable", new MenuItem("Enable", false));

            Entries = new List<ObjectTextEntry>();

            Drawing.OnDraw += delegate
            {
                if (!Menu["text_enable"])
                {
                    return;
                }

                var enabledEntries = Entries.Where(item => Menu[$"text_{item.Id}"] && item.Condition())
                        .OrderBy(item => item.Id)
                        .ToList();

                if (enabledEntries.Count == 0)
                {
                    return;
                }

                foreach (var o in ObjectCache.GetNative<GameObject>())
                {
                    var currentObjectEntries = enabledEntries.FindAll(item => item.Draw(o));

                    if (currentObjectEntries.Count == 0)
                    {
                        continue;
                    }

                    var pos = Drawing.WorldToScreen(o.Position);
                    var steps = 0;

                    foreach (var item in currentObjectEntries)
                    {
                        var text = item.DrawnText(o);

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            Drawing.DrawText(pos.X - text.Length * 5, pos.Y - steps++ * StepSize, item.Color(o), text);
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Adds an item to the <see cref="E:TextHandlerMenu"/>
        /// </summary>
        /// <param name="item">The <see cref="ObjectText"/>'s component to be added</param>
        public static void AddItem(ObjectTextEntry item)
        {
            Menu.Add($"text_{item.Id}", new MenuItem($"Enable \"{item.MenuText}\"", item.OnByDefault));

            Entries.Add(item);
        }

        /// <summary>
        /// Removes an item from the <see cref="E:TextHandlerMenu"/>
        /// </summary>
        /// <param name="item">The <see cref="ObjectText"/>'s component to be removed</param>
        public static void RemoveItem(ObjectTextEntry item)
        {
           // Menu.Remove($"text_{item.Id}"); TODO

            Entries.RemoveAt(Entries.FindIndex(i => i.Id == item.Id));
        }
    }
}