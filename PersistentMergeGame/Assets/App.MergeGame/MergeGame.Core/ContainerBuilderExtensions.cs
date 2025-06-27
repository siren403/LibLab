using MergeGame.Core.Internal.Managers;
using VContainer;

namespace MergeGame.Core
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCore(this IContainerBuilder builder)
        {
            builder.Register<GameManager>(Lifetime.Singleton).AsSelf();
        }
    }
}
