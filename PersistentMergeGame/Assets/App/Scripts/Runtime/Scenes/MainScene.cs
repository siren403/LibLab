using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SceneLauncher;
using VContainer;
using VContainer.Unity;
using VExtensions.ZLogger;
using VitalRouter.VContainer;
using ZLogger.Unity;
using VExtensions.SceneNavigation.Extensions;

namespace App.Scenes
{
    public class MainScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterZLogger(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
// #if UNITY_EDITOR
                // logging.AddZLoggerRollingFile((dt, index) => $"Logs/{dt:yyyy-MM-dd}_{index}.log", 1024 * 1024);
// #endif
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(formatter => { formatter.WithEditorConsolePro(); });
                });
            });
            builder.RegisterVitalRouter(routing => { });
            builder.RegisterNavigator(navigation => { navigation.StartupRootOnlyMainScene(); });


            builder.RegisterComponentInHierarchy<BootstrapCover>();
            builder.RegisterBuildCallback(container =>
            {
                var cover = container.Resolve<BootstrapCover>();
                StartupLauncher.LaunchedTask.ContinueWith(ctx => { cover.Hide(); });
            });
        }
    }
}
