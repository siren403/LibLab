using Microsoft.Extensions.Logging;
using VContainer;
using VContainer.Unity;
using VExtensions.ZLogger;
using VitalRouter.VContainer;
using ZLogger;
using ZLogger.Unity;

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
                logging.AddZLoggerRollingFile((dt, index) => $"Logs/{dt:yyyy-MM-dd}_{index}.log", 1024 * 1024);
// #endif
                logging.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(formatter => { formatter.WithEditorConsolePro(); });
                });
            });
            builder.RegisterVitalRouter(routing => { });
        }
    }
}
