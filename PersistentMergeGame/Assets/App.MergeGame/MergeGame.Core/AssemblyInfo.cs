using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MergeGame.Core.Tests")]
[assembly: InternalsVisibleTo("MergeGame.Infrastructure")]
[assembly: InternalsVisibleTo("MergeGame.Infrastructure.Tests")]

[assembly: ZLinq.ZLinqDropInAttribute("MergeGame", ZLinq.DropInGenerateTypes.Everything)]

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
