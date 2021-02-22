using Microsoft.AspNetCore.Builder;

namespace CsgoHoldem.Api.Middleware
{
    public static class MiddlewareProvider
    {
      
        public static IApplicationBuilder useAlreadyExistsMiddeware(this IApplicationBuilder builder)
        {
            return builder;
            // this is here as an example for middleware.
            // return builder.UseMiddleware<AlreadyExistsMiddeware>();
        }
        
    }
}