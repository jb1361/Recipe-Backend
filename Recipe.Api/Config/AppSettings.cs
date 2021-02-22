namespace Recipe.Api.Config
{
    public class AppSettings
    {
        public string JWTSecret { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public LoggingSettings Logging { get; set; }
        public bool EnableDataDogTracing { get; set; }
        public bool DataDogTracerDebugMode { get; set; }
        /**
         * Restrict the data point values query to the last n hours for assets and system processes.
         * This speeds up the query for latest data. Reduce the number to make the query faster but if data is
         * older than the number set then it will not show on the frontend.
         */
        public int DataHistoricalHoursRestriction { get; set; } = 10;
        public bool EnableDeveloperMode { get; set; }
        public MockUser MockUser { get; set; }
        public SmtpSettings SmtpSettings { get; set; }
        public string FrontendUrl { get; set; }
    }
}