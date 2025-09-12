using ToDoList.Data;
using ToDoList.Helpers;
using ToDoList.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ToDoList.Extentions
{
    public static class AppBuilderExtensions
    {

        public static async Task<IApplicationBuilder> UseSeeder(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<AppData>();  // Make sure AppData is your context type
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>(); // Ensure your user manager is injected
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); // Ensure your role manager is injected
                var seeder = new Seeder(dataContext, userManager, roleManager);

                await seeder.SeedSuperAdmin("osama", "123456"); // You can add a password for the SuperAdmin

            }

            return app;
        }

        public static IApplicationBuilder UseIdentitySeedRoles(this IApplicationBuilder app, params string[] roles)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.CreateScope())
            {
                IServiceProvider serviceProvider = serviceScope.ServiceProvider;
                RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                foreach (string roleName2 in roles)
                {
                    EnsureRoleExists(roleName2).GetAwaiter().GetResult();
                }

                async Task EnsureRoleExists(string roleName)
                {
                    if (!(await roleManager.RoleExistsAsync(roleName)))
                    {
                        await roleManager.CreateAsync(new IdentityRole { Name = roleName });
                    }
                }
            }

            return app;
        }
       
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();



            return app;
        }
    

        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>();
        }
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(delegate (SwaggerUIOptions opt)
            {
                opt.SwaggerEndpoint("/swagger/Main/swagger.json", "v1");
                opt.InjectStylesheet("/swagger/swagger-dark.css");
                opt.InjectJavascript("/swagger/theme-switcher.js");
                opt.RoutePrefix = "swagger";
                opt.DocumentTitle = "SuperNova";
                opt.DocExpansion(DocExpansion.None);
                opt.DisplayRequestDuration();
                opt.EnableFilter();
                opt.EnableValidator();
                opt.EnableDeepLinking();
                opt.EnablePersistAuthorization();
                opt.EnableTryItOutByDefault();
            });
            return app;
        }
        public static IApplicationBuilder UseCustomSwaggerWithAuth(this IApplicationBuilder app, string Title)
        {
            string Title2 = Title;
            app.UseSwagger();
            app.UseSwaggerAuthorized();

            app.UseSwaggerUI(delegate (SwaggerUIOptions opt)
            {
                opt.SwaggerEndpoint("/swagger/Main/swagger.json", "v1");
                opt.InjectStylesheet("/swagger/swagger-dark.css");
                opt.InjectJavascript("/swagger/theme-switcher.js");
                opt.DocumentTitle = Title2;
                opt.DocExpansion(DocExpansion.None);
                opt.DisplayRequestDuration();
                opt.EnableFilter();
                opt.EnableValidator();
                opt.EnableDeepLinking();
                opt.EnablePersistAuthorization();
                opt.EnableTryItOutByDefault();
            });
            return app;
        }
    }
}
