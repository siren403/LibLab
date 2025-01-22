using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using UnityEngine.UIElements;

namespace MasterMemory.Sample
{
    public static class Initializer
    {
        private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupMessagePackResolver()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            StaticCompositeResolver.Instance.Register(MasterMemoryResolver.Instance, StandardResolver.Instance);

            MessagePackSerializerOptions options =
                MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = options;

            ConverterGroup inverse = new("inverse");
            inverse.AddConverter((ref bool v) => !v);

            ConverterGroups.RegisterConverterGroup(inverse);
        }
    }
}
