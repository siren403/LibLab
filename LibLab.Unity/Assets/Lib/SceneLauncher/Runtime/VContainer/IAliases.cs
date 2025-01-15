#if VCONTAINER
namespace SceneLauncher.VContainer
{
    public interface IAliases
    {
        string this[string key] { get; set; }
        void Clear();
    }
}
#endif