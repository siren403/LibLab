// Optional: Unity can't load default namespace to Source Generator
// If not specified, 'MasterMemory' will be used by default,
// but you can use this attribute if you want to specify a different namespace.

// [assembly: MasterMemoryGeneratorOptions(Namespace = "MyProj")]

// Optional: If you want to use init keyword, copy-and-paste this.

using UnityEditor.UIElements;

[assembly: UxmlNamespacePrefix("MasterMemory.Sample.UI", "mmui")]

namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit
    {
    }
}
