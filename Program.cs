using ToDoList.Extensions;
using ToDoList.Extentions;
using ToDoList.Helpers;
using ToDoList.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigProvider.config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSizeLimit();
builder.Services.AddDbConnection();
builder.Services.AddCorss();
builder.Services.AddIdentityConfig();
builder.Services.AddAuthConfig();
builder.Services.AddSwaggerConfig();
builder.Services.AddServices();
/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7177);
});*/

var app = builder.Build();

app.UseHsts();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");

app.UseIdentitySeedRoles(Roles.SuperAdmin, Roles.Admin, Roles.user);
app.UseAuth();
await app.UseSeeder();


app.UseCustomSwagger();
app.UseContentSecurityPolicy();

app.MapControllers();  // <-- This maps [ApiController] endpoints

// Configure MVC routing
// This section defines all the routes for the application
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add specific routes for better navigation
app.MapControllerRoute(
    name: "home",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Home", action = "Login" });

app.MapControllerRoute(
    name: "register",
    pattern: "register",
    defaults: new { controller = "Home", action = "Register" });

app.MapControllerRoute(
    name: "todo",
    pattern: "todo",
    defaults: new { controller = "Home", action = "Index" });

app.Run();