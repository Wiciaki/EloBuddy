namespace SparkTech.SDK.MenuWrapper
{
    public class QuickMenu : Menu
    {
        public QuickMenu(string translationKey) : base(translationKey.Replace('_', '.'), translationKey)
        {

        }
    }
}