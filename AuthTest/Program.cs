using AuthTest.Data;
using AuthTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Dbcontext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("defaultConnection")
        ));
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", config =>
    {
        config.Cookie.Name = "UserLogin";
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
    });
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("UserPolicy", policyBuilder =>
    {
    policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(new string[] {UserRoles.User,UserRoles.Admin } );
        policyBuilder.RequireClaim(ClaimTypes.Role);
        policyBuilder.RequireClaim(ClaimTypes.Sid);
    });
});
builder.Services.AddScoped<IUserManager, UserManager>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
