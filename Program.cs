using Blogie.Admin.Data.Repository;
using Blogie.Auth.Repository;
using Blogie.Blogger.Data.Repository;
using Blogie.DataAccess.SqlDatabaseAccess;
using Blogie.Home.Data.Repository;
using Blogie.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);



builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IUserInfoService,UserInfoService>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddTransient<IBloggerRepository, BloggerRepository>();
builder.Services.AddTransient<IAllHomeRepository, AllHomeRepository>();
builder.Services.AddTransient<IAdminRepository, AdminRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        cookie =>
            cookie.LoginPath = "/Home/Login"
    );


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
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
