using Serilog;

namespace PrintBucket.Common.Logging
{
    public static class SerilogLogger
    {
        private static bool _initialized = false;

        public static void Initialize(string logFilePath = "Logs/log.txt")
        {
            if (_initialized) return;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _initialized = true;
        }
    }
}
