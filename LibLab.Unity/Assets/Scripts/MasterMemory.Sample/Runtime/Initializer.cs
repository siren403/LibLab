using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace MasterMemory.Sample
{
    public static class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupMessagePackResolver()
        {
            StaticCompositeResolver.Instance.Register(MasterMemoryResolver.Instance, StandardResolver.Instance);

            var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = options;
        }
    }
}