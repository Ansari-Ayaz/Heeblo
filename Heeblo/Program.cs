using Heeblo.Implementation;
using Heeblo.Models;
using Heeblo.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(z => z.UseNpgsql(builder.Configuration.GetConnectionString("HBL")));
builder.Services.AddTransient<IUser, UserRepo>();
builder.Services.AddTransient<IProject, ProjectRepo>();
builder.Services.AddTransient<IApplication, ApplicationRepo>();
builder.Services.AddTransient<IHeeblo, HeebloRepo>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", cp =>
    {
        cp.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
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
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
