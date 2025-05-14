using System.Globalization;
using Agar.io_Alpfa.Models;
using Agar.io_Alpfa.Interfaces;
using Agar.io_Alpfa.Entities;
using Agar.io_Alpfa.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<GameController>();
//builder.Services.AddSingleton<IModel, GameModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

app.UseHttpsRedirection();
app.UseRouting();
app.UseWebSockets();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Begining}/{action=Begining}/{id?}")
    .WithStaticAssets();

app.Run();