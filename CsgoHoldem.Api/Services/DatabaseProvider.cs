using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace CsgoHoldem.Api.Services
{
    public static class DatabaseProvider
    {
        
        private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole().AddFilter((_, level) => level == LogLevel.Information);
        });
        
        public static void ConfigureMysql(this DbContextOptionsBuilder options, string connection, bool enableDatabaseLogging)
        {
            options.UseMySql(connection, mySqlOptions =>
            {
                if (enableDatabaseLogging)
                {
                    options.UseLoggerFactory(MyLoggerFactory);
                }

                // Increase timeouts so that adding indexes to large tables dont crash
                mySqlOptions.CommandTimeout((int) TimeSpan.FromMinutes(30).TotalSeconds);
#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
                mySqlOptions.ServerVersion(new Version(10, 4, 8), ServerType.MariaDb);
            });
        }
    }
}