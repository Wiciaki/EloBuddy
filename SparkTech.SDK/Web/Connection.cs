namespace SparkTech.SDK.Web
{
    using System.Net;

    using SparkTech.SDK.Executors;

  //  [Trigger]
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

            var menu = Creator.MainMenu.GetMenu("st.web")["enable.main"];

            menu.PropertyChanged += delegate
                {
                    PermissionChanged?.Invoke(IsAllowed = menu);
                };

            IsAllowed = menu;
        }
    }
}