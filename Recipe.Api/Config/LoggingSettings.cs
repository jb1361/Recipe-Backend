namespace Recipe.Api.Config
{
    public class LoggingSettings
    {
        public LogLevelSettings LogLevel { get; set; }
        public bool EnableDatabaseLogging { get; set; }
        public bool EnablePollProcessorProfiler { get; set; }
    }
}