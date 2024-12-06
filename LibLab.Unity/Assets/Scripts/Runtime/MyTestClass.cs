using MessagePack;

namespace Runtime
{
    [MessagePackObject]
    public class MyTestClass
    {
        [Key(0)]
        public int MyProperty { get; set; }
    }
}