#if VCONTAINER
using VContainer.Unity;

namespace SceneLauncher
{
    public interface ISceneInstaller : IInstaller
    {
        public string ScenePath { get; }
    }
}
#endif