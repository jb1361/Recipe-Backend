namespace Recipe.Api.Config
{
    public class SmtpSettings
    {
        public string SmtpHost { get; set; }
        public string SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool IsAnonymous { get; set; }
    }
}