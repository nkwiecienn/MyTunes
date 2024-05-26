using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyTunes.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyTunesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MyTunesContext") ?? throw new InvalidOperationException("Connection string 'MyTunesContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "profile",
    pattern: "profile/{action=Profile}/{id?}",
    defaults: new { controller = "Profile", action = "Profile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=login}/{id?}");
app.UseStatusCodePagesWithReExecute("/");

app.Use(async (ctx, next) =>
{
    await next();

    if ((ctx.Response.StatusCode == 404 || ctx.Response.StatusCode == 400) && !ctx.Response.HasStarted)
    {
        string originalPath = ctx.Request.Path.Value ?? "";
        ctx.Items["originalPath"] = originalPath;
        ctx.Request.Path = "/login/";
        ctx.Response.Redirect("/login/");
        await next();
    }
});

app.Run();
