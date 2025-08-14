using MasterMemory;
using ZLinq;

[assembly: ZLinqDropIn("App.Tests", DropInGenerateTypes.Everything)]
[assembly: MasterMemoryGeneratorOptions(Namespace = "App.Tests")]

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
