namespace ToDoList.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' https://js.monitor.azure.com; " +
                    "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                    "img-src 'self' data: blob:; " +
                    "font-src 'self' https://fonts.gstatic.com; " +
                    "connect-src 'self' ws: wss: https://js.monitor.azure.com; " +
                    "frame-src 'none'; " +
                    "object-src 'none'; " +
                    "base-uri 'self'; " +
                    "frame-ancestors 'none';");

                await next();
            });

            return app;
        }
    }
}
