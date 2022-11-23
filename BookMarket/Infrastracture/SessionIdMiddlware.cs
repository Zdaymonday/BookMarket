using Microsoft.AspNetCore.Http;

namespace BookMarket.Infrastracture
{
    public class SessionGuidMiddlware
    {
        private readonly RequestDelegate requestDelegate;

        public SessionGuidMiddlware(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Session.Keys.Contains("GUID"))
                context.Session.SetString("GUID", Guid.NewGuid().ToString());
            await requestDelegate.Invoke(context); 
        }
    }

    public static class SessionExtensions
    {
        public static IApplicationBuilder SessionGuidMiddlware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionGuidMiddlware>();
        }
    }
}
