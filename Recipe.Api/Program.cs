﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Recipe.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {

            var builder = WebHost.CreateDefaultBuilder(args);
            return builder.UseStartup<Startup>();
        }
           
    }
}
