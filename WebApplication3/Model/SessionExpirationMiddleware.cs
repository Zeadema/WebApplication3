namespace WebApplication3.Model
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using WebApplication3.ViewModels;

    public class SessionExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Session != null)
            {
                if (context.Session.TryGetValue("SessionToken", out var sessionToken))
                {
                    var sessionTimeout = TimeSpan.FromMinutes(5); 
                    var lastActivityTime = context.Session.Get<DateTime>("LastActivityTime");

                    if (DateTime.Now - lastActivityTime > sessionTimeout)
                    {
                        context.Session.Remove("SessionToken");
                    }
                }
            }

            await _next(context);
        }
    }
}
