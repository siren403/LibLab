namespace SceneLauncher
{
    public sealed partial class LaunchOptions
    {
        private static readonly LaunchOptions Default = new();

        private LaunchOptions()
        {
        }

        public static LaunchOptions Create()
        {
            return Default;
        }
    }
}