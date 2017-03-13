namespace SparkTech.SDK.MenuWrapper
{
    public class QuickMenu : Menu
    {
        public QuickMenu(string name) : base(name, name.Replace('.', '_'))
        {

        }
    }
}