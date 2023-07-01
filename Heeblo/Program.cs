using Heeblo.Implementation;
using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(z => z.UseNpgsql(builder.Configuration.GetConnectionString("HBL")));
builder.Services.AddTransient<IUser, UserRepo>();
builder.Services.AddTransient<IProject, ProjectRepo>();
builder.Services.AddTransient<IApplication, ApplicationRepo>();
builder.Services.AddTransient<IHeeblo, HeebloRepo>();
builder.Services.AddHttpContextAccessor();
//corsBuilder.AllowCredentials();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", cp =>
    {
        cp.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "123231719566-usi33hu495tmjg72f9jm6eoj6ia1ga3b.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Landing}/{id?}");

app.Run();











//using Heeblo.Implementation;
//using Heeblo.Models;
//using Heeblo.Repository;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
////builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<ApplicationDbContext>(z => z.UseNpgsql(builder.Configuration.GetConnectionString("HBL")));
//builder.Services.AddTransient<IUser, UserRepo>();
//builder.Services.AddTransient<IProject, ProjectRepo>();
//builder.Services.AddTransient<IApplication, ApplicationRepo>();
//builder.Services.AddTransient<IHeeblo, HeebloRepo>();
//builder.Services.AddHttpContextAccessor();
////corsBuilder.AllowCredentials();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(60);
//});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", cp =>
//    {
//        cp.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
//    });
//});
//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = "123231719566-usi33hu495tmjg72f9jm6eoj6ia1ga3b.apps.googleusercontent.com";
//        options.ClientSecret = "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";
//    });
//var app = builder.Build();

//// Configure the HTTP request pipeline.
////if (!app.Environment.IsDevelopment())
////{
////    app.UseExceptionHandler("/Home/Error");
////    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
////    app.UseHsts();
////}

//app.UseDeveloperExceptionPage(); 

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseSession();
//app.UseAuthentication();
//app.UseRouting();
//app.UseAuthorization(); 
//app.UseCors("AllowAll");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Auth}/{action=Landing}/{id?}");

//app.Run();
