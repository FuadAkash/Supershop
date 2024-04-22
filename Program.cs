using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Supershop.Data;
using System;
using Microsoft.AspNetCore.Authorization;
using Supershop.Authorization;
using Supershop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure session
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true; // Make the session cookie essential
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration time
        options.LoginPath = "/Users/Login"; // Redirect to login page if unauthorized
        options.AccessDeniedPath = "/Home/Index"; // Redirect if access is denied
        options.SlidingExpiration = true; // Extend expiration on each request

        // Event to handle access denied redirect
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.Redirect(context.RedirectUri);
                context.Response.Cookies.Append("UserUnauthorized", "true");
                return Task.CompletedTask;
            }
        };
    });


// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AdminAuthorization.PolicyName, policy =>
    {
        policy.Requirements.Add(new AdminRequirement("Admin"));
    });

    // Add policy for Admin and MTofficer authorization
    options.AddPolicy(OfficerAuthorization.PolicyName, policy =>
    {
        policy.Requirements.Add(new OfficerRequirement("Admin", "MTofficer"));
    });
});


builder.Services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, OfficerAuthorizationHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Use session middleware
app.UseSession();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
