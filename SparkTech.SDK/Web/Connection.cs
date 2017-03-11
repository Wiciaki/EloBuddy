namespace SparkTech.SDK.Web
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using EloBuddy.SDK.Events;

    public static class Connection
    {
        public delegate void PermissionChange(bool enabled);

        public static event PermissionChange PermissionChanged;

        public static WebClient WebClient { get; private set; }

        public static bool IsAllowed { get; private set; }

        static Connection()
        {
            WebClient = new WebClient();

            WebClient.Disposed += delegate
                {
                    WebClient = new WebClient();
                };

            Loading.OnLoadingComplete += delegate
                {
                    var menu = Creator.MainMenu.GetMenu("st.web")["enable.main"];

                    menu.PropertyChanged += delegate
                        {
                            PermissionChanged?.Invoke(IsAllowed = menu);
                        };

                    IsAllowed = menu;

                    if (IsAllowed)
                    {
                        PermissionChanged?.Invoke(true);
                    }
                };
        }

        public static void Execute(Action action)
        {
            var task = new Task(action);

            if (IsAllowed)
            {
                task.Start();
            }
            else
            {
                PermissionChange change = null;

                change = enabled =>
                    {
                        if (!enabled)
                        {
                            return;
                        }

                        PermissionChanged -= change;

                        task.Start();
                    };

                PermissionChanged += change;
            }
        }
    }
}