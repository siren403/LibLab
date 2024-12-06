namespace Constants.Example
{
    public static partial class FooConstants
    {
        static FooConstants()
        {
            Register("A");

            var bar = new BarConstants();
            bar.Register("B");
            bar.Register("C");
            bar.Register("D");
        }

        private static void Register([Const] string value)
        {
        }
    }

    public partial class BarConstants
    {
        public void Register([Const] string value)
        {
        }
    }

    public partial class BarConstants
    {
        public static class Constants
        {
            public const string A = "A";
            public const string B = "B";
            public const string C = "C";
        }
    }
}