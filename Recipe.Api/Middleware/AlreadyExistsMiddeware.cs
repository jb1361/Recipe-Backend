using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Recipe.Api.Models;

namespace Recipe.Api.Middleware
{
    // Example middleware that is retired in favor of proper validation hooks.
    // This gives an example of how to intercept a request.
    public class AlreadyExistsMiddeware
    {
        private readonly RequestDelegate _next;

        public AlreadyExistsMiddeware(RequestDelegate next)
        {

            _next = next ?? throw new ArgumentNullException(nameof(next));
          
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AlreadyExistsException ex)
            {

                context.Response.Clear();
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    message = ex.Message
                }));

                return;
            }
        }
    }
}