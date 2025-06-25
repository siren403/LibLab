# VExtensions.ZLogger

## TL;DR

```csharp
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
```
