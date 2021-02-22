using System;
using Datadog.Trace;
using Datadog.Trace.Configuration;
using Recipe.Api.Config;

namespace Recipe.Api.Services
{
    public class DataDogTracerServiceProvider
    {
        public static void ProvideTracer(AppSettings appSettings)
        {
            var settings = TracerSettings.FromDefaultSources();
            settings.Environment = "prod";
            settings.ServiceName = "CsgoHoldem.api";
            settings.ServiceVersion = "alpha";
            GlobalSettings.SetDebugEnabled(appSettings.DataDogTracerDebugMode);
            settings.AgentUri = new Uri("http://localhost:8126/");
            settings.TraceEnabled = appSettings.EnableDataDogTracing;
            var tracer = new Tracer(settings);
            Tracer.Instance = tracer;
        }
    }
}