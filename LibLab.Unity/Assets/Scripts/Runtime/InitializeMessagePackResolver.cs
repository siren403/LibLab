using MessagePack;
using MessagePack.Unity;
using UnityEngine;

namespace Runtime
{
    public static class InitializeMessagePackResolver
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard
                .WithResolver(UnityResolver.InstanceWithStandardResolver);
            // .WithResolver(MyResolver.Instance);
        }
    }

    [GeneratedMessagePackResolver]
    internal partial class MyResolver
    {
    }
}