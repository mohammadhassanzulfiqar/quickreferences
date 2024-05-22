
using Microsoft.EntityFrameworkCore;
using OrnoBusiness.Common;
using OrnoBusiness.Data; using efcore.Model;
using OrnoBusiness.General;
using OrnoBusiness.WebCommon;
using OrnoBusinessCore.General;
using System.Runtime.CompilerServices;
using efcore;
using Microsoft.AspNetCore.Authentication.Cookies;
using OrnoBusinessCore.modules;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Application.services;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemWebAdapters();
builder.Services.AddHttpForwarder();


// Add services to the container.
AddDatabaseServices(builder);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use default (PascalCase) naming policy
    }).AddRazorRuntimeCompilation()
    .AddFluentValidation();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
Dependency(builder.Services);
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSystemWebAdapters();

//app.MapDefaultControllerRoute();
//app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

//app.MapControllerRoute("Default", "{controller=Login}/{action=Login}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Login}/{id?}");
});

app.Run();


void Dependency(IServiceCollection services)
{
    services.AddTransient<Current>();
    services.AddTransient<OBCommon>();
    services.AddTransient<CurrentSession>();
    services.AddTransient<Server>();
    services.AddUnitOfWork();
    services.AddRepositories();
    services.AddAppServices();
    services.AddValidators();
    services.AddMapster();
    services.AddGlobalQueryFilters();
    
    
}

void Authentication(IServiceCollection services)
{
    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();
}

void AddDatabaseServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DB");
    Shared.ConnectionString = connectionString;
    builder.Services.AddDbContext<OrnoBusinessContext>(
        //options => options.UseSqlServer(connectionString)
        );
}